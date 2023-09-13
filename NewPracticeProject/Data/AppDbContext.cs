using Microsoft.EntityFrameworkCore;
using NewPracticeProject.Models;

namespace NewPracticeProject.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet<StudentRegister> studentRegisters  { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<Audit> Audits { get; set; }



        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseOracle("OracleConnection");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define primary key for the Employee entity
            modelBuilder.Entity<Employee>().HasKey(e => e.Id);



            // Configure the one-to-many relationship between Department and Employee
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId);

            // Other model configurations...

            base.OnModelCreating(modelBuilder);
        }
    }
}
