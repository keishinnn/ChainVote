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

            // Organization → Event
            modelBuilder.Entity<OrganizationsData>()
                .HasOne(o => o.Event)
                .WithMany(e => e.Organizations)
                .HasForeignKey(o => o.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization → Candidates
            modelBuilder.Entity<CandidatesData>()
                .HasOne(c => c.Organization)
                .WithMany(o => o.Candidates)
                .HasForeignKey(c => c.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization → Positions
            modelBuilder.Entity<OrganizationPosition>()
                .HasOne(p => p.Organization)
                .WithMany(o => o.Positions)
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Position → Candidate (optional)
            modelBuilder.Entity<OrganizationPosition>()
                .HasOne(p => p.Candidate)
                .WithMany(c => c.Positions)
                .HasForeignKey(p => p.CandidateId)
                .OnDelete(DeleteBehavior.SetNull);

            // Candidate → User
            modelBuilder.Entity<CandidatesData>()
                .HasOne(c => c.ApplicationUser)
                .WithMany()
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prevent deletion of User if any Candidate still references it
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Candidates)
                .WithOne(c => c.ApplicationUser)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
