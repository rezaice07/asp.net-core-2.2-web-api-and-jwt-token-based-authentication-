using Microsoft.EntityFrameworkCore;
using System;
using University.Data.Models;

namespace University.Service
{
    public class UniversityDbContext : DbContext
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
            : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Class> Class { get; set; }
        public DbSet<StudentEnrolledClass> StudentEnrolledClass { get; set; }
    }    
}
