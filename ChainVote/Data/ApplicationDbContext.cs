using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ChainVote.Models.Identity;
using ChainVote.Models.DatabaseEntities;
using Microsoft.AspNetCore.Identity;

namespace ChainVote.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets for your entities
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<AdminAccount> AdminAccount { get; set; }
        public DbSet<CandidatesData> CandidatesData { get; set; }
        public DbSet<EventsData> EventsData { get; set; }
        public DbSet<OrganizationsData> OrganizationsData { get; set; }
        public DbSet<OrganizationPosition> OrganizationPosition { get; set; }
        public DbSet<Votes> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Convert enum to string (Event status)
            modelBuilder.Entity<EventsData>()
                .Property(e => e.Status)
                .HasConversion<string>();

            // Indexes
            modelBuilder.Entity<Votes>()
                .HasIndex(v => v.CandidateId)
                .HasDatabaseName("IX_Votes_CandidateId");

            modelBuilder.Entity<Votes>()
                .HasIndex(v => v.EventId)
                .HasDatabaseName("IX_Votes_EventId");

            // Relationships

            // Event → Organizations
            modelBuilder.Entity<OrganizationsData>()
                .HasOne(o => o.Event)
                .WithMany(e => e.Organizations)
                .HasForeignKey(o => o.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization → Positions
            modelBuilder.Entity<OrganizationPosition>()
                .HasOne(p => p.Organization)
                .WithMany(o => o.Positions)
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict); // or .Cascade

            // OrganizationPosition → CandidatesData
            modelBuilder.Entity<CandidatesData>()
                .HasOne(c => c.Position)
                .WithMany(p => p.Candidates)
                .HasForeignKey(c => c.PositionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Candidate → ApplicationUser (One-to-One)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Candidate)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<CandidatesData>(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict); // prevent deletion if candidate exists
        }
    }
}
