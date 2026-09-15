using System.ComponentModel.DataAnnotations;
using PerfectCareer.Web.Models.Profiles;

namespace PerfectCareer.Web.Models.Projects;

public sealed class CandidateProject
{
    public int Id { get; set; }

    public int CandidateProfileId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [Required]
    [MaxLength(10000)]
    public string Description { get; set; } = string.Empty;

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public CandidateProfile CandidateProfile { get; set; } = null!;

    public ICollection<CandidateProjectTechnologyTag> ProjectTechnologyTags { get; set; } =
        new List<CandidateProjectTechnologyTag>();
}