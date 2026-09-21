using PerfectCareer.Web.Models.Cvs;
using PerfectCareer.Web.ViewModels.Profiles;

namespace PerfectCareer.Web.ViewModels.Cvs;

public sealed class CandidateCvsIndexViewModel
{
    public CandidateProfileDetailsViewModel Profile { get; init; } = new();

    public IReadOnlyList<CandidateCvListItemViewModel> Cvs { get; init; } =
        Array.Empty<CandidateCvListItemViewModel>();
}

public sealed class CandidateCvListItemViewModel
{
    public int Id { get; init; }

    public string PositionTitle { get; init; } = string.Empty;

    public string PositionLocation { get; init; } = string.Empty;

    public string EmploymentType { get; init; } = string.Empty;

    public CandidateCvStatus Status { get; init; }

    public DateTimeOffset UpdatedAtUtc { get; init; }

    public DateTimeOffset? PublishedAtUtc { get; init; }

    public int SelectedProjectCount { get; init; }
}