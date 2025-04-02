using BackendForChat.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendForChat.Models.DatabaseContext
{
    public class UserConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Username)
                 .IsRequired()
                 .HasMaxLength(32);
            builder.Property(p => p.PasswordHash)
                .IsRequired();
            builder.HasMany(p => p.ChatUsers)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(p => p.SentMessages)
                .WithOne(p => p.Sender)
                .HasForeignKey(p => p.SenderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
