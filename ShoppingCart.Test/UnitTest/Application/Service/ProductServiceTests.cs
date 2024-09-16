using ShoppingCart.Service.Service;
using ShoppingCart.Entity.Model;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Moq;

namespace ShoppingCart.Test.UnitTest.Application.Service
{
    public class ProductServiceTests
    {
        private readonly ProductService _productService;
        private readonly Mock<IProductRepository> _productRepositoryMock;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task GetProductsAsync_ShouldReturnProductDTOs()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 9.99m, ImageUrl = "image1.jpg" },
            new Product { Id = 2, Name = "Product 2", Price = 19.99m, ImageUrl = "image2.jpg" }
        };

            _productRepositoryMock.Setup(repo => repo.GetAllProductsAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Product 1");
            result.Last().Price.Should().Be(19.99m);

            _productRepositoryMock.Verify(repo => repo.GetAllProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldReturnTrue_WhenUpdateSucceeds()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1", Price = 9.99m };
            _productRepositoryMock.Setup(repo => repo.UpdateProductAsync(product))
                .ReturnsAsync(true);

            // Act
            var result = await _productService.UpdateProductAsync(product);

            // Assert
            result.Should().BeTrue();
            _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1", Price = 9.99m };
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Product 1");
            _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productService.GetProductByIdAsync(100);

            // Assert
            result.Should().BeNull();
            _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(100), Times.Once);
        }

        [Fact]
        public async Task ReleaseProductStockAsync_ShouldReduceReservedStock_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1", Price = 9.99m, ReservedQuantity = 10 };
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            await _productService.ReleaseProductStockAsync(1, 5);

            // Assert
            product.ReservedQuantity.Should().Be(5);
            _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(1), Times.Once);
            _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task ReleaseProductStockAsync_ShouldSetReservedStockToZero_WhenQuantityIsLessThanOrEqualToZero()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1", Price = 9.99m, ReservedQuantity = 10 };
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            await _productService.ReleaseProductStockAsync(1, -5);

            // Assert
            product.ReservedQuantity.Should().Be(0);
            _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(1), Times.Once);
            _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task ReleaseProductStockAsync_ShouldNotUpdateProduct_WhenProductDoesNotExist()
        {
            // Arrange
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product)null);

            // Act
            await _productService.ReleaseProductStockAsync(1, 5);

            // Assert
            _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(1), Times.Once);
            _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(It.IsAny<Product>()), Times.Never);
        }
    }
}
