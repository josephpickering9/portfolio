namespace Portfolio.Api.Requests;

public sealed record VertexQueryRequest(string Query);

public sealed record SuggestProjectDescriptionRequest
{
    public string? AdditionalContext { get; init; }
    public Dictionary<string, string>? AdditionalData { get; init; }
    public int? TargetWordCount { get; init; }
}
