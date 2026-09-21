using PerfectCareer.Web.ViewModels.Profiles;

namespace PerfectCareer.Web.ViewModels.Projects;

public sealed class CandidateProjectDeleteViewModel
{
    public CandidateProfileDetailsViewModel Profile { get; init; } = new();

    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateOnly StartDate { get; init; }

    public DateOnly EndDate { get; init; }

    public IReadOnlyList<string> TechnologyTags { get; init; } =
        Array.Empty<string>();

    public string RowVersion { get; init; } = string.Empty;
}