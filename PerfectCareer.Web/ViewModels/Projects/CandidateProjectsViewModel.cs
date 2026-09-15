using PerfectCareer.Web.ViewModels.Profiles;

namespace PerfectCareer.Web.ViewModels.Projects;

public sealed class CandidateProjectsViewModel
{
    public CandidateProfileDetailsViewModel Profile { get; init; } = new();

    public IReadOnlyList<CandidateProjectListItemViewModel> Projects { get; init; } =
        Array.Empty<CandidateProjectListItemViewModel>();
}

public sealed class CandidateProjectListItemViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateOnly StartDate { get; init; }

    public DateOnly EndDate { get; init; }

    public string Description { get; init; } = string.Empty;

    public IReadOnlyList<string> TechnologyTags { get; init; } =
        Array.Empty<string>();
}