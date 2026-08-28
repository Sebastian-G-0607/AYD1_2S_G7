using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class ItemEntityConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.Property(item => item.Name)
               .HasMaxLength(50);

        builder.Property(item => item.Description)
               .HasMaxLength(500);

        builder.Property(item => item.Price)
               .HasPrecision(5, 2);
    }
}
