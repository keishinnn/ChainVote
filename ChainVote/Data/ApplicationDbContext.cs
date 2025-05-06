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

        // DbSets for application entities
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<AdminAccount> AdminAccount { get; set; }
        public DbSet<CandidatesData> CandidatesData { get; set; }
        public DbSet<EventsData> EventsData { get; set; }
        public DbSet<OrganizationsData> OrganizationsData { get; set; }

        // Configuring relationships and delete behaviors
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexing relationships for performance optimizations
            modelBuilder.Entity<Votes>()
                .HasIndex(v => v.CandidateId)
                .HasDatabaseName("IX_Votes_CandidateId");

            modelBuilder.Entity<Votes>()
                .HasIndex(v => v.EventId)
                .HasDatabaseName("IX_Votes_EventId");

            modelBuilder.Entity<PositionsData>()
                .HasIndex(p => p.EventId)
                .HasDatabaseName("IX_Positions_EventId");

            // Configuring Enum conversion for Event status (as string)
            modelBuilder.Entity<EventsData>()
                .Property(e => e.Status)
                .HasConversion<string>();

            modelBuilder.Entity<OrganizationsData>()
                .HasOne(o => o.Event)   // Each Organization has one Event
                .WithMany(e => e.Organizations)  // Each Event can have many Organizations
                .HasForeignKey(o => o.EventId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent deletion of Event if it has related Organizations

            modelBuilder.Entity<CandidatesData>()
                .HasOne(c => c.Organization)  // Each Candidate belongs to one Organization
                .WithMany(o => o.Candidates)  // Each Organization can have many Candidates
                .HasForeignKey(c => c.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent deletion of Organization if it has related Candidates

            // Cascade delete for Candidates when ApplicationUser is deleted
            modelBuilder.Entity<CandidatesData>()
                .HasOne(c => c.ApplicationUser)
                .WithMany()  // ApplicationUser has many Candidates, but we don't need to specify navigation on ApplicationUser side
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);  // Deletes Candidate when User is deleted

            // Prevent deletion of User if there are associated Candidates
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Candidates)  // A User can have many Candidates
                .WithOne(c => c.ApplicationUser)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent deletion of User if any Candidate exists

        }
    }
}
