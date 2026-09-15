using System.ComponentModel.DataAnnotations;
using PerfectCareer.Web.ViewModels.Profiles;

namespace PerfectCareer.Web.ViewModels.Projects;

public sealed class CandidateProjectFormViewModel : IValidatableObject
{
    public CandidateProfileDetailsViewModel Profile { get; set; } = new();

    public int? Id { get; set; }

    [Required(ErrorMessage = "Project name is required.")]
    [StringLength(150)]
    [Display(Name = "Project name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Start date")]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "End date")]
    public DateOnly? EndDate { get; set; }

    [Required(ErrorMessage = "Project description is required.")]
    [StringLength(10000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Add at least one technology tag.")]
    [Display(Name = "Technology tags")]
    public string TechnologyTagsJson { get; set; } = string.Empty;

    public string RowVersion { get; set; } = string.Empty;

    public IReadOnlyList<string> SuggestedTechnologyTags { get; set; } =
        Array.Empty<string>();

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (StartDate.HasValue &&
            EndDate.HasValue &&
            EndDate.Value < StartDate.Value)
        {
            yield return new ValidationResult(
                "End date cannot be before start date.",
                new[] { nameof(EndDate) });
        }
    }
}