using System.Text.Json.Serialization;

namespace Work_Experience_Search.Services.VertexAi;

public sealed record GenerateContentRequest(
    GenerateContentMessage SystemInstruction,
    IReadOnlyList<GenerateContentMessage> Contents,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyList<GenerateContentTool>? Tools = null);

public sealed record GenerateContentMessage(string Role, IReadOnlyList<GenerateContentPart> Parts)
{
    private const string SystemRole = "system";
    private const string UserRole = "user";

    public static GenerateContentMessage FromSystem(string text) => new(SystemRole, [new GenerateContentPart(text)]);

    public static GenerateContentMessage FromUser(string text) => new(UserRole, [new GenerateContentPart(text)]);
}

public sealed record GenerateContentPart(string Text);

public sealed record GenerateContentTool(GenerateContentRetrieval Retrieval);

public sealed record GenerateContentRetrieval(VertexAiSearchSource VertexAiSearch);

public sealed record VertexAiSearchSource(string Datastore);

public sealed record GenerateContentResponse
{
    [JsonPropertyName("candidates")]
    public IReadOnlyList<Candidate> Candidates { get; init; } = [];
}

public sealed record Candidate
{
    [JsonPropertyName("content")]
    public Content? Content { get; init; }

    [JsonPropertyName("groundingMetadata")]
    public GroundingMetadata? GroundingMetadata { get; init; }

    [JsonPropertyName("citationMetadata")]
    public CitationMetadata? CitationMetadata { get; init; }

    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; init; }
}

public sealed record Content
{
    [JsonPropertyName("parts")]
    public IReadOnlyList<Part> Parts { get; init; } = [];
}

public sealed record Part
{
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

public sealed record GroundingMetadata
{
    [JsonPropertyName("groundingChunks")]
    public IReadOnlyList<GroundingChunk> GroundingChunks { get; init; } = [];
}

public sealed record GroundingChunk
{
    [JsonPropertyName("retrievedContext")]
    public RetrievedContext? RetrievedContext { get; init; }
}

public sealed record RetrievedContext
{
    [JsonPropertyName("documentName")]
    public string? DocumentName { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

public sealed record CitationMetadata
{
    [JsonPropertyName("citations")]
    public IReadOnlyList<Citation> Citations { get; init; } = [];
}

public sealed record Citation
{
    [JsonPropertyName("uri")]
    public string? Uri { get; init; }

    // Vertex populates either uri or source depending on the grounding backend.
    [JsonPropertyName("source")]
    public string? Source { get; init; }
}
