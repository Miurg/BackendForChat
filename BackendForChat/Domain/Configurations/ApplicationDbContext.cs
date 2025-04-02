using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System;
using System.Reflection;
using BackendForChat.Models.Entities;

namespace BackendForChat.Models.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ChatUserModel> ChatUsers { get; set; }
        public DbSet<ChatModel> Chats { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ChatTypeModel> ChatTypes { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Подключаем все конфигурации из сборки
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
