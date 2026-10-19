using Microsoft.AspNetCore.Mvc;

namespace Work_Experience_Search.Requests;

public sealed record SearchQuery
{
    [FromQuery(Name = "search")]
    public string? Search { get; init; }
}
