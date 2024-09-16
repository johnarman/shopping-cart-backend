using ShoppingCart.Service.Interface;
using ShoppingCart.Entity.Model;
using Infrastructure.Repositories.Interfaces;

namespace ShoppingCart.Service.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            });
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            return await _productRepository.UpdateProductAsync(product);
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _productRepository.GetProductByIdAsync(productId);
        }

        public async Task ReleaseProductStockAsync(int productId, int quantity)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product != null)
            {
                if (quantity <= 0)
                    product.ReservedQuantity = 0;
                else
                    product.ReservedQuantity -= quantity;

                await _productRepository.UpdateProductAsync(product);
            }
        }
    }
}
