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
            });
    }
}