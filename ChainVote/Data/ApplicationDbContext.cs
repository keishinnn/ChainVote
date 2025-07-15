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

        // DbSets for entities
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<AdminAccount> AdminAccount { get; set; }
        public DbSet<CandidatesData> CandidatesData { get; set; }
        public DbSet<EventsData> EventsData { get; set; }
        public DbSet<OrganizationsData> OrganizationsData { get; set; }
        public DbSet<OrganizationPosition> OrganizationPosition { get; set; }
        public DbSet<VoteRecord> VoteRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Convert enum to string (Event status)
            modelBuilder.Entity<EventsData>()
                .Property(e => e.Status)
                .HasConversion<string>();

            // Indexes
            modelBuilder.Entity<VoteRecord>()
                .HasIndex(v => v.CandidateId)
                .HasDatabaseName("IX_Votes_CandidateId");

            modelBuilder.Entity<VoteRecord>()
                .HasIndex(v => v.EventId)
                .HasDatabaseName("IX_Votes_EventId");

            // Relationships

            // Event → Organizations
            modelBuilder.Entity<OrganizationsData>()
                .HasOne(o => o.Event)
                .WithMany(e => e.Organizations)
                .HasForeignKey(o => o.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Organization → Positions
            modelBuilder.Entity<OrganizationPosition>()
                .HasOne(p => p.Organization)
                .WithMany(o => o.Positions)
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade); // or .Cascade

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


            // VoteRecord → Voter (ApplicationUser) (M:1)
            modelBuilder.Entity<VoteRecord>()
                .HasOne(v => v.Voter)
                .WithMany()
                .HasForeignKey(v => v.VoterId)
                .OnDelete(DeleteBehavior.Restrict);

            // VoteRecord → Candidate (M:1)
            modelBuilder.Entity<VoteRecord>()
                .HasOne(v => v.Candidate)
                .WithMany(c => c.VoteRecords)
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            // VoteRecord → Event (M:1)
            modelBuilder.Entity<VoteRecord>()
                .HasOne(v => v.Event)
                .WithMany(e => e.VoteRecords)
                .HasForeignKey(v => v.EventId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
