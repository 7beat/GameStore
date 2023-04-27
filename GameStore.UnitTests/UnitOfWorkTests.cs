using GameStore.DataAccess.Repository;
using GameStoreWeb.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace GameStore.UnitTests
{
    public class UnitOfWorkTests
    {
        [Test]
        public void Save_Should_Call_SaveChanges_Method()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var mockDbContext = new Mock<ApplicationDbContext>(options);
            var unitOfWork = new UnitOfWork(mockDbContext.Object);

            // Act
            unitOfWork.Save();

            // Assert
            mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task SaveAsync_Should_Call_SaveChangesAsync_Method()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var mockDbContext = new Mock<ApplicationDbContext>(options);
            var unitOfWork = new UnitOfWork(mockDbContext.Object);

            // Act
            await unitOfWork.SaveAsync();

            // Assert
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
