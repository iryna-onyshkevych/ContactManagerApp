using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ContactManagerApp.Models
{
    public class ApplicationContext:DbContext
    {
        public DbSet<FileModel> Files { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }

    }
}
