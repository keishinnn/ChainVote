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
        public DbSet<CandidatesData> CandidatesData { get; set; }
        public DbSet<EventsData> EventsData { get; set; }
        public DbSet<OrganizationsData> OrganizationsData { get; set; }
        public DbSet<Voters> Voters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Votes>()
                .HasIndex(v => v.CandidateId)
                .HasDatabaseName("IX_Votes_CandidateId");

            modelBuilder.Entity<Votes>()
                .HasIndex(v => v.EventId)
                .HasDatabaseName("IX_Votes_EventId");

            modelBuilder.Entity<PositionsData>()
                .HasIndex(p => p.EventId)
                .HasDatabaseName("IX_Positions_EventId");
        }

    }
}
