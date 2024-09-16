using ShoppingCart.Entity.Model;
using ShoppingCart.Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Infrastructure.Repositories.Implementations
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ShoppingCartContext _context;

        public ShoppingCartRepository(ShoppingCartContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(int userId)
        {
            return await _context.ShoppingCarts
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Cart> GetCartItemByUserAndProductIdAsync(int userId, int productId)
        {
            return await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }

        public async Task<bool> AddItemToCartAsync(Cart cartItem)
        {
            _context.ShoppingCarts.Add(cartItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCartItemAsync(Cart cartItem)
        {
            _context.ShoppingCarts.Update(cartItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveItemFromCartAsync(int userId, int productId)
        {
            var cartItem = await GetCartItemByUserAndProductIdAsync(userId, productId);
            if (cartItem != null)
            {
                _context.ShoppingCarts.Remove(cartItem);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<Cart>> GetAbandonedCartItemsAsync(TimeSpan expirationThreshold)
        {
            var expirationDate = DateTime.Now.Subtract(expirationThreshold);

            return await _context.ShoppingCarts
                .Where(c => c.LastUpdatedAt < expirationDate)
                .ToListAsync();
        }
    }
}
