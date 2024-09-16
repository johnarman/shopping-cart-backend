using ShoppingCart.Entity.Model;

namespace Common.Helper
{
    public interface IJwtHelper
    {
        string GenerateToken(User user);
    }
}
