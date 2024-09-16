using ShoppingCart.Service.Interface;
using ShoppingCart.Service.Service;
using Common.DTOs;
using ShoppingCart.Entity.Model;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Moq;

namespace ShoppingCart.Test.UnitTest.Application.Service
{
    public class ShoppingCartServiceTests
    {
        private readonly ShoppingCartService _shoppingCartService;
        private readonly Mock<IShoppingCartRepository> _shoppingCartRepositoryMock;
        private readonly Mock<IProductService> _productServiceMock;

        public ShoppingCartServiceTests()
        {
            _shoppingCartRepositoryMock = new Mock<IShoppingCartRepository>();
            _productServiceMock = new Mock<IProductService>();
            _shoppingCartService = new ShoppingCartService(
                _shoppingCartRepositoryMock.Object,
                _productServiceMock.Object
            );
        }

        [Fact]
        public async Task GetCartItemsAsync_ShouldReturnCartItems_WhenUserHasItemsInCart()
        {
            // Arrange
            var cartItems = new List<Cart>
        {
            new Cart { UserId = 1, ProductId = 1, Quantity = 2 },
            new Cart { UserId = 1, ProductId = 2, Quantity = 1 }
        };
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 9.99m },
            new Product { Id = 2, Name = "Product 2", Price = 19.99m }
        };

            _shoppingCartRepositoryMock.Setup(repo => repo.GetCartItemsByUserIdAsync(1))
                .ReturnsAsync(cartItems);

            _productServiceMock.Setup(service => service.GetProductByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => products.Find(p => p.Id == id));

            // Act
            var result = await _shoppingCartService.GetCartItemsAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(item => item.ProductId == 1 && item.ProductName == "Product 1" && item.Quantity == 2);
            result.Should().Contain(item => item.ProductId == 2 && item.ProductName == "Product 2" && item.Quantity == 1);
        }

        [Fact]
        public async Task AddOrUpdateItemInCartAsync_ShouldAddNewItem_WhenItemDoesNotExistInCart()
        {
            // Arrange
            var cartItemDto = new CartDTO
            {
                UserId = 1,
                ProductId = 1,
                Quantity = 2
            };
            var product = new Product { Id = 1, Name = "Product 1", Stock = 10, ReservedQuantity = 0 };

            _productServiceMock.Setup(service => service.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _shoppingCartRepositoryMock.Setup(repo => repo.GetCartItemByUserAndProductIdAsync(1, 1))
                .ReturnsAsync((Cart)null);

            _shoppingCartRepositoryMock.Setup(repo => repo.AddItemToCartAsync(It.IsAny<Cart>()))
                .ReturnsAsync(true);

            // Act
            var result = await _shoppingCartService.AddOrUpdateItemInCartAsync(cartItemDto);

            // Assert
            result.Should().BeTrue();
            _shoppingCartRepositoryMock.Verify(repo => repo.AddItemToCartAsync(It.IsAny<Cart>()), Times.Once);
            _productServiceMock.Verify(service => service.UpdateProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateItemInCartAsync_ShouldUpdateExistingItem_WhenItemExistsInCart()
        {
            // Arrange
            var cartItemDto = new CartDTO { UserId = 1, ProductId = 1, Quantity = 2 };
            var existingCartItem = new Cart { UserId = 1, ProductId = 1, Quantity = 1 };
            var product = new Product { Id = 1, Name = "Product 1", Stock = 10, ReservedQuantity = 0 };

            _productServiceMock.Setup(service => service.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _shoppingCartRepositoryMock.Setup(repo => repo.GetCartItemByUserAndProductIdAsync(1, 1))
                .ReturnsAsync(existingCartItem);

            _shoppingCartRepositoryMock.Setup(repo => repo.UpdateCartItemAsync(existingCartItem))
                .ReturnsAsync(true);

            // Act
            var result = await _shoppingCartService.AddOrUpdateItemInCartAsync(cartItemDto);

            // Assert
            result.Should().BeTrue();
            existingCartItem.Quantity.Should().Be(3);
            _shoppingCartRepositoryMock.Verify(repo => repo.UpdateCartItemAsync(existingCartItem), Times.Once);
            _productServiceMock.Verify(service => service.UpdateProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task RemoveItemFromCartAsync_ShouldReturnTrue_WhenItemIsRemoved()
        {
            // Arrange
            var cartItem = new Cart { UserId = 1, ProductId = 1, Quantity = 2 };

            _shoppingCartRepositoryMock.Setup(repo => repo.GetCartItemByUserAndProductIdAsync(1, 1))
                .ReturnsAsync(cartItem);

            _shoppingCartRepositoryMock.Setup(repo => repo.RemoveItemFromCartAsync(1, 1))
                .ReturnsAsync(true);

            // Act
            var result = await _shoppingCartService.RemoveItemFromCartAsync(1, 1);

            // Assert
            result.Should().BeTrue();
            _shoppingCartRepositoryMock.Verify(repo => repo.RemoveItemFromCartAsync(1, 1), Times.Once);
            _productServiceMock.Verify(service => service.ReleaseProductStockAsync(1, 2), Times.Once);
        }

        [Fact]
        public async Task RemoveItemFromCartAsync_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            // Arrange
            _shoppingCartRepositoryMock.Setup(repo => repo.GetCartItemByUserAndProductIdAsync(1, 1))
                .ReturnsAsync((Cart)null);

            // Act
            var result = await _shoppingCartService.RemoveItemFromCartAsync(1, 1);

            // Assert
            result.Should().BeFalse();
            _shoppingCartRepositoryMock.Verify(repo => repo.RemoveItemFromCartAsync(1, 1), Times.Never);
        }

        [Fact]
        public async Task UpdateCartItemQuantityAsync_ShouldRemoveItem_WhenQuantityIsZero()
        {
            // Arrange
            var cartItemDto = new CartDTO { UserId = 1, ProductId = 1, Quantity = 0 };
            var existingCartItem = new Cart { UserId = 1, ProductId = 1, Quantity = 5 };
            var product = new Product { Id = 1, Name = "Product 1", ReservedQuantity = 5 };

            _productServiceMock.Setup(service => service.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _shoppingCartRepositoryMock.Setup(repo => repo.GetCartItemByUserAndProductIdAsync(1, 1))
                .ReturnsAsync(existingCartItem);

            _shoppingCartRepositoryMock.Setup(repo => repo.RemoveItemFromCartAsync(1, 1))
                .ReturnsAsync(true);

            // Act
            var result = await _shoppingCartService.UpdateCartItemQuantityAsync(cartItemDto);

            // Assert
            result.Should().BeTrue();
            _shoppingCartRepositoryMock.Verify(repo => repo.RemoveItemFromCartAsync(1, 1), Times.Once);
            _productServiceMock.Verify(service => service.ReleaseProductStockAsync(1, 0), Times.Once);
        }

        [Fact]
        public async Task GetAbandonedCartItemsAsync_ShouldReturnAbandonedItems()
        {
            // Arrange
            var abandonedCartItems = new List<Cart>
        {
            new Cart { UserId = 1, ProductId = 1, Quantity = 1 },
            new Cart { UserId = 2, ProductId = 2, Quantity = 2 }
        };

            _shoppingCartRepositoryMock.Setup(repo => repo.GetAbandonedCartItemsAsync(It.IsAny<TimeSpan>()))
                .ReturnsAsync(abandonedCartItems);

            // Act
            var result = await _shoppingCartService.GetAbandonedCartItemsAsync(TimeSpan.FromMinutes(15));

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(item => item.UserId == 1 && item.ProductId == 1);
            result.Should().Contain(item => item.UserId == 2 && item.ProductId == 2);
        }
    }
}
