using GameStore.DataAccess.Repository;
using GameStore.DataAccess.Repository.IRepository;
using GameStoreWeb.Data;
using Moq;
using NUnit.Framework;

namespace GameStore.UnitTests
{
    public class UnitOfWorkTests
    {
        private Mock<ApplicationDbContext> _mockDbContext;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void Setup()
        {
            _mockDbContext = new Mock<ApplicationDbContext>();
            _unitOfWork = new UnitOfWork(_mockDbContext.Object);
        }

        [Test]
        public void Save_Should_Call_SaveChanges_Method()
        {
            // Act
            _unitOfWork.Save();

            // Assert
            _mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task SaveAsync_Should_Call_SaveChangesAsync_Method()
        {
            // Act
            await _unitOfWork.SaveAsync();

            // Assert
            _mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
