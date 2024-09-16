using Common.DTOs;
using ShoppingCart.Entity.Model;

namespace ShoppingCart.Service.Interface
{
    public interface IShoppingCartService
    {   
        Task<IEnumerable<CartDTO>> GetCartItemsAsync(int userId);
        Task<bool> AddOrUpdateItemInCartAsync(CartDTO cartItemDto);
        Task<bool> RemoveItemFromCartAsync(int userId, int productId);
        Task<IEnumerable<Cart>> GetAbandonedCartItemsAsync(TimeSpan expirationThreshold);
        Task<bool> UpdateCartItemQuantityAsync(CartDTO cartItemDto);
    }
}
