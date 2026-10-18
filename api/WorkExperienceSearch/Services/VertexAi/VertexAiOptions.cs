namespace Work_Experience_Search.Services.VertexAi;

public sealed class VertexAiOptions
{
    public const string SectionName = "VertexAi";
    private const string GlobalLocation = "global";

    public string ProjectId { get; set; } = string.Empty;
    public string Location { get; set; } = GlobalLocation;
    public string Collection { get; set; } = "default_collection";
    public string Branch { get; set; } = "0";
    public string Model { get; set; } = "gemini-3.8-flash";
    public string ModelLocation { get; set; } = GlobalLocation;
    public VertexEnvironment Environment { get; set; } = VertexEnvironment.Local;
    public string QueryDataStoreSuffix { get; set; } = "project_structured_v2";
    public string DataStoreVersion { get; set; } = "v2";
    public string? CredentialsFile { get; set; }
    public string? CredentialsJson { get; set; }

    public string GenerateContentUrl()
    {
        var location = string.IsNullOrWhiteSpace(ModelLocation) ? Location : ModelLocation;
        var host = location == GlobalLocation ? "aiplatform.googleapis.com" : $"{location}-aiplatform.googleapis.com";
        return $"https://{host}/v1/projects/{ProjectId}/locations/{location}/publishers/google/models/{Model}:generateContent";
    }

    public string GetFeatureDataStoreId(VertexFeatureType featureType) =>
        $"{EnvironmentKey()}_{featureType.ToString().ToLowerInvariant()}_structured_{DataStoreVersion}";

    public string GetFeatureDataStoreResource(VertexFeatureType featureType) =>
        $"projects/{ProjectId}/locations/{Location}/collections/{Collection}/dataStores/{GetFeatureDataStoreId(featureType)}";

    public string GetEngineId() => $"{EnvironmentKey()}-chatbot";

    private string EnvironmentKey() => Environment.ToString().ToLowerInvariant();
}

public enum VertexEnvironment
{
    Local,
    Production
}
