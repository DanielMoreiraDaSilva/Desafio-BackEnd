using Business.Services;
using Core.Interfaces.Repositories;
using Core.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Business.Services
{
    public class MotorcycleServiceTest
    {
        private readonly Mock<IMotorcycleRepository> motorcycleRepositoryMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<ILogger<MotorcycleService>> loggerMock;
        public MotorcycleServiceTest()
        {
            motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            loggerMock = new Mock<ILogger<MotorcycleService>>();
        }
        [Fact]
        public async Task UpdatePlateAsync_ValidIdAndPlate_CallsRepositoryUpdate()
        {
            // Arrange
            var motorcycleService = new MotorcycleService(motorcycleRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object);
            var motorcycleId = Guid.NewGuid();
            var newPlate = "DEF456";

            // Act & Assert
            await motorcycleService.UpdatePlateAsync(motorcycleId, newPlate);
            motorcycleRepositoryMock.Verify(repo => repo.UpdatePlateAsync(motorcycleId, newPlate), Times.Once);
        }

        [Fact]
        public async Task ThereIsRentalForMotorcycleAsync_RentalExists_ReturnsTrue()
        {
            // Arrange
            var motorcycleService = new MotorcycleService(motorcycleRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object);
            var motorcycleId = Guid.NewGuid();
            motorcycleRepositoryMock.Setup(repo => repo.ThereIsRentalForMotorcycleAsync(motorcycleId)).ReturnsAsync(true);

            // Act
            var result = await motorcycleService.ThereIsRentalForMotorcycleAsync(motorcycleId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ThereIsRentalForMotorcycleAsync_NoRental_ReturnsFalse()
        {
            // Arrange
            var motorcycleService = new MotorcycleService(motorcycleRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object);
            var motorcycleId = Guid.NewGuid();
            motorcycleRepositoryMock.Setup(repo => repo.ThereIsRentalForMotorcycleAsync(motorcycleId)).ReturnsAsync(false);

            // Act
            var result = await motorcycleService.ThereIsRentalForMotorcycleAsync(motorcycleId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsPlateUniqueAsync_PlateIsUnique_ReturnsTrue()
        {
            // Arrange
            var motorcycleService = new MotorcycleService(motorcycleRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object);
            var plate = "GHI789";
            motorcycleRepositoryMock.Setup(repo => repo.IsPlateUniqueAsync(plate)).ReturnsAsync(true);

            // Act
            var result = await motorcycleService.IsPlateUniqueAsync(plate);

            // Assert
            motorcycleRepositoryMock.Verify(repo => repo.IsPlateUniqueAsync(plate), Times.Once);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsPlateUniqueAsync_PlateIsNotUnique_ReturnsFalse()
        {
            // Arrange
            var motorcycleService = new MotorcycleService(motorcycleRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object);
            var plate = "GHI789";
            motorcycleRepositoryMock.Setup(repo => repo.IsPlateUniqueAsync(plate)).ReturnsAsync(false);

            // Act
            var result = await motorcycleService.IsPlateUniqueAsync(plate);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllRentalPlansAsync_ReturnsListOfRentalPlans()
        {
            // Arrange
            var motorcycleService = new MotorcycleService(motorcycleRepositoryMock.Object, userRepositoryMock.Object, loggerMock.Object);
            var expectedRentalPlans = new List<RentalPlan> { new RentalPlan { Id = Guid.NewGuid(), Name = "Plan A" } };
            motorcycleRepositoryMock.Setup(repo => repo.GetListRentalPlansAsync(null)).ReturnsAsync(expectedRentalPlans);

            // Act
            var result = await motorcycleService.GetAllRentalPlansAsync();

            // Assert
            result.Should().NotBeNull();
            expectedRentalPlans.Count.Should().Be(result.Count());
            expectedRentalPlans.First().Id.Should().Be(result.First().Id);
            expectedRentalPlans.First().Name.Should().Be(result.First().Name);
        }
    }
}
