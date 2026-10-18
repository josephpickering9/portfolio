using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Models;

public sealed class ProjectStat
{
    [Required] public ProjectStatId Id { get; set; } = ProjectStatId.New();

    [Required] public string Label { get; set; } = null!;

    [Required] public string Value { get; set; } = null!;

    public int? Order { get; set; }

    public ProjectId ProjectId { get; set; }
    [Required] [JsonIgnore] public Project? Project { get; set; }
}
