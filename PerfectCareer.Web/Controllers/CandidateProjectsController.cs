using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Authorization;
using PerfectCareer.Web.Data;
using PerfectCareer.Web.Models.Profiles;
using PerfectCareer.Web.Models.Projects;
using PerfectCareer.Web.ViewModels.Profiles;
using PerfectCareer.Web.ViewModels.Projects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PerfectCareer.Web.Controllers;

[Authorize(Roles = AppRoles.Candidate)]
public sealed class CandidateProjectsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CandidateProjectsController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profile = await _context.CandidateProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var projects = await _context.CandidateProjects
            .AsNoTracking()
            .Where(project =>
                project.CandidateProfileId == profile.Id)
            .OrderByDescending(project => project.EndDate)
            .ThenByDescending(project => project.StartDate)
            .ThenBy(project => project.Name)
            .Select(project =>
                new CandidateProjectListItemViewModel
                {
                    Id = project.Id,
                    Name = project.Name,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Description = project.Description,
                    TechnologyTags = project.ProjectTechnologyTags
                        .OrderBy(projectTag =>
                            projectTag.TechnologyTag.Name)
                        .Select(projectTag =>
                            projectTag.TechnologyTag.Name)
                        .ToArray()
                })
            .ToArrayAsync(cancellationToken);

        var model = new CandidateProjectsViewModel
        {
            Profile = CreateProfileDetails(profile),
            Projects = projects
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profile = await _context.CandidateProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var suggestedTechnologyTags = await _context.TechnologyTags
            .AsNoTracking()
            .OrderBy(tag => tag.Name)
            .Select(tag => tag.Name)
            .Take(100)
            .ToArrayAsync(cancellationToken);

        var model = new CandidateProjectFormViewModel
        {
            Profile = CreateProfileDetails(profile),
            SuggestedTechnologyTags = suggestedTechnologyTags
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CandidateProjectFormViewModel model,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profile = await _context.CandidateProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var technologyTagNames =
            ValidateTechnologyTags(model);

        if (!ModelState.IsValid)
        {
            await PopulateFormContextAsync(
                model,
                profile,
                cancellationToken);

            return View(model);
        }

        var existingTags = await _context.TechnologyTags
            .Where(tag => technologyTagNames.Contains(tag.Name))
            .ToListAsync(cancellationToken);

        var tagsByName = existingTags.ToDictionary(
            tag => tag.Name,
            StringComparer.OrdinalIgnoreCase);

        var project = new CandidateProject
        {
            CandidateProfileId = profile.Id,
            Name = model.Name.Trim(),
            StartDate = model.StartDate!.Value,
            EndDate = model.EndDate!.Value,
            Description = model.Description.Trim()
        };

        foreach (var tagName in technologyTagNames)
        {
            if (!tagsByName.TryGetValue(
                    tagName,
                    out var technologyTag))
            {
                technologyTag = new TechnologyTag
                {
                    Name = tagName
                };

                tagsByName.Add(tagName, technologyTag);
            }

            project.ProjectTechnologyTags.Add(
                new CandidateProjectTechnologyTag
                {
                    CandidateProject = project,
                    TechnologyTag = technologyTag
                });
        }

        _context.CandidateProjects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        TempData["ProjectMessage"] =
            "Project added successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profile = await _context.CandidateProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var project = await _context.CandidateProjects
            .AsNoTracking()
            .Include(item => item.ProjectTechnologyTags)
            .ThenInclude(projectTag => projectTag.TechnologyTag)
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.CandidateProfileId == profile.Id,
                cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        var model = new CandidateProjectFormViewModel
        {
            Id = project.Id,
            Name = project.Name,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Description = project.Description,
            TechnologyTagsJson = JsonSerializer.Serialize(
                project.ProjectTechnologyTags
                    .OrderBy(projectTag =>
                        projectTag.TechnologyTag.Name)
                    .Select(projectTag =>
                        new TechnologyTagInput
                        {
                            Value = projectTag.TechnologyTag.Name
                        })),
            RowVersion = Convert.ToBase64String(
                project.RowVersion)
        };

        await PopulateFormContextAsync(
            model,
            profile,
            cancellationToken);

        return View("Create", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CandidateProjectFormViewModel model,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        if (model.Id != id)
        {
            return BadRequest();
        }

        var profile = await _context.CandidateProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var project = await _context.CandidateProjects
            .Include(item => item.ProjectTechnologyTags)
            .ThenInclude(projectTag => projectTag.TechnologyTag)
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.CandidateProfileId == profile.Id,
                cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        var technologyTagNames =
            ValidateTechnologyTags(model);

        byte[]? originalRowVersion = null;

        if (string.IsNullOrWhiteSpace(model.RowVersion))
        {
            ModelState.AddModelError(
                string.Empty,
                "The project version is missing. Reload the page and try again.");
        }
        else
        {
            try
            {
                originalRowVersion =
                    Convert.FromBase64String(model.RowVersion);
            }
            catch (FormatException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The project version is invalid. Reload the page and try again.");
            }
        }

        if (!ModelState.IsValid || originalRowVersion is null)
        {
            await PopulateFormContextAsync(
                model,
                profile,
                cancellationToken);

            return View("Create", model);
        }

        var existingTags = await _context.TechnologyTags
            .Where(tag => technologyTagNames.Contains(tag.Name))
            .ToListAsync(cancellationToken);

        var tagsByName = existingTags.ToDictionary(
            tag => tag.Name,
            StringComparer.OrdinalIgnoreCase);

        var selectedTagNames = technologyTagNames.ToHashSet(
            StringComparer.OrdinalIgnoreCase);

        var projectTagsToRemove = project.ProjectTechnologyTags
            .Where(projectTag =>
                !selectedTagNames.Contains(
                    projectTag.TechnologyTag.Name))
            .ToArray();

        _context.RemoveRange(projectTagsToRemove);

        foreach (var tagName in technologyTagNames)
        {
            var projectAlreadyHasTag = project.ProjectTechnologyTags
                .Any(projectTag =>
                    string.Equals(
                        projectTag.TechnologyTag.Name,
                        tagName,
                        StringComparison.OrdinalIgnoreCase));

            if (projectAlreadyHasTag)
            {
                continue;
            }

            if (!tagsByName.TryGetValue(
                    tagName,
                    out var technologyTag))
            {
                technologyTag = new TechnologyTag
                {
                    Name = tagName
                };

                tagsByName.Add(tagName, technologyTag);
            }

            project.ProjectTechnologyTags.Add(
                new CandidateProjectTechnologyTag
                {
                    CandidateProject = project,
                    TechnologyTag = technologyTag
                });
        }

        project.Name = model.Name.Trim();
        project.StartDate = model.StartDate!.Value;
        project.EndDate = model.EndDate!.Value;
        project.Description = model.Description.Trim();

        var projectEntry = _context.Entry(project);

        projectEntry.Property(item => item.RowVersion)
            .OriginalValue = originalRowVersion;

        projectEntry.Property(item => item.Name)
            .IsModified = true;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            var databaseValues = await projectEntry
                .GetDatabaseValuesAsync(cancellationToken);

            if (databaseValues is null)
            {
                TempData["ProjectError"] =
                    "This project was deleted by another session.";

                return RedirectToAction(nameof(Index));
            }

            model.RowVersion = Convert.ToBase64String(
                databaseValues.GetValue<byte[]>(
                    nameof(CandidateProject.RowVersion)));

            ModelState.AddModelError(
                string.Empty,
                "This project was changed in another session. " +
                "Review your values and save again.");

            await PopulateFormContextAsync(
                model,
                profile,
                cancellationToken);

            return View("Create", model);
        }

        TempData["ProjectMessage"] =
            "Project updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profile = await _context.CandidateProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        if (profile is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var project = await _context.CandidateProjects
            .AsNoTracking()
            .Include(item => item.ProjectTechnologyTags)
            .ThenInclude(projectTag => projectTag.TechnologyTag)
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.CandidateProfileId == profile.Id,
                cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        var model = new CandidateProjectDeleteViewModel
        {
            Profile = CreateProfileDetails(profile),
            Id = project.Id,
            Name = project.Name,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            TechnologyTags = project.ProjectTechnologyTags
                .OrderBy(projectTag =>
                    projectTag.TechnologyTag.Name)
                .Select(projectTag =>
                    projectTag.TechnologyTag.Name)
                .ToArray(),
            RowVersion = Convert.ToBase64String(
                project.RowVersion)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        string? rowVersion,
        CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profileId = await _context.CandidateProfiles
            .AsNoTracking()
            .Where(profile => profile.UserId == userId)
            .Select(profile => (int?)profile.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (!profileId.HasValue)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var project = await _context.CandidateProjects
            .SingleOrDefaultAsync(
                item =>
                    item.Id == id &&
                    item.CandidateProfileId == profileId.Value,
                cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(rowVersion))
        {
            TempData["ProjectError"] =
                "The project version is missing. Reload and try again.";

            return RedirectToAction(nameof(Index));
        }

        byte[] originalRowVersion;

        try
        {
            originalRowVersion =
                Convert.FromBase64String(rowVersion);
        }
        catch (FormatException)
        {
            TempData["ProjectError"] =
                "The project version is invalid. Reload and try again.";

            return RedirectToAction(nameof(Index));
        }

        if (originalRowVersion.Length != 8)
        {
            TempData["ProjectError"] =
                "The project version is invalid. Reload and try again.";

            return RedirectToAction(nameof(Index));
        }

        _context.Entry(project)
            .Property(item => item.RowVersion)
            .OriginalValue = originalRowVersion;

        var cvProjectLinks = await _context.CandidateCvProjects
            .Where(link =>
                link.CandidateProjectId == project.Id)
            .ToArrayAsync(cancellationToken);

        _context.CandidateCvProjects.RemoveRange(
            cvProjectLinks);

        _context.CandidateProjects.Remove(project);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["ProjectError"] =
                "This project changed or was already deleted. " +
                "Review the latest list and try again.";

            return RedirectToAction(nameof(Index));
        }

        TempData["ProjectMessage"] =
            "Project deleted successfully.";

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateFormContextAsync(
        CandidateProjectFormViewModel model,
        CandidateProfile profile,
        CancellationToken cancellationToken)
    {
        model.Profile = CreateProfileDetails(profile);

        model.SuggestedTechnologyTags = await _context.TechnologyTags
            .AsNoTracking()
            .OrderBy(tag => tag.Name)
            .Select(tag => tag.Name)
            .Take(100)
            .ToArrayAsync(cancellationToken);
    }

    private static string[] ParseTechnologyTags(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Array.Empty<string>();
        }

        try
        {
            var items =
                JsonSerializer.Deserialize<TechnologyTagInput[]>(json)
                ?? Array.Empty<TechnologyTagInput>();

            return items
                .Select(item => item.Value?.Trim())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
        catch (JsonException)
        {
            return Array.Empty<string>();
        }
    }

    private string[] ValidateTechnologyTags(
        CandidateProjectFormViewModel model)
    {
        var technologyTagNames = ParseTechnologyTags(
            model.TechnologyTagsJson);

        if (technologyTagNames.Length == 0)
        {
            ModelState.AddModelError(
                nameof(model.TechnologyTagsJson),
                "Add at least one technology tag.");
        }
        else if (technologyTagNames.Length > 15)
        {
            ModelState.AddModelError(
                nameof(model.TechnologyTagsJson),
                "A project can have at most 15 technology tags.");
        }

        if (technologyTagNames.Any(tag => tag.Length > 50))
        {
            ModelState.AddModelError(
                nameof(model.TechnologyTagsJson),
                "A technology tag cannot exceed 50 characters.");
        }

        return technologyTagNames;
    }

    private static CandidateProfileDetailsViewModel CreateProfileDetails(
        CandidateProfile profile)
    {
        return new CandidateProfileDetailsViewModel
        {
            HasProfile = true,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Location = profile.Location,
            PersonalPhotoUrl = profile.PersonalPhotoUrl,
            CompletionPercentage = 100
        };
    }

    private sealed class TechnologyTagInput
    {
        [JsonPropertyName("value")]
        public string? Value { get; init; }
    }
}