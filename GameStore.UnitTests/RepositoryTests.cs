using GameStore.DataAccess.Repository;
using GameStore.Models;
using GameStoreWeb.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace GameStore.UnitTests
{
    public class RepositoryTests
    {
        private ApplicationDbContext _dbContext;
        private Mock<ApplicationDbContext> _mockDbContext;
        private Repository<Platform> _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _mockDbContext = new Mock<ApplicationDbContext>(options);
            _repository = new Repository<Platform>(_dbContext);
        }

        [Test]
        public void Add_Entity_ShouldBeAddedToDatabase()
        {
            // Arrange
            var entity = new Platform { Id = 1, Name = "Platform" };

            // Act
            _repository.Add(entity);
            _dbContext.SaveChanges();

            // Assert
            var savedEntity = _dbContext.Set<Platform>().SingleOrDefault(e => e.Id == entity.Id);
            Assert.That(savedEntity, Is.Not.Null);
            Assert.That(savedEntity.Name, Is.EqualTo(entity.Name));
        }

        [Test]
        public void Delete_Entity_ShouldBeRemovedFromDatabase()
        {
            // Arrange
            var entity = new Platform { Id = 1, Name = "Platform" };
            _repository.Add(entity);
            _dbContext.SaveChanges();

            // Act
            _repository.Remove(entity);
            _dbContext.SaveChanges();

            // Assert
            var deletedEntity = _dbContext.Set<Platform>().SingleOrDefault(e => e.Id == entity.Id);
            Assert.That(deletedEntity, Is.Null);
        }

        [Test]
        public async Task GetFirstOrDefault_Entity_ShouldReturnSelectedEntity()
        {
            // Arrange
            var expectedEntity = new Platform { Id = 1, Name = "Platform" };
            _repository.Add(expectedEntity);
            _dbContext.SaveChanges();

            // Act 
            _repository.GetFirstOrDefault(x => x.Id == 1);

            // Assert
            var entity = _dbContext.Set<Platform>().SingleOrDefault(e => e.Id == expectedEntity.Id);

            Assert.IsNotNull(entity);
            Assert.AreEqual(expectedEntity.Id, entity.Id);
            Assert.AreEqual(expectedEntity.Name, entity.Name);
        }


        [Test]
        public void GetAll_Entities_ShouldReturnAllEntities()
        {
            // Arrange
            var testData = new List<Platform>
            {
            new Platform { Id = 1, Name = "Platform1" },
            new Platform { Id = 2, Name = "Platform2" },
            new Platform { Id = 3, Name = "Platform3" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Platform>>();
            mockDbSet.As<IQueryable<Platform>>().Setup(m => m.Provider).Returns(testData.Provider);
            mockDbSet.As<IQueryable<Platform>>().Setup(m => m.Expression).Returns(testData.Expression);
            mockDbSet.As<IQueryable<Platform>>().Setup(m => m.ElementType).Returns(testData.ElementType);
            mockDbSet.As<IQueryable<Platform>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

            _mockDbContext.Setup(c => c.Set<Platform>()).Returns(mockDbSet.Object);

            _repository = new Repository<Platform>(_mockDbContext.Object);

            // Act
            var result = _repository.GetAll();

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(e => e.Id == 1));
            Assert.IsTrue(result.Any(e => e.Id == 2));
            Assert.IsTrue(result.Any(e => e.Id == 3));
        }

        [TearDown]
        public void TestCleanup()
        {
            _dbContext.Dispose();
        }

    }
}
