using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendForChat.Models.DatabaseContext
{
    public class ChatTypeConfiguration : IEntityTypeConfiguration<ChatTypeModel>
    {
        public void Configure(EntityTypeBuilder<ChatTypeModel> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
