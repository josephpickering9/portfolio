using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Api.Gax.Grpc;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.DiscoveryEngine.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Auth;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services.VertexAi;

public interface IVertexChatbotClient
{
    Task InitialiseCachesAsync(bool ensureSchema = false, CancellationToken cancellationToken = default);
    Task UpsertFeatureAsync<T>(VertexFeatureType featureType, string documentId, T value, string? jsonSchema = null, bool ensureSchema = false, CancellationToken cancellationToken = default);
}

public sealed class VertexChatbotClient(IOptions<VertexAiOptions> options, ILogger<VertexChatbotClient> logger) : IVertexChatbotClient
{
    private const string StructuredDataStoreDisplayNameSuffix = "Structured JSON";
    private const string DefaultSchemaId = "default_schema";
    private const string DataStoreIdsField = "data_store_ids";

    private static readonly VertexFeatureType[] StructuredFeatureTypes = [VertexFeatureType.Company, VertexFeatureType.Project, VertexFeatureType.Tag];

    private static readonly JsonSerializerOptions StructSerializerOptions = new()
    {
        Converters = { new IdJsonConverterFactory(), (JsonConverter)new DateOnlyIso8601Converter() }
    };

    private readonly VertexAiOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, DataStore> _dataStoreCache = new();
    private readonly Lazy<VertexGrpcClients> _clients = new(
        () => VertexGrpcClients.Create(VertexCredentialFactory.Create(options.Value, logger)),
        LazyThreadSafetyMode.PublicationOnly);

    public async Task InitialiseCachesAsync(bool ensureSchema = false, CancellationToken cancellationToken = default)
    {
        var dataStores = await GetOrCreateDataStoresAsync(ensureSchema, cancellationToken);
        await GetOrCreateEngineAsync(dataStores, cancellationToken);
    }

    public async Task UpsertFeatureAsync<T>(VertexFeatureType featureType, string documentId, T value, string? jsonSchema = null, bool ensureSchema = false, CancellationToken cancellationToken = default)
    {
        var spec = StructuredDataStoreSpec(featureType, jsonSchema);
        await GetOrCreateDataStoreAsync(spec, ensureSchema, cancellationToken);

        var document = new Document
        {
            Name = GetDocumentName(spec.DataStoreId, documentId),
            StructData = MapToStruct(value)
        };

        await _clients.Value.Documents.UpdateDocumentAsync(new UpdateDocumentRequest
        {
            Document = document,
            AllowMissing = true
        }, cancellationToken: cancellationToken);
    }

    private async Task<IReadOnlyList<string>> GetOrCreateDataStoresAsync(bool ensureSchema, CancellationToken cancellationToken)
    {
        var dataStoreIds = new List<string>();

        foreach (var featureType in StructuredFeatureTypes)
        {
            var dataStore = await GetOrCreateDataStoreAsync(StructuredDataStoreSpec(featureType, null), ensureSchema, cancellationToken);
            dataStoreIds.Add(GetResourceId(dataStore.Name));
        }

        return dataStoreIds;
    }

    private async Task<DataStore> GetOrCreateDataStoreAsync(DataStoreSpec spec, bool ensureSchema, CancellationToken cancellationToken)
    {
        var shouldEnsureSchema = ensureSchema && spec.JsonSchema is not null;

        if (_dataStoreCache.TryGetValue(spec.DataStoreId, out var cachedDataStore))
        {
            if (shouldEnsureSchema) await EnsureSchemaUpToDateAsync(spec.DataStoreId, spec.JsonSchema!, cancellationToken);
            return cachedDataStore;
        }

        var dataStoreName = DataStoreName.FromProjectLocationDataStore(_options.ProjectId, _options.Location, spec.DataStoreId).ToString();

        try
        {
            var existing = await _clients.Value.DataStores.GetDataStoreAsync(new GetDataStoreRequest { Name = dataStoreName }, cancellationToken: cancellationToken);
            if (shouldEnsureSchema) await EnsureSchemaUpToDateAsync(spec.DataStoreId, spec.JsonSchema!, cancellationToken);

            _dataStoreCache.TryAdd(spec.DataStoreId, existing);
            return existing;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            var created = await CreateDataStoreAsync(spec, cancellationToken);
            if (shouldEnsureSchema) await EnsureSchemaUpToDateAsync(spec.DataStoreId, spec.JsonSchema!, cancellationToken);

            _dataStoreCache.TryAdd(spec.DataStoreId, created);
            return created;
        }
    }

    private async Task<DataStore> CreateDataStoreAsync(DataStoreSpec spec, CancellationToken cancellationToken)
    {
        var dataStore = new DataStore
        {
            DisplayName = spec.DisplayName,
            ContentConfig = spec.ContentConfig,
            IndustryVertical = IndustryVertical.Generic
        };
        dataStore.SolutionTypes.Add(SolutionType.Search);

        if (spec.JsonSchema is not null)
        {
            dataStore.StartingSchema = new Schema { JsonSchema = spec.JsonSchema };
        }

        var operation = await _clients.Value.DataStores.CreateDataStoreAsync(new CreateDataStoreRequest
        {
            Parent = CollectionName.FromProjectLocationCollection(_options.ProjectId, _options.Location, _options.Collection).ToString(),
            DataStoreId = spec.DataStoreId,
            DataStore = dataStore
        }, cancellationToken: cancellationToken);

        var completed = await operation.PollUntilCompletedAsync(callSettings: CallSettings.FromCancellationToken(cancellationToken));
        return completed.Result;
    }

    private async Task EnsureSchemaUpToDateAsync(string dataStoreId, string jsonSchema, CancellationToken cancellationToken)
    {
        var parent = DataStoreName.FromProjectLocationDataStore(_options.ProjectId, _options.Location, dataStoreId).ToString();
        var schemas = await _clients.Value.Schemas
            .ListSchemasAsync(new ListSchemasRequest { Parent = parent })
            .ReadPageAsync(1, cancellationToken);
        var existing = schemas.FirstOrDefault();

        try
        {
            if (existing is null)
            {
                await _clients.Value.Schemas.CreateSchemaAsync(new CreateSchemaRequest
                {
                    Parent = parent,
                    SchemaId = DefaultSchemaId,
                    Schema = new Schema { JsonSchema = jsonSchema }
                }, cancellationToken: cancellationToken);
            }
            else
            {
                var updated = existing.Clone();
                updated.JsonSchema = jsonSchema;
                await _clients.Value.Schemas.UpdateSchemaAsync(new UpdateSchemaRequest { Schema = updated }, cancellationToken: cancellationToken);
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
        {
            logger.LogWarning("Schema update already in progress for datastore {DataStoreId}", dataStoreId);
        }
    }

    private async Task<Engine> GetOrCreateEngineAsync(IReadOnlyList<string> dataStoreIds, CancellationToken cancellationToken)
    {
        var engineId = _options.GetEngineId();
        var engineName = EngineName.FromProjectLocationCollectionEngine(_options.ProjectId, _options.Location, _options.Collection, engineId).ToString();

        try
        {
            var existing = await _clients.Value.Engines.GetEngineAsync(new GetEngineRequest { Name = engineName }, cancellationToken: cancellationToken);
            await EnsureEngineDataStoresAsync(existing, dataStoreIds, cancellationToken);
            return existing;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            var engine = new Engine
            {
                DisplayName = $"{_options.Environment} Chatbot",
                IndustryVertical = IndustryVertical.Generic,
                SolutionType = SolutionType.Search,
                SearchEngineConfig = new Engine.Types.SearchEngineConfig { SearchTier = SearchTier.Enterprise }
            };
            engine.DataStoreIds.AddRange(dataStoreIds);
            engine.SearchEngineConfig.SearchAddOns.Add(SearchAddOn.Llm);

            var operation = await _clients.Value.Engines.CreateEngineAsync(new CreateEngineRequest
            {
                Parent = LocationName.FromProjectLocation(_options.ProjectId, _options.Location).ToString(),
                EngineId = engineId,
                Engine = engine
            }, cancellationToken: cancellationToken);

            var completed = await operation.PollUntilCompletedAsync(callSettings: CallSettings.FromCancellationToken(cancellationToken));
            return completed.Result;
        }
    }

    private async Task EnsureEngineDataStoresAsync(Engine engine, IReadOnlyList<string> dataStoreIds, CancellationToken cancellationToken)
    {
        var expected = dataStoreIds.ToHashSet();
        if (engine.DataStoreIds.ToHashSet().SetEquals(expected)) return;

        var updated = engine.Clone();
        updated.DataStoreIds.Clear();
        updated.DataStoreIds.AddRange(expected);

        await _clients.Value.Engines.UpdateEngineAsync(new UpdateEngineRequest
        {
            Engine = updated,
            UpdateMask = new FieldMask { Paths = { DataStoreIdsField } }
        }, cancellationToken: cancellationToken);
    }

    private DataStoreSpec StructuredDataStoreSpec(VertexFeatureType featureType, string? jsonSchema) =>
        new(
            _options.GetFeatureDataStoreId(featureType),
            $"{featureType} {StructuredDataStoreDisplayNameSuffix}",
            DataStore.Types.ContentConfig.NoContent,
            jsonSchema);

    private string GetDocumentName(string dataStoreId, string documentId) =>
        DocumentName.FromProjectLocationDataStoreBranchDocument(_options.ProjectId, _options.Location, dataStoreId, _options.Branch, documentId).ToString();

    private static string GetResourceId(string name) => name.Split("/").Last();

    private static Struct MapToStruct<T>(T value)
    {
        var element = value is null
            ? JsonSerializer.SerializeToElement(new { })
            : JsonSerializer.SerializeToElement(value, StructSerializerOptions);
        return Struct.Parser.ParseJson(element.GetRawText());
    }

    private sealed record DataStoreSpec(string DataStoreId, string DisplayName, DataStore.Types.ContentConfig ContentConfig, string? JsonSchema);

    private sealed record VertexGrpcClients(
        EngineServiceClient Engines,
        DataStoreServiceClient DataStores,
        DocumentServiceClient Documents,
        SchemaServiceClient Schemas)
    {
        public static VertexGrpcClients Create(GoogleCredential credential)
        {
            var channelCredentials = credential.ToChannelCredentials();
            return new VertexGrpcClients(
                new EngineServiceClientBuilder { ChannelCredentials = channelCredentials }.Build(),
                new DataStoreServiceClientBuilder { ChannelCredentials = channelCredentials }.Build(),
                new DocumentServiceClientBuilder { ChannelCredentials = channelCredentials }.Build(),
                new SchemaServiceClientBuilder { ChannelCredentials = channelCredentials }.Build());
        }
    }
}
