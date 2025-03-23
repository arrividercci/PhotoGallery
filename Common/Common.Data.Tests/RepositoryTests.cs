using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Data.Tests
{
    public class RepositoryTests
    {
        private readonly TestItem item = new() { Name = "Test" };

        private readonly TestDbContext dbContext;
        private readonly ILogger logger;
        private readonly Repository<TestItem, int> repository;

        public RepositoryTests()
        {
            dbContext = CreateDbContext();
            dbContext.Set<TestItem>().Add(item);
            dbContext.SaveChanges();
            logger = A.Fake<ILogger>();
            repository = new Repository<TestItem, int>(dbContext, logger);
        }

        [Fact]
        public async Task Repository_Should_Add_Entity()
        {
            //Arrange
            TestItem testItem = new() { Name = "Test" };
            int carCount = dbContext.Items.Count();

            //Act
            repository.Add(testItem);
            await repository.SaveChangesAsync();

            //Assert
            dbContext.Items.Should().HaveCount(c => c == carCount + 1);
        }

        [Fact]
        public async Task Repository_GetById_Return_Entity()
        {
            //Arrange

            //Act
            TestItem? entity = await repository.GetByAsync(item.Id);

            //Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public async Task Repository_GetById_Return_Null()
        {
            // Arrange

            // Act
            TestItem? entity = await repository.GetByAsync(1000);

            // Assert
            entity.Should().BeNull();
        }

        [Fact]
        public void Repository_FindBy_Return_Entity()
        {
            // Arrange

            // Act
            List<TestItem> entity = repository.FindBy(e => e.Id == item.Id).ToList();

            // Assert
            entity.Should().HaveCount(c => c == 1);
        }

        [Fact]
        public async Task Repository_Delete_Entity()
        {
            // Arrange
            int itemCount = dbContext.Items.Count();

            // Act
            repository.Delete(item);
            await repository.SaveChangesAsync();

            // Assert
            dbContext.Set<TestItem>().Should().HaveCount(c => c == itemCount - 1);
        }

        [Fact]
        public async Task Repository_Should_Update_Entity()
        {
            // Arrange
            string newName = "Updated";

            // Act
            item.Name = newName;
            repository.Update(item);
            await repository.SaveChangesAsync();
            TestItem? updatedCar = await repository.GetByAsync(item.Id);

            // Assert
            updatedCar!.Name.Should().Be(newName);
        }

        private TestDbContext CreateDbContext()
        {
            DbContextOptionsBuilder<TestDbContext> builder = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}");

            return new TestDbContext(builder.Options);
        }
    }
}
