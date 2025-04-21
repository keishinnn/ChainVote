using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ChainVote.Models.Identity;
using ChainVote.Models.DatabaseEntities;
using Microsoft.AspNetCore.Identity;

namespace ChainVote.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<AdminAccount> AdminAccount { get; set; }
        public DbSet<CandidateData> CandidatesData { get; set; }
        public DbSet<EventData> EventsData { get; set; }
        public DbSet<OrganizationData> OrganizationsData { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.StudentId)
                .IsUnique()
                .HasDatabaseName("IX_ApplicationUser_StudentId");

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_ApplicationUser_Email");
        }

    }
}
