using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendForChat.Models.DatabaseContext
{
    public class MessageConfiguration : IEntityTypeConfiguration<MessageModel>
    {
        public void Configure(EntityTypeBuilder<MessageModel> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Chat)
                .WithMany(p =>  p.Messages)
                .HasForeignKey(p => p.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(p => p.ChatId)
                .IsRequired();

            builder.HasOne(p => p.Sender)
                .WithMany(p => p.SentMessages)
                .HasForeignKey(p => p.SenderId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(p => p.SenderId)
                .IsRequired();

            builder.Property(p =>p.Content)
                .IsRequired()
                .HasMaxLength(4096);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
