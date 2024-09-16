using System.Threading.Tasks;
using ShoppingCart.Entity.Model;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(string username);
    }
}
