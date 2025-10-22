using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<LecturerClaim> LecturerClaims { get; set; }
        public DbSet<ClaimDocument> ClaimDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply SQLite-friendly types for Identity
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                builder.Entity<IdentityRole>(b =>
                {
                    b.Property(r => r.Name).HasColumnType("TEXT");
                    b.Property(r => r.NormalizedName).HasColumnType("TEXT");
                    b.Property(r => r.ConcurrencyStamp).HasColumnType("TEXT");
                });

                builder.Entity<IdentityUser>(b =>
                {
                    b.Property(u => u.UserName).HasColumnType("TEXT");
                    b.Property(u => u.NormalizedUserName).HasColumnType("TEXT");
                    b.Property(u => u.Email).HasColumnType("TEXT");
                    b.Property(u => u.NormalizedEmail).HasColumnType("TEXT");
                    b.Property(u => u.PasswordHash).HasColumnType("TEXT");
                    b.Property(u => u.SecurityStamp).HasColumnType("TEXT");
                    b.Property(u => u.ConcurrencyStamp).HasColumnType("TEXT");
                });
            }
        }
    }
}