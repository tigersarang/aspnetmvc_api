using CommLibs.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtVueCrudApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Reply> Replies { get; set; }
    }
}
