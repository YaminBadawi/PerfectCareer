using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PerfectCareer.Web.Models.Attributes;
using PerfectCareer.Web.Models.Profiles;

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
    }
}