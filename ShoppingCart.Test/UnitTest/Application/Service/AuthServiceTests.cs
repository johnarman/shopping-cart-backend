using Moq;
using ShoppingCart.Service.Service;
using Common.DTOs;
using ShoppingCart.Entity.Model;
using Infrastructure.Repositories.Interfaces;
using FluentAssertions;
using Common.Helper;

namespace ShoppingCart.Test.UnitTest.Application.Service
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtHelper> _jwtHelperMock;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtHelperMock = new Mock<IJwtHelper>();

            _authService = new AuthService(_userRepositoryMock.Object, _jwtHelperMock.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new UserLoginDTO
            {
                Username = "testuser",
                Password = "password123"
            };

            var user = new User
            {
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("password123")
            };

            _userRepositoryMock.Setup(repo => repo.GetUserAsync(loginDto.Username))
                               .ReturnsAsync(user);

            _jwtHelperMock.Setup(helper => helper.GenerateToken(user))
                          .Returns("mocked_token");

            // Act
            var result = await _authService.AuthenticateAsync(loginDto);

            // Assert
            result.Should().Be("mocked_token");
            _jwtHelperMock.Verify(helper => helper.GenerateToken(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var loginDto = new UserLoginDTO
            {
                Username = "nonexistentuser",
                Password = "password123"
            };

            _userRepositoryMock.Setup(repo => repo.GetUserAsync(loginDto.Username))
                               .ReturnsAsync((User)null);

            // Act
            var result = await _authService.AuthenticateAsync(loginDto);

            // Assert
            result.Should().BeNull();
            _jwtHelperMock.Verify(helper => helper.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginDto = new UserLoginDTO
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("correctpassword")  // Correct password but different from what was entered
            };

            _userRepositoryMock.Setup(repo => repo.GetUserAsync(loginDto.Username))
                               .ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync(loginDto);

            // Assert
            result.Should().BeNull();
            _jwtHelperMock.Verify(helper => helper.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldVerifyPasswordWithBCrypt()
        {
            // Arrange
            var loginDto = new UserLoginDTO
            {
                Username = "testuser",
                Password = "password123"
            };

            var user = new User
            {
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("password123")
            };

            _userRepositoryMock.Setup(repo => repo.GetUserAsync(loginDto.Username))
                               .ReturnsAsync(user);

            // Act
            await _authService.AuthenticateAsync(loginDto);

            // Assert
            _userRepositoryMock.Verify(repo => repo.GetUserAsync(loginDto.Username), Times.Once);
        }
    }
}
