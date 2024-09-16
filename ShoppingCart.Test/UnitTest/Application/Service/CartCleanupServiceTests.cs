using ShoppingCart.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using FluentAssertions;
using ShoppingCart.Entity.Model;

namespace ShoppingCart.Test.UnitTest.Application.Service
{
    public class CartCleanupServiceTests
    {
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IShoppingCartService> _shoppingCartServiceMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly CartCleanupService _cartCleanupService;

        public CartCleanupServiceTests()
        {
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _shoppingCartServiceMock = new Mock<IShoppingCartService>();
            _productServiceMock = new Mock<IProductService>();
            _serviceProviderMock = new Mock<IServiceProvider>();

            _serviceProviderMock.Setup(provider => provider.GetService(typeof(IServiceScopeFactory)))
                                .Returns(_serviceScopeFactoryMock.Object);

            _serviceScopeFactoryMock.Setup(factory => factory.CreateScope())
                                    .Returns(_serviceScopeMock.Object);

            _serviceScopeMock.Setup(scope => scope.ServiceProvider)
                             .Returns(_serviceProviderMock.Object);

            _serviceProviderMock.Setup(provider => provider.GetService(typeof(IShoppingCartService)))
                                .Returns(_shoppingCartServiceMock.Object);

            _serviceProviderMock.Setup(provider => provider.GetService(typeof(IProductService)))
                                .Returns(_productServiceMock.Object);

            _cartCleanupService = new CartCleanupService(_serviceProviderMock.Object);
        }


        [Fact]
        public async Task CartCleanupService_ShouldInvokeCleanupAbandonedCarts()
        {
            // Arrange
            var abandonedCartItems = new List<Cart>
        {
            new Cart { UserId = 1, ProductId = 101, Quantity = 1 },
            new Cart { UserId = 2, ProductId = 102, Quantity = 1 }
        };

            _shoppingCartServiceMock
                .Setup(service => service.GetAbandonedCartItemsAsync(It.IsAny<TimeSpan>()))
                .ReturnsAsync(abandonedCartItems);

            // Act
            await _cartCleanupService.CleanupAbandonedCarts(_shoppingCartServiceMock.Object);

            // Assert
            _shoppingCartServiceMock.Verify(service => service.GetAbandonedCartItemsAsync(It.IsAny<TimeSpan>()), Times.Once);
            _shoppingCartServiceMock.Verify(service => service.RemoveItemFromCartAsync(1, 101), Times.Once);
            _shoppingCartServiceMock.Verify(service => service.RemoveItemFromCartAsync(2, 102), Times.Once);
        }


        [Fact]
        public async Task CartCleanupService_ShouldHandleCancellationGracefully()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(50));

            _shoppingCartServiceMock.Setup(service => service.GetAbandonedCartItemsAsync(It.IsAny<TimeSpan>()))
                                    .ReturnsAsync(new List<Cart>());

            // Act
            await _cartCleanupService.StartAsync(cts.Token);

            // Assert: Ensure that no further interactions occurred after cancellation
            _shoppingCartServiceMock.Verify(service => service.GetAbandonedCartItemsAsync(It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task CartCleanupService_ShouldNotThrowException_WhenNoAbandonedItems()
        {
            // Arrange
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            var abandonedCartItems = new List<Cart>();

            _shoppingCartServiceMock.Setup(service => service.GetAbandonedCartItemsAsync(It.IsAny<TimeSpan>()))
                                    .ReturnsAsync(abandonedCartItems);

            // Act
            Func<Task> act = async () => { await _cartCleanupService.StartAsync(cts.Token); };

            // Assert: Ensure the cleanup runs without exceptions
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task CartCleanupService_ShouldLogOrCatchExceptionsDuringExecution()
        {
            // Arrange
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            var abandonedCartItems = new List<Cart>
        {
            new Cart { UserId = 1, ProductId = 101 }
        };

            _shoppingCartServiceMock.Setup(service => service.GetAbandonedCartItemsAsync(It.IsAny<TimeSpan>()))
                                    .ReturnsAsync(abandonedCartItems);

            _shoppingCartServiceMock.Setup(service => service.RemoveItemFromCartAsync(It.IsAny<int>(), It.IsAny<int>()))
                                    .ThrowsAsync(new Exception("Test Exception"));

            // Act
            Func<Task> act = async () => { await _cartCleanupService.StartAsync(cts.Token); };

            // Assert: Ensure the service does not crash due to the exception
            await act.Should().NotThrowAsync();
        }
    }
}
