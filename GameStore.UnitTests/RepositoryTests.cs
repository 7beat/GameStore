using GameStore.DataAccess.Repository;
using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStoreWeb.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace GameStore.UnitTests
{
    public class RepositoryTests
    {

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Add_Should_Add_Entity_To_DbSet()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);
            var repository = new Repository<Platform>(context);

            var entity = new Platform { Id = 1, Name = "Platform" };

            // Act
            repository.Add(entity);

            // Assert
            var addedEntity = context.Set<Platform>().Find(entity.Id);
            Assert.IsNotNull(addedEntity);
            Assert.AreEqual(entity, addedEntity);
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

        //[Test]
        //public void Add_ShouldAddEntityToDatabase()
        //{
        //    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //    .UseInMemoryDatabase(databaseName: "TestDatabase")
        //    .Options;
        //    var context = new ApplicationDbContext(options);
        //    // Arrange
        //    var mockUnitOfWork = new Mock<IUnitOfWork>();
        //    var repository = new Repository<Platform>(context);
        //    var entity = new Platform { Id = 1, Name = "Platform1" };

        //    mockUnitOfWork.Setup(uow => uow.Save()).Verifiable();

        //    // Act
        //    //repository.Add(entity);
        //    repository.Add(entity);

        //    // Assert
        //    mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        //    var addedEntity = repository.GetFirstOrDefault(x => x.Id == entity.Id);
        //    Assert.AreEqual(entity, addedEntity);
        //}
    }
}
