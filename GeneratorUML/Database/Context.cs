using GeneratorUML.Database.Models;
using Microsoft.EntityFrameworkCore;
namespace GeneratorUML.Database
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Post> Books { get; set; }
    }
}