using ShoppingCart.Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShoppingCart.Infrastructure.Data.Configurations
{
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(sc => sc.Id);

            builder.Property(sc => sc.UserId).IsRequired();
            builder.Property(sc => sc.ProductId).IsRequired();
            builder.Property(sc => sc.Quantity).IsRequired().HasDefaultValue(1);
            builder.Property(sc => sc.CreatedAt).HasDefaultValueSql("GETDATE()").IsRequired();
            builder.Property(sc => sc.LastUpdatedAt).HasDefaultValueSql("GETDATE()").IsRequired();

            builder.HasOne<Product>().WithMany().HasForeignKey(sc => sc.ProductId);
            builder.HasOne<User>().WithMany().HasForeignKey(sc => sc.UserId);
        }
    }
}
