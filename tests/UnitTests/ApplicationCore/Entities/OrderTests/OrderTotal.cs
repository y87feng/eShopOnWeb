using System.Collections.Generic;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.UnitTests.Builders;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.OrderTests;

public class OrderTotal
{
    private decimal _testUnitPrice = 42m;

    [Fact]
    public void IsZeroForNewOrder()
    {
        var order = new OrderBuilder().WithNoItems();

        Assert.Equal(0, order.Total());
    }

    [Fact]
    public void IsCorrectGiven1Item()
    {
        var builder = new OrderBuilder();
        var items = new List<OrderItem>
            {
                new OrderItem(builder.TestCatalogItemOrdered, _testUnitPrice, 1)
            };
        var order = new OrderBuilder().WithItems(items);
        Assert.Equal(_testUnitPrice, order.Total());
    }

    [Fact]
    public void IsCorrectGiven3Items()
    {
        var builder = new OrderBuilder();
        var order = builder.WithDefaultValues();

        Assert.Equal(builder.TestUnitPrice * builder.TestUnits, order.Total());
    }

    // ************* GPT-4o Generated Tests *************

    [Fact]
    public void IsCorrectForMultipleItemsWithDifferentPricesAndQuantities()
    {
        var items = new List<OrderItem>
        {
            new OrderItem(new CatalogItemOrdered(1, "Item1", "uri"), 10m, 2),
            new OrderItem(new CatalogItemOrdered(2, "Item2", "uri"), 20m, 3),
            new OrderItem(new CatalogItemOrdered(3, "Item3", "uri"), 15m, 4)
        };
        var order = new OrderBuilder().WithItems(items);

        Assert.Equal(10m * 2 + 20m * 3 + 15m * 4, order.Total());
    }

    [Fact]
    public void IsCorrectWhenItemsHaveZeroQuantity()
    {
        var items = new List<OrderItem>
        {
            new OrderItem(new CatalogItemOrdered(1, "Item1", "uri"), 10m, 0),
            new OrderItem(new CatalogItemOrdered(2, "Item2", "uri"), 20m, 3)
        };
        var order = new OrderBuilder().WithItems(items);

        Assert.Equal(20m * 3, order.Total());
    }

    [Fact]
    public void IsCorrectWhenItemsHaveZeroPrice()
    {
        var items = new List<OrderItem>
        {
            new OrderItem(new CatalogItemOrdered(1, "Item1", "uri"), 0m, 3),
            new OrderItem(new CatalogItemOrdered(2, "Item2", "uri"), 15m, 4)
        };
        var order = new OrderBuilder().WithItems(items);

        Assert.Equal(15m * 4, order.Total());
    }

    [Fact]
    public void HandlesLargeValuesWithoutOverflow()
    {
        var largeValue = decimal.MaxValue / 2;
        var items = new List<OrderItem>
        {
            new OrderItem(new CatalogItemOrdered(1, "Item1", "uri"), largeValue, 1),
            new OrderItem(new CatalogItemOrdered(2, "Item2", "uri"), largeValue, 1)
        };
        var order = new OrderBuilder().WithItems(items);

        Assert.Equal(largeValue * 2, order.Total());
    }

    // ************* GPT-4o Generated Tests *************

    // ************* Sonnet 3.5 Generated Tests *************
    [Fact]
    public void IsCorrectGivenMultipleItemsWithDifferentPrices()
    {
        var builder = new OrderBuilder();
        var items = new List<OrderItem>
    {
        new OrderItem(builder.TestCatalogItemOrdered, _testUnitPrice, 1),
        new OrderItem(builder.TestCatalogItemOrdered, _testUnitPrice * 2, 2),
        new OrderItem(builder.TestCatalogItemOrdered, _testUnitPrice / 2, 3)
    };
        var order = new OrderBuilder().WithItems(items);

        // Expected total: (42 * 1) + (84 * 2) + (21 * 3) = 42 + 168 + 63 = 273
        decimal expectedTotal = (_testUnitPrice * 1) + (_testUnitPrice * 2 * 2) + (_testUnitPrice / 2 * 3);
        Assert.Equal(expectedTotal, order.Total());
    }

    [Fact]
    public void IsCorrectGivenItemWithZeroQuantity()
    {
        var builder = new OrderBuilder();
        var items = new List<OrderItem>
    {
        new OrderItem(builder.TestCatalogItemOrdered, _testUnitPrice, 0)
    };
        var order = new OrderBuilder().WithItems(items);
        Assert.Equal(0, order.Total());
    }

    [Fact]
    public void IsCorrectGivenItemWithZeroPrice()
    {
        var builder = new OrderBuilder();
        var items = new List<OrderItem>
    {
        new OrderItem(builder.TestCatalogItemOrdered, 0, 5)
    };
        var order = new OrderBuilder().WithItems(items);
        Assert.Equal(0, order.Total());
    }

    [Fact]
    public void IsCorrectGivenMixOfZeroAndNonZeroPriceItems()
    {
        var builder = new OrderBuilder();
        var items = new List<OrderItem>
    {
        new OrderItem(builder.TestCatalogItemOrdered, _testUnitPrice, 2),
        new OrderItem(builder.TestCatalogItemOrdered, 0, 3),
        new OrderItem(builder.TestCatalogItemOrdered, _testUnitPrice, 1)
    };
        var order = new OrderBuilder().WithItems(items);

        decimal expectedTotal = (_testUnitPrice * 2) + (0 * 3) + (_testUnitPrice * 1);
        Assert.Equal(expectedTotal, order.Total());
    }

    // ***************************
}
