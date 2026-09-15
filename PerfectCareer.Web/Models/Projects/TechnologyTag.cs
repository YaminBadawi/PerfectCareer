using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PerfectCareer.Web.Models.Projects;

[Index(nameof(Name), IsUnique = true)]
public sealed class TechnologyTag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<CandidateProjectTechnologyTag> ProjectTechnologyTags { get; set; } =
        new List<CandidateProjectTechnologyTag>();
}
