using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Portfolio.Api.Types;

namespace Portfolio.Api.Models;

public sealed class ProjectRepository
{
    [Required] public ProjectRepositoryId Id { get; set; } = ProjectRepositoryId.New();

    [Required] public string Title { get; set; } = null!;

    [Required] public string Url { get; set; } = null!;

    public int? Order { get; set; }

    public ProjectId ProjectId { get; set; }
    [Required] [JsonIgnore] public Project? Project { get; set; }
}
