using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Authorization;
using PerfectCareer.Web.Data;
using PerfectCareer.Web.Models.Profiles;
using PerfectCareer.Web.ViewModels.Profiles;

namespace PerfectCareer.Web.Controllers;

[Authorize(Roles = AppRoles.Candidate)]
public sealed class CandidateProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CandidateProfileController(
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

        var model = new CandidateProfileDetailsViewModel
        {
            HasProfile = profile is not null,
            FirstName = profile?.FirstName ?? string.Empty,
            LastName = profile?.LastName ?? string.Empty,
            Location = profile?.Location ?? string.Empty,
            PersonalPhotoUrl = profile?.PersonalPhotoUrl ?? string.Empty,
            CompletionPercentage = profile is null ? 0 : 100
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
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

        var model = profile is null
            ? new CandidateProfileEditViewModel()
            : new CandidateProfileEditViewModel
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Location = profile.Location,
                PersonalPhotoUrl = profile.PersonalPhotoUrl,
                RowVersion = profile.RowVersion
            };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        CandidateProfileEditViewModel model,
        CancellationToken cancellationToken)
    {
        ValidatePhotoUrl(model.PersonalPhotoUrl);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var profile = await _context.CandidateProfiles
            .SingleOrDefaultAsync(
                item => item.UserId == userId,
                cancellationToken);

        if (profile is null)
        {
            profile = new CandidateProfile
            {
                UserId = userId,
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                Location = model.Location.Trim(),
                PersonalPhotoUrl = model.PersonalPhotoUrl.Trim()
            };

            _context.CandidateProfiles.Add(profile);
        }
        else
        {
            if (model.RowVersion is null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The profile version is missing. Reload the page and try again.");

                return View(model);
            }

            _context.Entry(profile)
                .Property(item => item.RowVersion)
                .OriginalValue = model.RowVersion;

            profile.FirstName = model.FirstName.Trim();
            profile.LastName = model.LastName.Trim();
            profile.Location = model.Location.Trim();
            profile.PersonalPhotoUrl = model.PersonalPhotoUrl.Trim();
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentProfile = await _context.CandidateProfiles
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item => item.UserId == userId,
                    cancellationToken);

            if (currentProfile is null)
            {
                model.RowVersion = null;

                ModelState.AddModelError(
                    string.Empty,
                    "The profile was deleted in another session. Submit again to recreate it.");
            }
            else
            {
                model.RowVersion = currentProfile.RowVersion;

                ModelState.AddModelError(
                    string.Empty,
                    "The profile was changed in another session. Review your information and submit again.");
            }

            return View(model);
        }

        TempData["ProfileMessage"] = "Profile saved successfully.";

        return RedirectToAction(nameof(Index));
    }

    private void ValidatePhotoUrl(string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return;
        }

        var isValid = Uri.TryCreate(
            photoUrl,
            UriKind.Absolute,
            out var uri);

        var isHttpUrl = isValid &&
            (uri!.Scheme == Uri.UriSchemeHttps ||
             uri.Scheme == Uri.UriSchemeHttp);

        if (!isHttpUrl)
        {
            ModelState.AddModelError(
                nameof(CandidateProfileEditViewModel.PersonalPhotoUrl),
                "Personal photo must use a valid HTTP or HTTPS URL.");
        }
    }
}