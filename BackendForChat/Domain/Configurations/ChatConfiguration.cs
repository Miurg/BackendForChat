using BackendForChat.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendForChat.Models.DatabaseContext
{
    public class ChatConfiguration : IEntityTypeConfiguration<ChatModel>
    {
        public void Configure(EntityTypeBuilder<ChatModel> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.HasOne(p => p.ChatType)
                .WithMany()
                .HasForeignKey(p => p.ChatTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Messages)
                .WithOne(p => p.Chat)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.ChatUsers)
                .WithOne(p => p.Chat)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
