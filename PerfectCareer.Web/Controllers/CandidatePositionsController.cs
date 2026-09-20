using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Authorization;
using PerfectCareer.Web.Data;
using PerfectCareer.Web.Models.Cvs;
using PerfectCareer.Web.Models.Profiles;
using PerfectCareer.Web.ViewModels.Positions;

namespace PerfectCareer.Web.Controllers;

[Authorize(Roles = AppRoles.Candidate)]
public sealed class CandidatePositionsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CandidatePositionsController(
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

        var candidateProfileId =
            await _context.CandidateProfiles
                .AsNoTracking()
                .Where(profile =>
                    profile.UserId == userId)
                .Select(profile =>
                    (int?)profile.Id)
                .SingleOrDefaultAsync(
                    cancellationToken);

        if (candidateProfileId is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var candidateCvs =
            await _context.CandidateCvs
                .AsNoTracking()
                .Where(cv =>
                    cv.CandidateProfileId ==
                    candidateProfileId.Value)
                .Select(cv => new
                {
                    cv.Id,
                    cv.PositionId,
                    cv.Status
                })
                .ToDictionaryAsync(
                    cv => cv.PositionId,
                    cancellationToken);

        var positions =
            await _context.Positions
                .AsNoTracking()
                .Where(position =>
                    position.IsActive)
                .OrderByDescending(position =>
                    position.UpdatedAtUtc)
                .ThenBy(position =>
                    position.Title)
                .Select(position =>
                    new CandidatePositionListItemViewModel
                    {
                        Id = position.Id,
                        Title = position.Title,
                        Location = position.Location,

                        EmploymentType =
                            position.EmploymentType,

                        Description =
                            position.Description,

                        TechnologyTags =
                            position.PositionTechnologyTags
                                .OrderBy(positionTag =>
                                    positionTag
                                        .TechnologyTag
                                        .Name)
                                .Select(positionTag =>
                                    positionTag
                                        .TechnologyTag
                                        .Name)
                                .ToArray(),

                        RequiredAttributes =
                            position.TemplateAttributes
                                .OrderBy(templateAttribute =>
                                    templateAttribute
                                        .DisplayOrder)
                                .Select(templateAttribute =>
                                    templateAttribute
                                        .AttributeDefinition
                                        .Name)
                                .ToArray()
                    })
                .ToArrayAsync(
                    cancellationToken);

        foreach (var position in positions)
        {
            if (!candidateCvs.TryGetValue(
                    position.Id,
                    out var candidateCv))
            {
                continue;
            }

            position.CandidateCvId =
                candidateCv.Id;

            position.CvStatus =
                candidateCv.Status;
        }

        var model =
            new CandidatePositionsViewModel
            {
                Positions = positions
            };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(
        int id,
        CancellationToken cancellationToken)
    {
        var userId =
            _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var candidateProfileId =
            await _context.CandidateProfiles
                .AsNoTracking()
                .Where(profile =>
                    profile.UserId == userId)
                .Select(profile =>
                    (int?)profile.Id)
                .SingleOrDefaultAsync(
                    cancellationToken);

        if (candidateProfileId is null)
        {
            return RedirectToAction(
                "Edit",
                "CandidateProfile");
        }

        var positionExists =
            await _context.Positions
                .AsNoTracking()
                .AnyAsync(
                    position =>
                        position.Id == id &&
                        position.IsActive,
                    cancellationToken);

        if (!positionExists)
        {
            return NotFound();
        }

        var requiredAttributeIds =
            await _context.PositionTemplateAttributes
                .AsNoTracking()
                .Where(templateAttribute =>
                    templateAttribute.PositionId == id &&
                    !templateAttribute
                        .AttributeDefinition
                        .IsBuiltIn)
                .Select(templateAttribute =>
                    templateAttribute
                        .AttributeDefinitionId)
                .ToArrayAsync(
                    cancellationToken);

        var existingAttributeIds =
            await _context.CandidateAttributeValues
                .AsNoTracking()
                .Where(value =>
                    value.CandidateProfileId ==
                        candidateProfileId.Value &&
                    requiredAttributeIds.Contains(
                        value.AttributeDefinitionId))
                .Select(value =>
                    value.AttributeDefinitionId)
                .ToArrayAsync(
                    cancellationToken);

        var missingAttributeValues =
            requiredAttributeIds
                .Except(existingAttributeIds)
                .Select(attributeDefinitionId =>
                    new CandidateAttributeValue
                    {
                        CandidateProfileId =
                            candidateProfileId.Value,

                        AttributeDefinitionId =
                            attributeDefinitionId
                    })
                .ToArray();

        _context.CandidateAttributeValues
            .AddRange(missingAttributeValues);

        var existingCvId =
            await _context.CandidateCvs
                .AsNoTracking()
                .Where(cv =>
                    cv.CandidateProfileId ==
                        candidateProfileId.Value &&
                    cv.PositionId == id)
                .Select(cv =>
                    (int?)cv.Id)
                .SingleOrDefaultAsync(
                    cancellationToken);

        if (existingCvId is not null)
        {
            if (missingAttributeValues.Length > 0)
            {
                try
                {
                    await _context.SaveChangesAsync(
                        cancellationToken);
                }
                catch (DbUpdateException exception) when (
                    exception.InnerException is
                        SqlException
                    {
                        Number: 2601 or 2627
                    })
                {
                    // Another request added the same values first.
                }
            }

            return RedirectToAction(
                "Edit",
                "CandidateCvs",
                new
                {
                    id = existingCvId.Value
                });
        }

        var now =
            DateTimeOffset.UtcNow;

        var candidateCv =
            new CandidateCv
            {
                CandidateProfileId =
                    candidateProfileId.Value,

                PositionId = id,
                Status =
                    CandidateCvStatus.Draft,

                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

        _context.CandidateCvs.Add(
            candidateCv);

        try
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is
                SqlException
            {
                Number: 2601 or 2627
            })
        {
            existingCvId =
                await _context.CandidateCvs
                    .AsNoTracking()
                    .Where(cv =>
                        cv.CandidateProfileId ==
                            candidateProfileId.Value &&
                        cv.PositionId == id)
                    .Select(cv =>
                        (int?)cv.Id)
                    .SingleOrDefaultAsync(
                        cancellationToken);

            if (existingCvId is null)
            {
                throw;
            }

            return RedirectToAction(
                "Edit",
                "CandidateCvs",
                new
                {
                    id = existingCvId.Value
                });
        }

        return RedirectToAction(
            "Edit",
            "CandidateCvs",
            new
            {
                id = candidateCv.Id
            });
    }
}