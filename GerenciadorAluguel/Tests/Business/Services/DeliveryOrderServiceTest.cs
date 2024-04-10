using Business.Services;
using Core.Interfaces.Repositories;
using Core.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace Tests.Business.Services
{
    public class DeliveryOrderServiceTest
    {
        private readonly Mock<IDeliveryOrderRepository> deliveryOrderRepositoryMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<ILogger<DeliveryOrderService>> loggerMock;
        private readonly Func<IDbConnection> connectionFactoryMock;

        public DeliveryOrderServiceTest()
        {
            deliveryOrderRepositoryMock = new Mock<IDeliveryOrderRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            loggerMock = new Mock<ILogger<DeliveryOrderService>>();
            connectionFactoryMock = () => new Mock<IDbConnection>().Object;
        }
        [Fact]
        public async Task AddDeliveryAsync_ValidDeliveryOrder_CallsRepositoryMethods()
        {
            // Arrange
            var deliveryOrderService = new DeliveryOrderService(deliveryOrderRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object, connectionFactoryMock);
            var deliveryOrder = new DeliveryOrder { Id = Guid.NewGuid(), DateCreate = DateTime.Now, CostDelivery = 50.0, IdStatusDeliveryOrder = Guid.NewGuid() };
            deliveryOrderRepositoryMock.Setup(repo => repo.AddDeliveryAsync(deliveryOrder)).Verifiable();
            deliveryOrderRepositoryMock.Setup(repo => repo.SendMessageSQSAsync(deliveryOrder)).Verifiable();

            // Act
            await deliveryOrderService.AddDeliveryAsync(deliveryOrder);

            // Assert
            deliveryOrderRepositoryMock.Verify(repo => repo.AddDeliveryAsync(deliveryOrder), Times.Once);
            deliveryOrderRepositoryMock.Verify(repo => repo.SendMessageSQSAsync(deliveryOrder), Times.Once);
        }

        [Fact]
        public async Task GetAllStatusDeliveryAsync_ReturnsListOfStatusDeliveryOrder()
        {
            // Arrange
            var deliveryOrderService = new DeliveryOrderService(deliveryOrderRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object, connectionFactoryMock);
            var expectedStatusDelivery = new List<StatusDeliveryOrder> { new StatusDeliveryOrder { Id = Guid.NewGuid(), Status = "Status1" } };
            deliveryOrderRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedStatusDelivery);

            // Act
            var result = await deliveryOrderService.GetAllStatusDeliveryAsync();

            // Assert
            result.Should().NotBeNull();
            expectedStatusDelivery.Count.Should().Be(result.Count());
            expectedStatusDelivery.First().Id.Should().Be(result.First().Id);
            expectedStatusDelivery.First().Status.Should().Be(result.First().Status);
        }

        [Fact]
        public async Task IsStatusValidAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var deliveryOrderService = new DeliveryOrderService(deliveryOrderRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object, connectionFactoryMock);
            var statusId = Guid.NewGuid();
            deliveryOrderRepositoryMock.Setup(repo => repo.IsStatusValidAsync(statusId)).ReturnsAsync(true);

            // Act
            var result = await deliveryOrderService.IsStatusValidAsync(statusId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsStatusValidAsync_InvalidId_ReturnsFalse()
        {
            // Arrange
            var deliveryOrderService = new DeliveryOrderService(deliveryOrderRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object, connectionFactoryMock);
            var statusId = Guid.NewGuid();
            deliveryOrderRepositoryMock.Setup(repo => repo.IsStatusValidAsync(statusId)).ReturnsAsync(false);

            // Act
            var result = await deliveryOrderService.IsStatusValidAsync(statusId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsUserNotifiedValidAsync_UserNotified_ReturnsTrue()
        {
            // Arrange
            var deliveryOrderService = new DeliveryOrderService(deliveryOrderRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object, connectionFactoryMock);
            var userId = Guid.NewGuid();
            var deliveryOrderId = Guid.NewGuid();
            var userList = new List<User> { new User { Id = userId } };
            userRepositoryMock.Setup(repo => repo.GetListUserNotifiedByIdDeliveryOrder(deliveryOrderId)).ReturnsAsync(userList);

            // Act
            var result = await deliveryOrderService.IsUserNotifiedValidAsync(userId, deliveryOrderId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsUserNotifiedValidAsync_UserNotNotified_ReturnsFalse()
        {
            // Arrange
            var deliveryOrderService = new DeliveryOrderService(deliveryOrderRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object, connectionFactoryMock);
            var userId = Guid.NewGuid();
            var deliveryOrderId = Guid.NewGuid();
            var userList = new List<User>();
            userRepositoryMock.Setup(repo => repo.GetListUserNotifiedByIdDeliveryOrder(deliveryOrderId)).ReturnsAsync(userList);

            // Act
            var result = await deliveryOrderService.IsUserNotifiedValidAsync(userId, deliveryOrderId);

            // Assert
            result.Should().BeFalse();
        }
    }
}
