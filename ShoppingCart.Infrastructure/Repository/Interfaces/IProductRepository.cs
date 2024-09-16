using System.Collections.Generic;
using ShoppingCart.Entity.Model;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task<bool> UpdateProductAsync(Product product);
    }
}
