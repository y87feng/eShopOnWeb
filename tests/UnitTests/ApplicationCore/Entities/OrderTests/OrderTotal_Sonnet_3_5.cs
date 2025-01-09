using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.OrderTests
{
    public class OrderTotal_Sonnet_3_5
    {
        private readonly string _testBuyerId = "test-buyer-id";
        private readonly Address _testAddress = new("123 Main St.", "Anytown", "CA", "90210", "USA");

        [Fact]
        public void Returns_Zero_Given_Empty_Order()
        {
            // Arrange
            var order = new Order(_testBuyerId, _testAddress, new List<OrderItem>());

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(0m, total);
        }

        [Fact]
        public void Calculates_Total_Given_Single_Item()
        {
            // Arrange
            var items = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(1, "Test Product", "test.jpg"), 10.00m, 2)
            };
            var order = new Order(_testBuyerId, _testAddress, items);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(20.00m, total);
        }

        [Fact]
        public void Calculates_Total_Given_Multiple_Items()
        {
            // Arrange
            var items = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(1, "Product 1", "test1.jpg"), 10.00m, 2),
                new OrderItem(new CatalogItemOrdered(2, "Product 2", "test2.jpg"), 15.00m, 3),
                new OrderItem(new CatalogItemOrdered(3, "Product 3", "test3.jpg"), 20.00m, 1)
            };
            var order = new Order(_testBuyerId, _testAddress, items);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(85.00m, total); // (10 * 2) + (15 * 3) + (20 * 1) = 20 + 45 + 20 = 85
        }

        [Fact]
        public void Calculates_Total_With_Decimal_Prices()
        {
            // Arrange
            var items = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(1, "Product 1", "test1.jpg"), 10.99m, 2),
                new OrderItem(new CatalogItemOrdered(2, "Product 2", "test2.jpg"), 15.50m, 3)
            };
            var order = new Order(_testBuyerId, _testAddress, items);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(68.48m, total); // (10.99 * 2) + (15.50 * 3) = 21.98 + 46.50 = 68.48
        }

        [Fact]
        public void Calculates_Total_With_Maximum_Decimal_Values()
        {
            // Arrange
            var items = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(1, "Product 1", "test1.jpg"), decimal.MaxValue / 2, 1),
                new OrderItem(new CatalogItemOrdered(2, "Product 2", "test2.jpg"), decimal.MaxValue / 2, 1)
            };
            var order = new Order(_testBuyerId, _testAddress, items);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(decimal.MaxValue, total);
        }

        [Fact]
        public void Calculates_Total_With_Many_Items_Same_Price()
        {
            // Arrange
            var items = new List<OrderItem>();
            for (int i = 1; i <= 100; i++)
            {
                items.Add(new OrderItem(
                    new CatalogItemOrdered(i, $"Product {i}", $"test{i}.jpg"),
                    1.00m,
                    1));
            }
            var order = new Order(_testBuyerId, _testAddress, items);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(100.00m, total);
        }

        [Fact]
        public void Handles_Zero_Price_Items()
        {
            // Arrange
            var items = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(1, "Free Product", "free.jpg"), 0m, 5),
                new OrderItem(new CatalogItemOrdered(2, "Regular Product", "regular.jpg"), 10.00m, 1)
            };
            var order = new Order(_testBuyerId, _testAddress, items);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(10.00m, total);
        }

        [Fact]
        public void Handles_Zero_Quantity_Items()
        {
            // Arrange
            var items = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(1, "Zero Quantity", "zero.jpg"), 10.00m, 0),
                new OrderItem(new CatalogItemOrdered(2, "Regular Product", "regular.jpg"), 10.00m, 1)
            };
            var order = new Order(_testBuyerId, _testAddress, items);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(10.00m, total);
        }
    }
}
