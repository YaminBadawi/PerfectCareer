using PerfectCareer.Web.Models.Cvs;

namespace PerfectCareer.Web.ViewModels.Positions;

public sealed class CandidatePositionsViewModel
{
    public IReadOnlyList<CandidatePositionListItemViewModel> Positions { get; init; } =
        Array.Empty<CandidatePositionListItemViewModel>();
}

public sealed class CandidatePositionListItemViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public string EmploymentType { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public IReadOnlyList<string> TechnologyTags { get; init; } =
        Array.Empty<string>();

    public IReadOnlyList<string> RequiredAttributes { get; init; } =
        Array.Empty<string>();

    public int? CandidateCvId { get; set; }

    public CandidateCvStatus? CvStatus { get; set; }
}