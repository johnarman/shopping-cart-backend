using ShoppingCart.Service.Interface;
using Common.DTOs;
using Common.Helper;
using Infrastructure.Repositories.Interfaces;

namespace ShoppingCart.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, IJwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<string> AuthenticateAsync(UserLoginDTO loginDto)
        {
            var user = await _userRepository.GetUserAsync(loginDto.Username);
            

            if (user != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return _jwtHelper.GenerateToken(user);
            }


            return null;
        }
    }
}
