using System.Collections.Generic;
using ShoppingCart.Entity.Model;

namespace ShoppingCart.Service.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync();
        Task<bool> UpdateProductAsync(Product product);
        Task<Product> GetProductByIdAsync(int productId);
        Task ReleaseProductStockAsync(int productId, int quantity);
    }
}
