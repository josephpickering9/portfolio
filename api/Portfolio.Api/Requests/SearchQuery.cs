using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Api.Requests;

public sealed record SearchQuery
{
    [FromQuery(Name = "search")]
    public string? Search { get; init; }
}
