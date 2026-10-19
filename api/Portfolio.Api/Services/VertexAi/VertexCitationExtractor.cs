namespace Portfolio.Api.Services.VertexAi;

internal static class VertexCitationExtractor
{
    private const string DocumentsSegment = "documents";
    private const string DataStoresSegment = "dataStores";
    private const string TitlePrefix = "Title:";
    private const char DocumentIdSeparator = ':';

    public static IReadOnlyList<RawVertexCitation> Extract(Candidate? candidate)
    {
        if (candidate is null) return [];

        var groundingCitations = (candidate.GroundingMetadata?.GroundingChunks ?? [])
            .Select(chunk => chunk.RetrievedContext)
            .Where(context => !string.IsNullOrWhiteSpace(context?.DocumentName))
            .Select(context => FromDocumentName(context!.DocumentName!, ExtractTitle(context.Text)));

        var metadataCitations = (candidate.CitationMetadata?.Citations ?? [])
            .Select(citation => citation.Uri ?? citation.Source)
            .Where(documentName => !string.IsNullOrWhiteSpace(documentName))
            .Select(documentName => FromDocumentName(documentName!, null));

        return groundingCitations.Concat(metadataCitations).ToList();
    }

    private static RawVertexCitation FromDocumentName(string documentName, string? title) =>
        new()
        {
            Id = ExtractId(documentName),
            FeatureType = ExtractFeatureType(documentName),
            Title = title
        };

    private static Guid? ExtractId(string documentName)
    {
        var rawId = SegmentAfter(documentName, DocumentsSegment);
        if (rawId is null) return null;

        return Guid.TryParse(rawId.Split(DocumentIdSeparator)[0], out var id) ? id : null;
    }

    private static VertexFeatureType? ExtractFeatureType(string documentName)
    {
        var dataStore = SegmentAfter(documentName, DataStoresSegment)?.ToLowerInvariant();
        if (dataStore is null) return null;

        return Enum.GetValues<VertexFeatureType>()
            .Cast<VertexFeatureType?>()
            .FirstOrDefault(type => dataStore.Contains(type!.Value.ToString().ToLowerInvariant()));
    }

    private static string? SegmentAfter(string documentName, string segment)
    {
        var parts = documentName.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var index = Array.IndexOf(parts, segment);
        return index >= 0 && index + 1 < parts.Length ? parts[index + 1] : null;
    }

    private static string? ExtractTitle(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;

        var titleLine = text.Split('\n').FirstOrDefault(l => l.StartsWith(TitlePrefix, StringComparison.OrdinalIgnoreCase));
        return titleLine?[TitlePrefix.Length..].Trim();
    }
}
