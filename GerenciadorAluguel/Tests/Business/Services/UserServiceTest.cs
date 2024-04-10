using Business.Services;
using Core.Interfaces.Repositories;
using Core.Models;
using FluentAssertions;
using Moq;

namespace Tests.Business.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> userRepositoryMock;

        public UserServiceTest()
        {
            userRepositoryMock = new Mock<IUserRepository>();
        }

        [Fact]
        public async Task AddAsync_ValidUser_CallsRepositoryMethod()
        {
            // Arrange
            var userService = new UserService(userRepositoryMock.Object);
            var user = new User { Id = Guid.NewGuid(), Name = "John", CNPJ = "12345678901234", BirthDay = DateTime.Now, CNHNumber = "1234567890", InsertCNHType = new string[] { "A", "B" }, CNHImagePath = "image.jpg" };
            userRepositoryMock.Setup(repo => repo.AddAsync(user)).Verifiable();

            // Act
            await userService.AddAsync(user);

            // Assert
            userRepositoryMock.Verify(repo => repo.AddAsync(user), Times.Once);
        }

        [Fact]
        public async Task GetAllCNHTypeAsync_ReturnsListOfCNHType()
        {
            // Arrange
            var userService = new UserService(userRepositoryMock.Object);
            var expectedCNHTypes = new List<TypeCNH> { new TypeCNH { Id = Guid.NewGuid(), Type = "A", Description = "Category A", Valid = true } };
            userRepositoryMock.Setup(repo => repo.GetAllCNHTypeAsync(null)).ReturnsAsync(expectedCNHTypes);

            // Act
            var result = await userService.GetAllCNHTypeAsync();

            // Assert
            result.Should().NotBeNull();
            expectedCNHTypes.Count.Should().Be(result.Count());
            expectedCNHTypes.First().Id.Should().Be(result.First().Id);
            expectedCNHTypes.First().Type.Should().Be(result.First().Type);
            expectedCNHTypes.First().Description.Should().Be(result.First().Description);
            expectedCNHTypes.First().Valid.Should().Be(result.First().Valid);
        }

        [Fact]
        public async Task ValidateNewUserAsync_ValidUser_ReturnsNoErrors()
        {
            // Arrange
            var userService = new UserService(userRepositoryMock.Object);
            var user = new User { Id = Guid.NewGuid(), Name = "John", CNPJ = "12345678901234", BirthDay = DateTime.Now, CNHNumber = "1234567890", InsertCNHType = new string[] { "A", "B" }, CNHImagePath = "image.jpg" };
            var validCNHTypes = new List<TypeCNH> { new TypeCNH { Type = "A" }, new TypeCNH { Type = "B" } };
            userRepositoryMock.Setup(repo => repo.GetAllCNHTypeAsync(true)).ReturnsAsync(validCNHTypes);
            userRepositoryMock.Setup(repo => repo.IsCNHUniqueAsync(user.CNHNumber)).ReturnsAsync(true);
            userRepositoryMock.Setup(repo => repo.IsCPJUniqueAsync(user.CNPJ)).ReturnsAsync(true);

            // Act
            var result = await userService.ValidateNewUserAsync(user);

            // Assert
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateNewUserAsync_InvalidUser_ReturnsErrors()
        {
            // Arrange
            var userService = new UserService(userRepositoryMock.Object);
            var user = new User { Id = Guid.NewGuid(), Name = "John", CNPJ = "12345678901234", BirthDay = DateTime.Now, CNHNumber = "1234567890", InsertCNHType = new string[] { "C" }, CNHImagePath = "image.jpg" };
            var validCNHTypes = new List<TypeCNH> { new TypeCNH { Type = "A" }, new TypeCNH { Type = "B" } };
            userRepositoryMock.Setup(repo => repo.GetAllCNHTypeAsync(true)).ReturnsAsync(validCNHTypes);
            userRepositoryMock.Setup(repo => repo.IsCNHUniqueAsync(user.CNHNumber)).ReturnsAsync(false);
            userRepositoryMock.Setup(repo => repo.IsCPJUniqueAsync(user.CNPJ)).ReturnsAsync(false);

            // Act
            var result = await userService.ValidateNewUserAsync(user);

            // Assert
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UploadCnhImageAsync_ValidIdAndStream_CallsRepositoryMethods()
        {
            // Arrange
            var userService = new UserService(userRepositoryMock.Object);
            var userId = Guid.NewGuid();
            var stream = new MemoryStream();
            var contentType = "image/jpeg";
            userRepositoryMock.Setup(repo => repo.GetLastCNHImagePathOfUserAsync(userId)).ReturnsAsync("image.jpg");
            userRepositoryMock.Setup(repo => repo.UploadCnhImageAsync(stream, contentType, It.IsAny<string>())).Verifiable();
            userRepositoryMock.Setup(repo => repo.UpdateCNHImagePathAsync(userId, It.IsAny<string>())).Verifiable();

            // Act
            await userService.UploadCnhImageAsync(userId, stream, contentType);

            // Assert
            userRepositoryMock.Verify(repo => repo.UploadCnhImageAsync(stream, contentType, It.IsAny<string>()), Times.Once);
            userRepositoryMock.Verify(repo => repo.UpdateCNHImagePathAsync(userId, It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetListUserNotifiedByIdDeliveryOrder_ValidId_ReturnsListOfUsers()
        {
            // Arrange
            var userService = new UserService(userRepositoryMock.Object);
            var deliveryOrderId = Guid.NewGuid();
            var expectedUsers = new List<User> { new User { Id = Guid.NewGuid(), Name = "John" } };
            userRepositoryMock.Setup(repo => repo.GetListUserNotifiedByIdDeliveryOrder(deliveryOrderId)).ReturnsAsync(expectedUsers);

            // Act
            var result = await userService.GetListUserNotifiedByIdDeliveryOrder(deliveryOrderId);

            // Assert
            result.Should().NotBeNull();
            expectedUsers.Count.Should().Be(result.Count());
            expectedUsers.First().Id.Should().Be(result.First().Id);
            expectedUsers.First().Name.Should().Be(result.First().Name);
        }
    }
}
