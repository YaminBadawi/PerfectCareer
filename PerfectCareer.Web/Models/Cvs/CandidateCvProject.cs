using PerfectCareer.Web.Models.Projects;

namespace PerfectCareer.Web.Models.Cvs;

public sealed class CandidateCvProject
{
    public int CandidateCvId { get; set; }

    public int CandidateProjectId { get; set; }

    public CandidateCv CandidateCv { get; set; } = null!;

    public CandidateProject CandidateProject { get; set; } = null!;
}