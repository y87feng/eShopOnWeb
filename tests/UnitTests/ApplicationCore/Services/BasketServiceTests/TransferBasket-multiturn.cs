using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Moq;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services
{
    public class BasketServiceTests2
    {
        private readonly Mock<IRepository<Basket>> _mockBasketRepository;
        private readonly Mock<IAppLogger<BasketService>> _mockLogger;
        private readonly BasketService _basketService;

        public BasketServiceTests2()
        {
            // Mock repository for Basket
            _mockBasketRepository = new Mock<IRepository<Basket>>();

            // Mock logger
            _mockLogger = new Mock<IAppLogger<BasketService>>();

            // Instantiate BasketService with mocked dependencies
            _basketService = new BasketService(
                _mockBasketRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task TransferBasketAsync_AnonymousBasketDoesNotExist_ShouldExitEarly()
        {
            // Arrange
            var anonymousId = "anon123";
            var userName = "user123";
            _mockBasketRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync((Basket)null);

            // Act
            await _basketService.TransferBasketAsync(anonymousId, userName);

            // Assert
            _mockBasketRepository.Verify(repo => repo.AddAsync(It.IsAny<Basket>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
            _mockBasketRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Basket>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task TransferBasketAsync_UserBasketDoesNotExist_ShouldCreateNewBasket()
        {
            // Arrange
            var anonymousId = "anon123";
            var userName = "user123";
            var anonymousBasket = new Basket(anonymousId);
            anonymousBasket.AddItem(1, 10.0m, 2);

            _mockBasketRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync((Basket spec) => spec.BuyerId == anonymousId ? anonymousBasket : null);

            // Act
            await _basketService.TransferBasketAsync(anonymousId, userName);

            // Assert
            _mockBasketRepository.Verify(repo => repo.AddAsync(It.Is<Basket>(b => b.BuyerId == userName), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            _mockBasketRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Basket>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            _mockBasketRepository.Verify(repo => repo.DeleteAsync(It.Is<Basket>(b => b.BuyerId == anonymousId), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TransferBasketAsync_UserBasketExists_ShouldMergeItems()
        {
            // Arrange
            var anonymousId = "anon123";
            var userName = "user123";
            var anonymousBasket = new Basket(anonymousId);
            anonymousBasket.AddItem(1, 10.0m, 2);

            var userBasket = new Basket(userName);
            userBasket.AddItem(1, 10.0m, 1);

            _mockBasketRepository.SetupSequence(repo => repo.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(anonymousBasket) // First call returns anonymous basket
                .ReturnsAsync(userBasket);    // Second call returns user basket

            // Act
            await _basketService.TransferBasketAsync(anonymousId, userName);

            // Assert
            _mockBasketRepository.Verify(repo => repo.UpdateAsync(It.Is<Basket>(b => b.BuyerId == userName && b.TotalItems == 3), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            _mockBasketRepository.Verify(repo => repo.DeleteAsync(It.Is<Basket>(b => b.BuyerId == anonymousId), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TransferBasketAsync_AnonymousBasketEmpty_ShouldOnlyDeleteAnonymousBasket()
        {
            // Arrange
            var anonymousId = "anon123";
            var userName = "user123";
            var anonymousBasket = new Basket(anonymousId); // Empty basket

            _mockBasketRepository.SetupSequence(repo => repo.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(anonymousBasket) // First call returns anonymous basket
                .ReturnsAsync((Basket)null);  // Second call returns no user basket

            // Act
            await _basketService.TransferBasketAsync(anonymousId, userName);

            // Assert
            // Assert
            _mockBasketRepository.Verify(repo => repo.AddAsync(It.IsAny<Basket>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
            _mockBasketRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Basket>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
            _mockBasketRepository.Verify(repo => repo.DeleteAsync(It.Is<Basket>(b => b.BuyerId == anonymousId), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
