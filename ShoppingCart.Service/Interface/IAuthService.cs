using System.Threading.Tasks;
using Common.DTOs;

namespace ShoppingCart.Service.Interface
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(UserLoginDTO loginDto);
    }
}
