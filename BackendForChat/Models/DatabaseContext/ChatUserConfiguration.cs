using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendForChat.Models.DatabaseContext
{
    public class ChatUserConfiguration : IEntityTypeConfiguration<ChatUserModel>
    {
        public void Configure(EntityTypeBuilder<ChatUserModel> builder)
        {
            builder.HasKey(p => new { p.UserId, p.ChatId });

            builder.HasOne(p => p.User)
                .WithMany(p => p.ChatUsers)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Chat)
                .WithMany(p => p.ChatUsers)
                .HasForeignKey(p => p.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }    
    }
}
