namespace PerfectCareer.Web.Models.Projects;

public sealed class CandidateProjectTechnologyTag
{
    public int CandidateProjectId { get; set; }

    public int TechnologyTagId { get; set; }

    public CandidateProject CandidateProject { get; set; } = null!;

    public TechnologyTag TechnologyTag { get; set; } = null!;
}