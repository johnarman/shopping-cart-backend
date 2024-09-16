using ShoppingCart.Service.Interface;
using Common.DTOs;
using ShoppingCart.Entity.Model;
using Infrastructure.Repositories.Interfaces;

namespace ShoppingCart.Service.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IProductService _productService;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, IProductService productService)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _productService = productService;
        }

        public async Task<IEnumerable<CartDTO>> GetCartItemsAsync(int userId)
        {
            var cartItems = await _shoppingCartRepository.GetCartItemsByUserIdAsync(userId);
            var cartItemDtos = new List<CartDTO>();

            foreach (var item in cartItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);

                cartItemDtos.Add(new CartDTO
                {
                    UserId = item.UserId,
                    ProductId = item.ProductId,
                    ProductName = product?.Name,
                    Price = product?.Price ?? 0,
                    Quantity = item.Quantity
                });
            }

            return cartItemDtos;
        }

        public async Task<bool> AddOrUpdateItemInCartAsync(CartDTO cartItemDto)
        {
            var product = await _productService.GetProductByIdAsync(cartItemDto.ProductId);
            if (product == null)
            {
                throw new Exception($"Product with ID {cartItemDto.ProductId} does not exist.");
            }

            var existingCartItem = await _shoppingCartRepository.GetCartItemByUserAndProductIdAsync(cartItemDto.UserId, cartItemDto.ProductId);

            if (existingCartItem != null)
            {
                int newQuantity = existingCartItem.Quantity + cartItemDto.Quantity;
                int quantityDifference = newQuantity - existingCartItem.Quantity;

                if (quantityDifference > 0)
                {   
                    int availableStock = product.Stock - product.ReservedQuantity;

                    if (quantityDifference > availableStock)
                    {
                        throw new Exception($"Only {availableStock} units of {product.Name} are available.");
                    }

                    product.ReservedQuantity += quantityDifference;
                }
                else if (quantityDifference < 0)
                {
                    product.ReservedQuantity += quantityDifference;
                }

                existingCartItem.Quantity = newQuantity;
                existingCartItem.LastUpdatedAt = DateTime.Now;

                await _productService.UpdateProductAsync(product);
                return await _shoppingCartRepository.UpdateCartItemAsync(existingCartItem);
            }
            else
            {
                int availableStock = product.Stock - product.ReservedQuantity;
                if (cartItemDto.Quantity > availableStock)
                {
                    throw new Exception($"Only {availableStock} units of {product.Name} are available.");
                }

                product.ReservedQuantity += cartItemDto.Quantity;

                var newCartItem = new Cart
                {
                    UserId = cartItemDto.UserId,
                    ProductId = cartItemDto.ProductId,
                    Quantity = cartItemDto.Quantity,
                    LastUpdatedAt = DateTime.Now
                };

                await _productService.UpdateProductAsync(product);
                return await _shoppingCartRepository.AddItemToCartAsync(newCartItem);
            }
        }



        public async Task<bool> RemoveItemFromCartAsync(int userId, int productId)
        {   
            var cartItem = await _shoppingCartRepository.GetCartItemByUserAndProductIdAsync(userId, productId);
            if (cartItem == null)
            {
                return false;
            }
            await _productService.ReleaseProductStockAsync(cartItem.ProductId, cartItem.Quantity);

            return await _shoppingCartRepository.RemoveItemFromCartAsync(userId, productId);
        }

        public async Task<IEnumerable<Cart>> GetAbandonedCartItemsAsync(TimeSpan expirationThreshold)
        {
            return await _shoppingCartRepository.GetAbandonedCartItemsAsync(expirationThreshold);
        }

        public async Task<bool> UpdateCartItemQuantityAsync(CartDTO cartItemDto)
        {   
            var product = await _productService.GetProductByIdAsync(cartItemDto.ProductId);
            if (product == null)
            {
                throw new Exception($"Product with ID {cartItemDto.ProductId} does not exist.");
            }

            var existingCartItem = await _shoppingCartRepository.GetCartItemByUserAndProductIdAsync(cartItemDto.UserId, cartItemDto.ProductId);
            if (existingCartItem == null)
            {
                throw new Exception("Cart item does not exist.");
            }

            if (cartItemDto.Quantity <= 0 || (cartItemDto.Quantity == existingCartItem.Quantity))
            {
                await _productService.ReleaseProductStockAsync(cartItemDto.ProductId, cartItemDto.Quantity);

                return await _shoppingCartRepository.RemoveItemFromCartAsync(cartItemDto.UserId, cartItemDto.ProductId);
            }
            
            int quantityToRemove = existingCartItem.Quantity - cartItemDto.Quantity;
            
            product.ReservedQuantity -= quantityToRemove;
            existingCartItem.Quantity = cartItemDto.Quantity;
            existingCartItem.LastUpdatedAt = DateTime.Now;

            await _productService.UpdateProductAsync(product);
            return await _shoppingCartRepository.UpdateCartItemAsync(existingCartItem);
        }
    }
}
