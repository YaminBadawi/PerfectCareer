using PerfectCareer.Web.Models.Attributes;
using PerfectCareer.Web.Models.Cvs;

namespace PerfectCareer.Web.ViewModels.Cvs;

public sealed class CandidateCvEditViewModel
{
    public int Id { get; init; }

    public string PositionTitle { get; init; } =
        string.Empty;

    public string PositionLocation { get; init; } =
        string.Empty;

    public string EmploymentType { get; init; } =
        string.Empty;

    public string PositionDescription { get; init; } =
        string.Empty;

    public CandidateCvStatus Status { get; init; }

    public string CvRowVersion { get; init; } =
        string.Empty;

    public CandidateCvProfileViewModel Profile { get; init; } =
        new();

    public IReadOnlyList<CandidateCvAttributeViewModel> Attributes { get; init; } =
        Array.Empty<CandidateCvAttributeViewModel>();

    public IReadOnlyList<CandidateCvProjectItemViewModel> Projects { get; init; } =
        Array.Empty<CandidateCvProjectItemViewModel>();

    public bool IsComplete =>
        Profile.IsComplete &&
        Attributes.All(attribute =>
            !attribute.IsMissing);
}

public sealed class CandidateCvProfileViewModel
{
    public string FirstName { get; init; } =
        string.Empty;

    public string LastName { get; init; } =
        string.Empty;

    public string Location { get; init; } =
        string.Empty;

    public string PersonalPhotoUrl { get; init; } =
        string.Empty;

    public string RowVersion { get; init; } =
        string.Empty;

    public bool IsComplete =>
        !string.IsNullOrWhiteSpace(FirstName) &&
        !string.IsNullOrWhiteSpace(LastName) &&
        !string.IsNullOrWhiteSpace(Location) &&
        !string.IsNullOrWhiteSpace(PersonalPhotoUrl);

    public string FullName =>
        $"{FirstName} {LastName}".Trim();

    public string Initials
    {
        get
        {
            var firstInitial =
                string.IsNullOrWhiteSpace(FirstName)
                    ? string.Empty
                    : FirstName[..1];

            var lastInitial =
                string.IsNullOrWhiteSpace(LastName)
                    ? string.Empty
                    : LastName[..1];

            var initials =
                firstInitial + lastInitial;

            return string.IsNullOrWhiteSpace(initials)
                ? "PC"
                : initials.ToUpperInvariant();
        }
    }
}

public sealed class CandidateCvAttributeViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } =
        string.Empty;

    public string Description { get; init; } =
        string.Empty;

    public AttributeCategory Category { get; init; }

    public AttributeDataType DataType { get; init; }

    public string? TextValue { get; init; }

    public decimal? NumberValue { get; init; }

    public DateOnly? DateValue { get; init; }

    public DateOnly? PeriodStart { get; init; }

    public DateOnly? PeriodEnd { get; init; }

    public bool? BooleanValue { get; init; }

    public int? SelectedOptionId { get; init; }

    public string RowVersion { get; init; } =
        string.Empty;

    public IReadOnlyList<CandidateCvAttributeOptionViewModel> Options { get; init; } =
        Array.Empty<CandidateCvAttributeOptionViewModel>();

    public bool IsMissing =>
        DataType switch
        {
            AttributeDataType.Text or
            AttributeDataType.MarkdownText or
            AttributeDataType.ExternalImage =>
                string.IsNullOrWhiteSpace(
                    TextValue),

            AttributeDataType.Number =>
                NumberValue is null,

            AttributeDataType.Date =>
                DateValue is null,

            AttributeDataType.Period =>
                PeriodStart is null ||
                PeriodEnd is null,

            AttributeDataType.Boolean =>
                BooleanValue is null,

            AttributeDataType.SingleChoice =>
                SelectedOptionId is null,

            _ => true
        };
}

public sealed class CandidateCvAttributeOptionViewModel
{
    public int Id { get; init; }

    public string Label { get; init; } =
        string.Empty;
}

public sealed class CandidateCvProjectItemViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } =
        string.Empty;

    public DateOnly StartDate { get; init; }

    public DateOnly EndDate { get; init; }

    public string Description { get; init; } =
        string.Empty;

    public IReadOnlyList<string> TechnologyTags { get; init; } =
        Array.Empty<string>();

    public bool IsSelected { get; init; }
}
