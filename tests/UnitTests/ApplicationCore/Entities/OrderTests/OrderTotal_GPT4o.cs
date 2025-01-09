using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.OrderTests
{
    public class OrderTotal_GPT4o
    {
        [Fact]
        public void Total_ReturnsZero_WhenNoOrderItems()
        {
            // Arrange
            var order = new Order("buyer123", new Address("Street 1", "City", "State", "Country", "12345"), new List<OrderItem>());

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(0m, total);
        }

        [Fact]
        public void Total_CalculatesCorrectly_WithSingleOrderItem()
        {
            // Arrange
            var orderItems = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(1, "Test Item", "image-url"), 10m, 2)
            };
            var order = new Order("buyer123", new Address("Street 1", "City", "State", "Country", "12345"), orderItems);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(20m, total);
        }

        [Fact]
        public void Total_CalculatesCorrectly_WithMultipleOrderItems()
        {
            // Arrange
            var orderItems = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(1, "Test Item 1", "image-url"), 10m, 2),
                new OrderItem(new CatalogItemOrdered(2, "Test Item 2", "image-url"), 15m, 1),
                new OrderItem(new CatalogItemOrdered(3, "Test Item 3", "image-url"), 5m, 5)
            };
            var order = new Order("buyer123", new Address("Street 1", "City", "State", "Country", "12345"), orderItems);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(70m, total);
        }

        [Fact]
        public void Total_HandlesNegativeUnitPrice()
        {
            // Arrange
            var orderItems = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(1, "Test Item", "image-url"), -10m, 2)
            };
            var order = new Order("buyer123", new Address("Street 1", "City", "State", "Country", "12345"), orderItems);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(-20m, total);
        }

        [Fact]
        public void Total_HandlesZeroUnits()
        {
            // Arrange
            var orderItems = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(1, "Test Item", "image-url"), 10m, 0)
            };
            var order = new Order("buyer123", new Address("Street 1", "City", "State", "Country", "12345"), orderItems);

            // Act
            var total = order.Total();

            // Assert
            Assert.Equal(0m, total);
        }
    }
}
