using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Authorization;
using PerfectCareer.Web.Data;
using PerfectCareer.Web.Models.Profiles;
using PerfectCareer.Web.ViewModels.Profiles;
using PerfectCareer.Web.ViewModels.Projects;

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
}