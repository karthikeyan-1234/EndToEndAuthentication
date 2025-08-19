using AuthenticatedBackEndAPI.Models;

using Microsoft.EntityFrameworkCore;

namespace AuthenticatedBackEndAPI.Contexts
{
    public class EmployeeContext: DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        public EmployeeContext(DbContextOptions<EmployeeContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var Employees = modelBuilder.Entity<Employee>();
            Employees.HasKey(e => e.Id).IsClustered();

            Employees.Property(e => e.Name).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
