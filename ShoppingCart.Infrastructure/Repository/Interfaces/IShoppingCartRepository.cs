using ShoppingCart.Entity.Model;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IShoppingCartRepository
    {   
        Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(int userId);
        Task<Cart> GetCartItemByUserAndProductIdAsync(int userId, int productId);
        Task<bool> AddItemToCartAsync(Cart cartItem);
        Task<bool> UpdateCartItemAsync(Cart cartItem);
        Task<bool> RemoveItemFromCartAsync(int userId, int productId);
        Task<IEnumerable<Cart>> GetAbandonedCartItemsAsync(TimeSpan expirationThreshold);
    }
}
