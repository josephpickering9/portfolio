using System.ComponentModel.DataAnnotations;
using Portfolio.Api.Models;
using Portfolio.Api.Types;

namespace Portfolio.Api.Responses;

public sealed record ProjectSummary
{
    [Required] public required ProjectId Id { get; init; }

    [Required] public required string Title { get; init; }

    [Required] public required string Slug { get; init; }

    [Required] public required string ShortDescription { get; init; }

    public CompanyId? CompanyId { get; init; }

    [Required] public required DateOnly StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    [Required] public required IReadOnlyList<Tag> Tags { get; init; }

    public string? CardImageUrl { get; init; }
}
