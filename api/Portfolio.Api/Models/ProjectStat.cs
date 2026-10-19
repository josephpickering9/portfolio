using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Portfolio.Api.Types;

namespace Portfolio.Api.Models;

public sealed class ProjectStat
{
    [Required] public ProjectStatId Id { get; set; } = ProjectStatId.New();

    [Required] public string Label { get; set; } = null!;

    [Required] public string Value { get; set; } = null!;

    public int? Order { get; set; }

    public ProjectId ProjectId { get; set; }
    [Required] [JsonIgnore] public Project? Project { get; set; }
}
