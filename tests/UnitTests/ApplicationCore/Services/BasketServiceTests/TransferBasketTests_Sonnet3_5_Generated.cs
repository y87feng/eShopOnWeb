using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Moq;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.BasketServiceTests
{
    public class TransferBasketTests_Sonnet3_5_Generated
    {
        private readonly Mock<IRepository<Basket>> _mockBasketRepo = new();
        private readonly Mock<IAppLogger<BasketService>> _mockLogger = new();
        private readonly string _anonymousId = "AnonymousId";
        private readonly string _userName = "testuser@example.com";

        [Fact]
        public async Task Should_TransferBasket_FromAnonymousToUserBasket()
        {
            // Arrange
            var anonymousBasket = new Basket(_anonymousId);
            anonymousBasket.AddItem(1, 10.00m, 2);
            anonymousBasket.AddItem(2, 15.00m, 1);

            _mockBasketRepo.Setup(x => x.FirstOrDefaultAsync(It.Is<BasketWithItemsSpecification>(spec =>
                spec.ToString().Contains(_anonymousId))))
                .ReturnsAsync(anonymousBasket);

            _mockBasketRepo.Setup(x => x.FirstOrDefaultAsync(It.Is<BasketWithItemsSpecification>(spec =>
                spec.ToString().Contains(_userName))))
                .ReturnsAsync((Basket)null);

            var basketService = new BasketService(_mockBasketRepo.Object, _mockLogger.Object);

            // Act
            await basketService.TransferBasketAsync(_anonymousId, _userName);

            // Assert
            _mockBasketRepo.Verify(x => x.AddAsync(It.Is<Basket>(b => b.BuyerId == _userName)), Times.Once);
            _mockBasketRepo.Verify(x => x.UpdateAsync(It.Is<Basket>(b =>
                b.BuyerId == _userName &&
                b.Items.Count == 2)), Times.Once);
            _mockBasketRepo.Verify(x => x.DeleteAsync(anonymousBasket), Times.Once);
        }

        [Fact]
        public async Task Should_NotTransfer_WhenAnonymousBasketNotFound()
        {
            // Arrange
            _mockBasketRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>()))
                .ReturnsAsync((Basket)null);

            var basketService = new BasketService(_mockBasketRepo.Object, _mockLogger.Object);

            // Act
            await basketService.TransferBasketAsync(_anonymousId, _userName);

            // Assert
            _mockBasketRepo.Verify(x => x.AddAsync(It.IsAny<Basket>()), Times.Never);
            _mockBasketRepo.Verify(x => x.UpdateAsync(It.IsAny<Basket>()), Times.Never);
            _mockBasketRepo.Verify(x => x.DeleteAsync(It.IsAny<Basket>()), Times.Never);
        }

        [Fact]
        public async Task Should_MergeItems_WhenUserBasketExists()
        {
            // Arrange
            var anonymousBasket = new Basket(_anonymousId);
            anonymousBasket.AddItem(1, 10.00m, 2);

            var userBasket = new Basket(_userName);
            userBasket.AddItem(2, 15.00m, 1);

            _mockBasketRepo.Setup(x => x.FirstOrDefaultAsync(It.Is<BasketWithItemsSpecification>(spec =>
                spec.ToString().Contains(_anonymousId))))
                .ReturnsAsync(anonymousBasket);

            _mockBasketRepo.Setup(x => x.FirstOrDefaultAsync(It.Is<BasketWithItemsSpecification>(spec =>
                spec.ToString().Contains(_userName))))
                .ReturnsAsync(userBasket);

            var basketService = new BasketService(_mockBasketRepo.Object, _mockLogger.Object);

            // Act
            await basketService.TransferBasketAsync(_anonymousId, _userName);

            // Assert
            _mockBasketRepo.Verify(x => x.AddAsync(It.IsAny<Basket>()), Times.Never);
            _mockBasketRepo.Verify(x => x.UpdateAsync(It.Is<Basket>(b =>
                b.BuyerId == _userName &&
                b.Items.Count == 2)), Times.Once);
            _mockBasketRepo.Verify(x => x.DeleteAsync(anonymousBasket), Times.Once);
        }

        [Fact]
        public async Task Should_TransferEmptyBasket_WhenAnonymousBasketHasNoItems()
        {
            // Arrange
            var anonymousBasket = new Basket(_anonymousId);

            _mockBasketRepo.Setup(x => x.FirstOrDefaultAsync(It.Is<BasketWithItemsSpecification>(spec =>
                spec.ToString().Contains(_anonymousId))))
                .ReturnsAsync(anonymousBasket);

            _mockBasketRepo.Setup(x => x.FirstOrDefaultAsync(It.Is<BasketWithItemsSpecification>(spec =>
                spec.ToString().Contains(_userName))))
                .ReturnsAsync((Basket)null);

            var basketService = new BasketService(_mockBasketRepo.Object, _mockLogger.Object);

            // Act
            await basketService.TransferBasketAsync(_anonymousId, _userName);

            // Assert
            _mockBasketRepo.Verify(x => x.AddAsync(It.Is<Basket>(b => b.BuyerId == _userName)), Times.Once);
            _mockBasketRepo.Verify(x => x.UpdateAsync(It.Is<Basket>(b =>
                b.BuyerId == _userName &&
                b.Items.Count == 0)), Times.Once);
            _mockBasketRepo.Verify(x => x.DeleteAsync(anonymousBasket), Times.Once);
        }

        [Fact]
        public async Task Should_HandleDuplicateItems_WhenTransferring()
        {
            // Arrange
            var anonymousBasket = new Basket(_anonymousId);
            anonymousBasket.AddItem(1, 10.00m, 2);

            var userBasket = new Basket(_userName);
            userBasket.AddItem(1, 10.00m, 1); // Same item as anonymous basket

            _mockBasketRepo.Setup(x => x.FirstOrDefaultAsync(It.Is<BasketWithItemsSpecification>(spec =>
                spec.ToString().Contains(_anonymousId))))
                .ReturnsAsync(anonymousBasket);

            _mockBasketRepo.Setup(x => x.FirstOrDefaultAsync(It.Is<BasketWithItemsSpecification>(spec =>
                spec.ToString().Contains(_userName))))
                .ReturnsAsync(userBasket);

            var basketService = new BasketService(_mockBasketRepo.Object, _mockLogger.Object);

            // Act
            await basketService.TransferBasketAsync(_anonymousId, _userName);

            // Assert
            _mockBasketRepo.Verify(x => x.AddAsync(It.IsAny<Basket>()), Times.Never);
            _mockBasketRepo.Verify(x => x.UpdateAsync(It.Is<Basket>(b =>
                b.BuyerId == _userName &&
                b.Items.Count == 1 &&
                b.Items[0].Quantity == 3)), Times.Once);
            _mockBasketRepo.Verify(x => x.DeleteAsync(anonymousBasket), Times.Once);
        }
    }
}
