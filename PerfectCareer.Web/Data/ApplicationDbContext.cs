using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Models.Attributes;
using PerfectCareer.Web.Models.Cvs;
using PerfectCareer.Web.Models.Positions;
using PerfectCareer.Web.Models.Profiles;
using PerfectCareer.Web.Models.Projects;

namespace PerfectCareer.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CandidateProfile> CandidateProfiles =>
        Set<CandidateProfile>();

    public DbSet<AttributeDefinition> AttributeDefinitions =>
        Set<AttributeDefinition>();

    public DbSet<AttributeOption> AttributeOptions =>
        Set<AttributeOption>();

    public DbSet<CandidateAttributeValue> CandidateAttributeValues =>
        Set<CandidateAttributeValue>();

    public DbSet<CandidateAttributeUsage> CandidateAttributeUsages =>
        Set<CandidateAttributeUsage>();

    public DbSet<CandidateProject> CandidateProjects =>
        Set<CandidateProject>();

    public DbSet<TechnologyTag> TechnologyTags =>
        Set<TechnologyTag>();

    public DbSet<CandidateProjectTechnologyTag> CandidateProjectTechnologyTags =>
        Set<CandidateProjectTechnologyTag>();

    public DbSet<Position> Positions =>
        Set<Position>();

    public DbSet<PositionTemplateAttribute> PositionTemplateAttributes =>
        Set<PositionTemplateAttribute>();

    public DbSet<PositionTechnologyTag> PositionTechnologyTags =>
        Set<PositionTechnologyTag>();

    public DbSet<CandidateCv> CandidateCvs =>
        Set<CandidateCv>();

    public DbSet<CandidateCvProject> CandidateCvProjects =>
        Set<CandidateCvProject>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CandidateProfile>(entity =>
        {
            entity.HasIndex(profile => profile.UserId)
                .IsUnique();

            entity.HasOne(profile => profile.User)
                .WithOne()
                .HasForeignKey<CandidateProfile>(
                    profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AttributeOption>(entity =>
        {
            entity.HasAlternateKey(option => new
            {
                option.AttributeDefinitionId,
                option.Id
            });

            entity.HasOne(option => option.AttributeDefinition)
                .WithMany()
                .HasForeignKey(
                    option => option.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CandidateAttributeValue>(entity =>
        {
            entity.HasOne(value => value.CandidateProfile)
                .WithMany()
                .HasForeignKey(
                    value => value.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(value => value.AttributeDefinition)
                .WithMany()
                .HasForeignKey(
                    value => value.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(value => value.SelectedOption)
                .WithMany()
                .HasForeignKey(value => new
                {
                    value.AttributeDefinitionId,
                    value.SelectedOptionId
                })
                .HasPrincipalKey(option => new
                {
                    option.AttributeDefinitionId,
                    option.Id
                })
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CandidateAttributeUsage>(entity =>
        {
            entity.HasKey(usage => new
            {
                usage.CandidateProfileId,
                usage.AttributeDefinitionId
            });

            entity.HasOne(usage => usage.CandidateProfile)
                .WithMany()
                .HasForeignKey(usage => usage.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(usage => usage.AttributeDefinition)
                .WithMany()
                .HasForeignKey(usage => usage.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CandidateProject>(entity =>
        {
            entity.HasOne(project => project.CandidateProfile)
                .WithMany(profile => profile.Projects)
                .HasForeignKey(project => project.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable(table =>
                table.HasCheckConstraint(
                    "CK_CandidateProjects_Period",
                    "[EndDate] >= [StartDate]"));
        });

        builder.Entity<CandidateProjectTechnologyTag>(entity =>
        {
            entity.HasKey(projectTag => new
            {
                projectTag.CandidateProjectId,
                projectTag.TechnologyTagId
            });

            entity.HasOne(projectTag => projectTag.CandidateProject)
                .WithMany(project => project.ProjectTechnologyTags)
                .HasForeignKey(projectTag =>
                    projectTag.CandidateProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(projectTag => projectTag.TechnologyTag)
                .WithMany(tag => tag.ProjectTechnologyTags)
                .HasForeignKey(projectTag =>
                    projectTag.TechnologyTagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Position>(entity =>
        {
            entity.HasIndex(position => new
            {
                position.IsActive,
                position.UpdatedAtUtc
            });
        });

        builder.Entity<PositionTemplateAttribute>(entity =>
        {
            entity.HasKey(templateAttribute => new
            {
                templateAttribute.PositionId,
                templateAttribute.AttributeDefinitionId
            });

            entity.HasIndex(templateAttribute => new
            {
                templateAttribute.PositionId,
                templateAttribute.DisplayOrder
            })
                .IsUnique();

            entity.HasOne(templateAttribute =>
                    templateAttribute.Position)
                .WithMany(position =>
                    position.TemplateAttributes)
                .HasForeignKey(templateAttribute =>
                    templateAttribute.PositionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(templateAttribute =>
                    templateAttribute.AttributeDefinition)
                .WithMany()
                .HasForeignKey(templateAttribute =>
                    templateAttribute.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PositionTechnologyTag>(entity =>
        {
            entity.HasKey(positionTag => new
            {
                positionTag.PositionId,
                positionTag.TechnologyTagId
            });

            entity.HasOne(positionTag => positionTag.Position)
                .WithMany(position =>
                    position.PositionTechnologyTags)
                .HasForeignKey(positionTag =>
                    positionTag.PositionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(positionTag =>
                    positionTag.TechnologyTag)
                .WithMany()
                .HasForeignKey(positionTag =>
                    positionTag.TechnologyTagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CandidateCv>(entity =>
        {
            entity.HasIndex(cv => new
            {
                cv.CandidateProfileId,
                cv.PositionId
            })
                .IsUnique();

            entity.HasIndex(cv => new
            {
                cv.Status,
                cv.UpdatedAtUtc
            });

            entity.HasOne(cv => cv.CandidateProfile)
                .WithMany()
                .HasForeignKey(cv =>
                    cv.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cv => cv.Position)
                .WithMany()
                .HasForeignKey(cv =>
                    cv.PositionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CandidateCvProject>(entity =>
        {
            entity.HasKey(cvProject => new
            {
                cvProject.CandidateCvId,
                cvProject.CandidateProjectId
            });

            entity.HasOne(cvProject => cvProject.CandidateCv)
                .WithMany(cv => cv.SelectedProjects)
                .HasForeignKey(cvProject =>
                    cvProject.CandidateCvId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cvProject =>
                    cvProject.CandidateProject)
                .WithMany()
                .HasForeignKey(cvProject =>
                    cvProject.CandidateProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AttributeDefinition>().HasData(
            new
            {
                Id = 1,
                Name = "First Name",
                Description = "Candidate's first name.",
                Category = AttributeCategory.PersonalInformation,
                DataType = AttributeDataType.Text,
                IsBuiltIn = true
            },
            new
            {
                Id = 2,
                Name = "Last Name",
                Description = "Candidate's last name.",
                Category = AttributeCategory.PersonalInformation,
                DataType = AttributeDataType.Text,
                IsBuiltIn = true
            },
            new
            {
                Id = 3,
                Name = "Location",
                Description = "Candidate's current location.",
                Category = AttributeCategory.PersonalInformation,
                DataType = AttributeDataType.Text,
                IsBuiltIn = true
            },
            new
            {
                Id = 4,
                Name = "Personal Photo",
                Description = "Candidate's personal photo.",
                Category = AttributeCategory.PersonalInformation,
                DataType = AttributeDataType.ExternalImage,
                IsBuiltIn = true
            },
            new
            {
                Id = 5,
                Name = "IELTS Score",
                Description = "Candidate's IELTS score.",
                Category = AttributeCategory.Certification,
                DataType = AttributeDataType.Number,
                IsBuiltIn = false
            },
            new
            {
                Id = 6,
                Name = "Presentation Skills",
                Description = "Candidate's presentation skill level.",
                Category = AttributeCategory.SoftSkills,
                DataType = AttributeDataType.SingleChoice,
                IsBuiltIn = false
            },
            new
            {
                Id = 7,
                Name = "Remote Work Availability",
                Description = "Whether the candidate is available for remote work.",
                Category = AttributeCategory.PersonalInformation,
                DataType = AttributeDataType.Boolean,
                IsBuiltIn = false
            },
            new
            {
                Id = 8,
                Name = "Professional Summary",
                Description = "A concise overview of the candidate's professional background and career goals.",
                Category = AttributeCategory.PersonalInformation,
                DataType = AttributeDataType.MarkdownText,
                IsBuiltIn = false
            },
            new
            {
                Id = 9,
                Name = "GitHub Profile",
                Description = "The candidate's GitHub profile URL or username.",
                Category = AttributeCategory.DomainKnowledge,
                DataType = AttributeDataType.Text,
                IsBuiltIn = false
            },
            new
            {
                Id = 10,
                Name = "Years of Experience",
                Description = "The candidate's total years of professional experience.",
                Category = AttributeCategory.DomainKnowledge,
                DataType = AttributeDataType.Number,
                IsBuiltIn = false
            },
            new
            {
                Id = 11,
                Name = "Available Start Date",
                Description = "The date when the candidate is available to start a new position.",
                Category = AttributeCategory.PersonalInformation,
                DataType = AttributeDataType.Date,
                IsBuiltIn = false
            },
            new
            {
                Id = 12,
                Name = "Relevant Experience Period",
                Description = "The start and end dates of the candidate's most relevant experience.",
                Category = AttributeCategory.DomainKnowledge,
                DataType = AttributeDataType.Period,
                IsBuiltIn = false
            },
            new
            {
                Id = 13,
                Name = "Open to Relocation",
                Description = "Whether the candidate is willing to relocate for a position.",
                Category = AttributeCategory.PersonalInformation,
                DataType = AttributeDataType.Boolean,
                IsBuiltIn = false
            },
            new
            {
                Id = 14,
                Name = "English Level",
                Description = "The candidate's overall English proficiency level.",
                Category = AttributeCategory.Certification,
                DataType = AttributeDataType.SingleChoice,
                IsBuiltIn = false
            });

        builder.Entity<AttributeOption>().HasData(
            new
            {
                Id = 1,
                AttributeDefinitionId = 6,
                Label = "Beginner"
            },
            new
            {
                Id = 2,
                AttributeDefinitionId = 6,
                Label = "Intermediate"
            },
            new
            {
                Id = 3,
                AttributeDefinitionId = 6,
                Label = "Advanced"
            },
            new
            {
                Id = 4,
                AttributeDefinitionId = 14,
                Label = "Beginner"
            },
            new
            {
                Id = 5,
                AttributeDefinitionId = 14,
                Label = "Intermediate"
            },
            new
            {
                Id = 6,
                AttributeDefinitionId = 14,
                Label = "Upper-Intermediate"
            },
            new
            {
                Id = 7,
                AttributeDefinitionId = 14,
                Label = "Advanced"
            },
            new
            {
                Id = 8,
                AttributeDefinitionId = 14,
                Label = "Fluent"
            });

        builder.Entity<Position>().HasData(
            new
            {
                Id = 1,
                Title = "Junior ASP.NET Core Developer",
                Location = "Riyadh, Saudi Arabia",
                EmploymentType = "Full-time",
                Description = "Build and maintain modern web applications using ASP.NET Core and Entity Framework Core.",
                IsActive = true,
                CreatedAtUtc = new DateTimeOffset(
                    2026, 9, 20, 8, 0, 0,
                    TimeSpan.Zero),
                UpdatedAtUtc = new DateTimeOffset(
                    2026, 9, 20, 8, 0, 0,
                    TimeSpan.Zero)
            },
            new
            {
                Id = 2,
                Title = "Frontend Developer",
                Location = "Remote",
                EmploymentType = "Full-time",
                Description = "Create responsive, accessible user interfaces for the Perfect Career platform.",
                IsActive = true,
                CreatedAtUtc = new DateTimeOffset(
                    2026, 9, 20, 9, 0, 0,
                    TimeSpan.Zero),
                UpdatedAtUtc = new DateTimeOffset(
                    2026, 9, 20, 9, 0, 0,
                    TimeSpan.Zero)
            });

        builder.Entity<PositionTemplateAttribute>().HasData(
            new
            {
                PositionId = 1,
                AttributeDefinitionId = 8,
                DisplayOrder = 1
            },
            new
            {
                PositionId = 1,
                AttributeDefinitionId = 9,
                DisplayOrder = 2
            },
            new
            {
                PositionId = 1,
                AttributeDefinitionId = 10,
                DisplayOrder = 3
            },
            new
            {
                PositionId = 1,
                AttributeDefinitionId = 14,
                DisplayOrder = 4
            },
            new
            {
                PositionId = 2,
                AttributeDefinitionId = 8,
                DisplayOrder = 1
            },
            new
            {
                PositionId = 2,
                AttributeDefinitionId = 9,
                DisplayOrder = 2
            },
            new
            {
                PositionId = 2,
                AttributeDefinitionId = 14,
                DisplayOrder = 3
            });
    }
}