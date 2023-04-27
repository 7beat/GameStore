using GameStore.DataAccess.Migrations;
using GameStore.DataAccess.Repository;
using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStoreWeb.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace GameStore.UnitTests
{
    public class RepositoryTests
    {
        private ApplicationDbContext _dbContext;
        private Repository<Platform> _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options);
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
        public void GetAll_ShouldReturnAllEntities_WhenCalled()
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

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
            var mockDbContext = new Mock<ApplicationDbContext>(options);

            mockDbContext.Setup(c => c.Set<Platform>()).Returns(mockDbSet.Object);

            var repository = new Repository<Platform>(mockDbContext.Object);

            // Act
            var result = repository.GetAll();

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
