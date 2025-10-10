using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<JobVendor> JobVendors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Many-to-many: User <-> Role using implicit join table "UserRoles"
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("UserRoles"));

            // ✅ Many-to-many: Job <-> Vendor via JobVendor
            modelBuilder.Entity<JobVendor>()
                .HasKey(jv => new { jv.JobId, jv.VendorId });

            modelBuilder.Entity<JobVendor>()
                .HasOne(jv => jv.Job)
                .WithMany(j => j.JobVendors)
                .HasForeignKey(jv => jv.JobId);

            modelBuilder.Entity<JobVendor>()
                .HasOne(jv => jv.Vendor)
                .WithMany(v => v.JobVendors)
                .HasForeignKey(jv => jv.VendorId);

            // ✅ Soft delete filters with admin bypass
           /* modelBuilder.Entity<User>().HasQueryFilter(u =>
                u.IsActive || u.Email == "admin@example.com"
            );*/    

            modelBuilder.Entity<Vendor>().HasQueryFilter(v => v.IsActive);
            modelBuilder.Entity<Job>().HasQueryFilter(j => j.IsActive);
            modelBuilder.Entity<Employee>().HasQueryFilter(e => e.IsActive);
        }
    }
}
