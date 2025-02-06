using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        public DbSet<UserModel> Users { get; set; } 
        public DbSet<MessageModel> Messages { get; set; }
    }
}
