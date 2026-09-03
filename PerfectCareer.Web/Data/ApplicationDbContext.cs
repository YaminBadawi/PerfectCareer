using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CandidateProfile>(entity =>
        {
            entity.HasIndex(profile => profile.UserId)
                .IsUnique();

            entity.HasOne(profile => profile.User)
                .WithOne()
                .HasForeignKey<CandidateProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}