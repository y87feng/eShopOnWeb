using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using NSubstitute;
using Ardalis.Specification;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services;

public class TransferBasketTests_GPT4o_Generated
{
    private readonly Mock<IRepository<Basket>> _mockBasketRepository;
    private readonly Mock<IAppLogger<BasketService>> _mockLogger;
    private readonly BasketService _basketService;

    public TransferBasketTests_GPT4o_Generated()
    {
        _mockBasketRepository = new Mock<IRepository<Basket>>();
        _mockLogger = new Mock<IAppLogger<BasketService>>();
        _basketService = new BasketService(_mockBasketRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task TransferBasketAsync_WhenAnonymousBasketIsNull_ShouldDoNothing()
    {
        // Arrange
        string anonymousId = "anon123";
        string userName = "user123";

        // GPT4o original version with syntax error
        //_mockBasketRepository
        //   .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>())
        //   .ReturnsAsync((Basket)null);

        _mockBasketRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), It.IsAny<CancellationToken>())) // After fixed 
            .ReturnsAsync((Basket)null);

        // Act
        await _basketService.TransferBasketAsync(anonymousId, userName);

        // Assert
        _mockBasketRepository.Verify(repo => repo.AddAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never());
        _mockBasketRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never());
        _mockBasketRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task TransferBasketAsync_WhenUserBasketIsNull_ShouldCreateNewUserBasketAndTransferItems()
    {
        // Arrange
        string anonymousId = "anon123";
        string userName = "user123";

        var anonymousBasket = new Basket(anonymousId);
        anonymousBasket.AddItem(1, 10.0m, 2);
        anonymousBasket.AddItem(2, 20.0m, 1);

        _mockBasketRepository.SetupSequence(repo => repo.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(anonymousBasket) // First call returns anonymous basket
            .ReturnsAsync((Basket)null); // Second call returns null for user basket

        // Act
        await _basketService.TransferBasketAsync(anonymousId, userName);

        // Assert
        _mockBasketRepository.Verify(repo => repo.AddAsync(It.Is<Basket>(b => b.BuyerId == userName && b.Items.Count == 2), It.IsAny<CancellationToken>()), Times.Once());
        _mockBasketRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Once());
        _mockBasketRepository.Verify(repo => repo.DeleteAsync(anonymousBasket, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task TransferBasketAsync_WhenUserBasketExists_ShouldTransferItemsToUserBasket()
    {
        // Arrange
        string anonymousId = "anon123";
        string userName = "user123";

        var anonymousBasket = new Basket(anonymousId);
        anonymousBasket.AddItem(1, 10.0m, 2);
        anonymousBasket.AddItem(2, 20.0m, 1);

        var userBasket = new Basket(userName);
        userBasket.AddItem(3, 15.0m, 1);

        _mockBasketRepository.SetupSequence(repo => repo.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(anonymousBasket) // First call returns anonymous basket
            .ReturnsAsync(userBasket); // Second call returns user basket

        // Act
        await _basketService.TransferBasketAsync(anonymousId, userName);

        // Assert
        _mockBasketRepository.Verify(repo => repo.UpdateAsync(It.Is<Basket>(b => b.BuyerId == userName && b.Items.Count == 3), It.IsAny<CancellationToken>()), Times.Once());
        _mockBasketRepository.Verify(repo => repo.DeleteAsync(anonymousBasket, It.IsAny<CancellationToken>()), Times.Once());
    }

    //[Fact]
    //public async Task TransferBasketAsync_WhenAnonymousBasketHasNoItems_ShouldDeleteAnonymousBasket()
    //{
    //    // Arrange
    //    string anonymousId = "anon123";
    //    string userName = "user123";

    //    var anonymousBasket = new Basket(anonymousId);

    //    _mockBasketRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>()))
    //        .ReturnsAsync(anonymousBasket);

    //    _mockBasketRepository.Setup(repo => repo.FirstOrDefaultAsync(It.Is<BasketWithItemsSpecification>(spec => spec.BuyerId == userName)))
    //        .ReturnsAsync((Basket)null);

    //    // Act
    //    await _basketService.TransferBasketAsync(anonymousId, userName);

    //    // Assert
    //    _mockBasketRepository.Verify(repo => repo.AddAsync(It.IsAny<Basket>()), Times.Never);
    //    _mockBasketRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Basket>()), Times.Never);
    //    _mockBasketRepository.Verify(repo => repo.DeleteAsync(anonymousBasket), Times.Once);
    //}
}
