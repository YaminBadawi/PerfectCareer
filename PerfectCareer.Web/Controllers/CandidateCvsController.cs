using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Authorization;
using PerfectCareer.Web.Data;
using PerfectCareer.Web.Models.Attributes;
using PerfectCareer.Web.Models.Cvs;
using PerfectCareer.Web.Models.Profiles;
using PerfectCareer.Web.ViewModels.Cvs;

namespace PerfectCareer.Web.Controllers;

[Authorize(Roles = AppRoles.Candidate)]
public sealed class CandidateCvsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CandidateCvsController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var userId =
            _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profile =
            await _context.CandidateProfiles
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item =>
                        item.UserId == userId,
                    cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var cv =
            await _context.CandidateCvs
                .AsNoTracking()
                .Where(item =>
                    item.Id == id &&
                    item.CandidateProfileId ==
                        profile.Id)
                .Select(item => new
                {
                    item.Id,
                    item.PositionId,
                    item.Status,
                    item.RowVersion,
                    PositionTitle =
                        item.Position.Title,
                    PositionLocation =
                        item.Position.Location,
                    EmploymentType =
                        item.Position.EmploymentType,
                    PositionDescription =
                        item.Position.Description
                })
                .SingleOrDefaultAsync(
                    cancellationToken);

        if (cv is null)
        {
            return NotFound();
        }

        var templateDefinitions =
            await _context.PositionTemplateAttributes
                .AsNoTracking()
                .Where(templateAttribute =>
                    templateAttribute.PositionId ==
                        cv.PositionId &&
                    !templateAttribute
                        .AttributeDefinition
                        .IsBuiltIn)
                .OrderBy(templateAttribute =>
                    templateAttribute.DisplayOrder)
                .Select(templateAttribute => new
                {
                    Definition =
                        templateAttribute
                            .AttributeDefinition
                })
                .ToArrayAsync(
                    cancellationToken);

        var definitionIds =
            templateDefinitions
                .Select(item =>
                    item.Definition.Id)
                .ToArray();

        var attributeValues =
            await _context.CandidateAttributeValues
                .AsNoTracking()
                .Where(value =>
                    value.CandidateProfileId ==
                        profile.Id &&
                    definitionIds.Contains(
                        value.AttributeDefinitionId))
                .ToArrayAsync(
                    cancellationToken);

        var valuesByDefinitionId =
            attributeValues.ToDictionary(
                value =>
                    value.AttributeDefinitionId);

        var attributeOptions =
            await _context.AttributeOptions
                .AsNoTracking()
                .Where(option =>
                    definitionIds.Contains(
                        option.AttributeDefinitionId))
                .OrderBy(option =>
                    option.Id)
                .ToArrayAsync(
                    cancellationToken);

        var optionsByDefinitionId =
            attributeOptions
                .GroupBy(option =>
                    option.AttributeDefinitionId)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(option =>
                            new CandidateCvAttributeOptionViewModel
                            {
                                Id = option.Id,
                                Label = option.Label
                            })
                        .ToArray());

        var attributes =
            templateDefinitions
                .Select(templateAttribute =>
                {
                    var definition =
                        templateAttribute.Definition;

                    valuesByDefinitionId.TryGetValue(
                        definition.Id,
                        out var value);

                    optionsByDefinitionId.TryGetValue(
                        definition.Id,
                        out var options);

                    return new CandidateCvAttributeViewModel
                    {
                        Id = definition.Id,
                        Name = definition.Name,
                        Description =
                            definition.Description,
                        Category =
                            definition.Category,
                        DataType =
                            definition.DataType,
                        TextValue =
                            value?.TextValue,
                        NumberValue =
                            value?.NumberValue,
                        DateValue =
                            value?.DateValue,
                        PeriodStart =
                            value?.PeriodStart,
                        PeriodEnd =
                            value?.PeriodEnd,
                        BooleanValue =
                            value?.BooleanValue,
                        SelectedOptionId =
                            value?.SelectedOptionId,
                        RowVersion =
                            value is null
                                ? string.Empty
                                : Convert.ToBase64String(
                                    value.RowVersion),
                        Options =
                            options ??
                            Array.Empty<CandidateCvAttributeOptionViewModel>()
                    };
                })
                .ToArray();

        var selectedProjectIds =
            await _context.CandidateCvProjects
                .AsNoTracking()
                .Where(cvProject =>
                    cvProject.CandidateCvId ==
                        cv.Id)
                .Select(cvProject =>
                    cvProject.CandidateProjectId)
                .ToArrayAsync(
                    cancellationToken);

        var projects =
            await _context.CandidateProjects
                .AsNoTracking()
                .Where(project =>
                    project.CandidateProfileId ==
                        profile.Id)
                .OrderByDescending(project =>
                    project.EndDate)
                .ThenByDescending(project =>
                    project.StartDate)
                .ThenBy(project =>
                    project.Name)
                .Select(project =>
                    new CandidateCvProjectItemViewModel
                    {
                        Id = project.Id,
                        Name = project.Name,
                        StartDate =
                            project.StartDate,
                        EndDate =
                            project.EndDate,
                        Description =
                            project.Description,

                        TechnologyTags =
                            project.ProjectTechnologyTags
                                .OrderBy(projectTag =>
                                    projectTag
                                        .TechnologyTag
                                        .Name)
                                .Select(projectTag =>
                                    projectTag
                                        .TechnologyTag
                                        .Name)
                                .ToArray(),

                        IsSelected =
                            selectedProjectIds.Contains(
                                project.Id)
                    })
                .ToArrayAsync(
                    cancellationToken);

        var model =
            new CandidateCvEditViewModel
            {
                Id = cv.Id,
                PositionTitle =
                    cv.PositionTitle,
                PositionLocation =
                    cv.PositionLocation,
                EmploymentType =
                    cv.EmploymentType,
                PositionDescription =
                    cv.PositionDescription,
                Status =
                    cv.Status,

                CvRowVersion =
                    Convert.ToBase64String(
                        cv.RowVersion),

                Profile =
                    new CandidateCvProfileViewModel
                    {
                        FirstName =
                            profile.FirstName,
                        LastName =
                            profile.LastName,
                        Location =
                            profile.Location,
                        PersonalPhotoUrl =
                            profile.PersonalPhotoUrl,

                        RowVersion =
                            Convert.ToBase64String(
                                profile.RowVersion)
                    },

                Attributes = attributes,
                Projects = projects
            };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProjects(
        int id,
        string? rowVersion,
        int[]? selectedProjectIds,
        CancellationToken cancellationToken)
    {
        var userId =
            _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        if (!TryDecodeRowVersion(
                rowVersion,
                out var originalRowVersion))
        {
            TempData["CvError"] =
                "The CV version is invalid. Reload the page.";

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        var cv =
            await _context.CandidateCvs
                .Include(item =>
                    item.SelectedProjects)
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == id &&
                        item.CandidateProfile
                            .UserId == userId,
                    cancellationToken);

        if (cv is null)
        {
            return NotFound();
        }

        if (cv.Status ==
            CandidateCvStatus.Published)
        {
            TempData["CvError"] =
                "Reopen the published CV before changing its projects.";

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        var requestedProjectIds =
            selectedProjectIds?
                .Distinct()
                .ToArray() ??
            Array.Empty<int>();

        var validProjectIds =
            requestedProjectIds.Length == 0
                ? Array.Empty<int>()
                : await _context.CandidateProjects
                    .AsNoTracking()
                    .Where(project =>
                        project.CandidateProfileId ==
                            cv.CandidateProfileId &&
                        requestedProjectIds.Contains(
                            project.Id))
                    .Select(project =>
                        project.Id)
                    .ToArrayAsync(
                        cancellationToken);

        if (validProjectIds.Length !=
            requestedProjectIds.Length)
        {
            TempData["CvError"] =
                "One or more selected projects are invalid.";

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        _context.Entry(cv)
            .Property(item =>
                item.RowVersion)
            .OriginalValue =
                originalRowVersion;

        var selectedIds =
            validProjectIds.ToHashSet();

        var linksToRemove =
            cv.SelectedProjects
                .Where(cvProject =>
                    !selectedIds.Contains(
                        cvProject.CandidateProjectId))
                .ToArray();

        _context.CandidateCvProjects
            .RemoveRange(linksToRemove);

        var currentProjectIds =
            cv.SelectedProjects
                .Select(cvProject =>
                    cvProject.CandidateProjectId)
                .ToHashSet();

        var linksToAdd =
            validProjectIds
                .Where(projectId =>
                    !currentProjectIds.Contains(
                        projectId))
                .Select(projectId =>
                    new CandidateCvProject
                    {
                        CandidateCvId = cv.Id,
                        CandidateProjectId =
                            projectId
                    })
                .ToArray();

        _context.CandidateCvProjects
            .AddRange(linksToAdd);

        cv.UpdatedAtUtc =
            DateTimeOffset.UtcNow;

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["CvError"] =
                "This CV changed in another session. Reload before saving again.";

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        TempData["CvMessage"] =
            "Selected projects saved.";

        return RedirectToAction(
            nameof(Edit),
            new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(
        int id,
        string? rowVersion,
        CancellationToken cancellationToken)
    {
        var userId =
            _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        if (!TryDecodeRowVersion(
                rowVersion,
                out var originalRowVersion))
        {
            TempData["CvError"] =
                "The CV version is invalid. Reload the page.";

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        var cv =
            await _context.CandidateCvs
                .Include(item =>
                    item.CandidateProfile)
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == id &&
                        item.CandidateProfile
                            .UserId == userId,
                    cancellationToken);

        if (cv is null)
        {
            return NotFound();
        }

        var profile =
            cv.CandidateProfile;

        var profileIsComplete =
            !string.IsNullOrWhiteSpace(
                profile.FirstName) &&
            !string.IsNullOrWhiteSpace(
                profile.LastName) &&
            !string.IsNullOrWhiteSpace(
                profile.Location) &&
            !string.IsNullOrWhiteSpace(
                profile.PersonalPhotoUrl);

        var requiredDefinitions =
            await _context.PositionTemplateAttributes
                .AsNoTracking()
                .Where(templateAttribute =>
                    templateAttribute.PositionId ==
                        cv.PositionId &&
                    !templateAttribute
                        .AttributeDefinition
                        .IsBuiltIn)
                .Select(templateAttribute => new
                {
                    Id =
                        templateAttribute
                            .AttributeDefinitionId,

                    DataType =
                        templateAttribute
                            .AttributeDefinition
                            .DataType
                })
                .ToArrayAsync(
                    cancellationToken);

        var requiredDefinitionIds =
            requiredDefinitions
                .Select(definition =>
                    definition.Id)
                .ToArray();

        var values =
            await _context.CandidateAttributeValues
                .AsNoTracking()
                .Where(value =>
                    value.CandidateProfileId ==
                        profile.Id &&
                    requiredDefinitionIds.Contains(
                        value.AttributeDefinitionId))
                .ToDictionaryAsync(
                    value =>
                        value.AttributeDefinitionId,
                    cancellationToken);

        var attributesAreComplete =
            requiredDefinitions.All(definition =>
                values.TryGetValue(
                    definition.Id,
                    out var value) &&
                HasValue(
                    value,
                    definition.DataType));

        if (!profileIsComplete ||
            !attributesAreComplete)
        {
            TempData["CvError"] =
                "Complete all highlighted information before publishing the CV.";

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        _context.Entry(cv)
            .Property(item =>
                item.RowVersion)
            .OriginalValue =
                originalRowVersion;

        var now =
            DateTimeOffset.UtcNow;

        cv.Status =
            CandidateCvStatus.Published;

        cv.PublishedAtUtc = now;
        cv.UpdatedAtUtc = now;

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["CvError"] =
                "This CV changed in another session. Reload before publishing.";

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        TempData["CvMessage"] =
            "CV published successfully.";

        return RedirectToAction(
            nameof(Edit),
            new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reopen(
        int id,
        string? rowVersion,
        CancellationToken cancellationToken)
    {
        var userId =
            _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        if (!TryDecodeRowVersion(
                rowVersion,
                out var originalRowVersion))
        {
            TempData["CvError"] =
                "The CV version is invalid. Reload the page.";

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        var cv =
            await _context.CandidateCvs
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == id &&
                        item.CandidateProfile
                            .UserId == userId,
                    cancellationToken);

        if (cv is null)
        {
            return NotFound();
        }

        if (cv.Status ==
            CandidateCvStatus.Draft)
        {
            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        _context.Entry(cv)
            .Property(item =>
                item.RowVersion)
            .OriginalValue =
                originalRowVersion;

        cv.Status =
            CandidateCvStatus.Draft;

        cv.PublishedAtUtc = null;

        cv.UpdatedAtUtc =
            DateTimeOffset.UtcNow;

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["CvError"] =
                "This CV changed in another session. Reload before editing.";

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        TempData["CvMessage"] =
            "The CV is now editable.";

        return RedirectToAction(
            nameof(Edit),
            new { id });
    }

    private static bool HasValue(
        CandidateAttributeValue value,
        AttributeDataType dataType)
    {
        return dataType switch
        {
            AttributeDataType.Text or
            AttributeDataType.MarkdownText or
            AttributeDataType.ExternalImage =>
                !string.IsNullOrWhiteSpace(
                    value.TextValue),

            AttributeDataType.Number =>
                value.NumberValue.HasValue,

            AttributeDataType.Date =>
                value.DateValue.HasValue,

            AttributeDataType.Period =>
                value.PeriodStart.HasValue &&
                value.PeriodEnd.HasValue,

            AttributeDataType.Boolean =>
                value.BooleanValue.HasValue,

            AttributeDataType.SingleChoice =>
                value.SelectedOptionId.HasValue,

            _ => false
        };
    }

    private static bool TryDecodeRowVersion(
        string? encodedRowVersion,
        out byte[] rowVersion)
    {
        rowVersion =
            Array.Empty<byte>();

        if (string.IsNullOrWhiteSpace(
                encodedRowVersion))
        {
            return false;
        }

        try
        {
            rowVersion =
                Convert.FromBase64String(
                    encodedRowVersion);

            return rowVersion.Length == 8;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}