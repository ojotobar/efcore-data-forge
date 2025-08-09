using EFCore.CrudKit.Library.Data.Implementations;
using EFCore.CrudKit.Tests.Data;
using EFCore.CrudKit.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCore.CrudKit.Tests
{
    public class DataForgeTest
    {
        private readonly EFCoreCrudKit<DataForgeTestDbContext> _repository;
        private readonly DataForgeTestDbContext _dbContext;

        public DataForgeTest()
        {
            var options = new DbContextOptionsBuilder<DataForgeTestDbContext>()
            .UseInMemoryDatabase("DataForgeTestDb")
            .Options;

            var context = new DataForgeTestDbContext(options);
            _dbContext = context;
            _repository = new EFCoreCrudKit<DataForgeTestDbContext>(context);
        }

        [Fact]
        public async Task InsertAsync_Should_Add_Entity()
        {
            // Arrange
            var entity = new DataForgeTestUser { Name = "Item1" };

            // Act
            await _repository.InsertAsync(entity);

            // Assert
            var savedEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(entity.Id));
            Assert.NotNull(savedEntity);
            Assert.Equal(entity.Id, savedEntity.Id);
        }

        [Fact]
        public async Task InsertRangeAsync_Should_Add_Multiple_Entities()
        {
            // Arrange
            var entities = new List<DataForgeTestUser>
            {
                new DataForgeTestUser { Name = "A" },
                new DataForgeTestUser { Name = "B" }
            };

            // Act
            await _repository.InsertRangeAsync(entities);

            // Assert
            Assert.Equal(2, await _dbContext.Users.CountAsync(u => entities.Select(e => e.Id).Contains(u.Id)));
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Entity()
        {
            // Arrange
            var entity = new DataForgeTestUser { Name = "Old" };
            await _repository.InsertAsync(entity);

            // Act
            entity.Name = "New";
            await _repository.UpdateAsync(entity);

            // Assert
            var updated = await _repository.FindByIdAsync<DataForgeTestUser>(entity.Id, false);
            Assert.NotNull(updated);
            Assert.Equal("New", updated.Name);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Entity()
        {
            // Arrange
            var entity = new DataForgeTestUser { Name = "Delete" };
            await _repository.InsertAsync(entity);

            // Act
            await _repository.DeleteAsync(entity);

            // Assert
            var deleted = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == entity.Id);
            Assert.True(deleted is null);
        }

        [Fact]
        public async Task ToggleAsync_Should_Flip_IsDeprecated()
        {
            var entity = new DataForgeTestUser { Name = "Toggle", IsDeprecated = false };
            await _repository.InsertAsync(entity);

            await _repository.ToggleAsync(entity);

            var updated = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == entity.Id);
            Assert.NotNull(updated);
            Assert.True(updated.IsDeprecated);
        }

        [Fact]
        public async Task FindByIdAsync_Should_Return_Entity()
        {
            // Arrange
            var id = Guid.NewGuid();
            await _repository.InsertAsync(new DataForgeTestUser { Id = id, Name = "Find" });

            // Act
            var found = await _repository.FindByIdAsync<DataForgeTestUser>(id, trackChanges: true);

            // Assert
            Assert.NotNull(found);
            Assert.Equal(id, found.Id);
        }

        [Fact]
        public async Task ExistsAsync_Should_Return_True_If_Entity_Exists()
        {
            var id = Guid.NewGuid();
            await _repository.InsertAsync(new DataForgeTestUser { Id = id, Name = "Exists" });

            var exists = await _repository.ExistsAsync<DataForgeTestUser>(x => x.Id == id);

            Assert.True(exists);
        }

        [Fact]
        public async Task CountAsync_Should_Return_Correct_Number()
        {
            // Arrange
            var users = new List<DataForgeTestUser>
            {
                new DataForgeTestUser { Name = "One" },
                new DataForgeTestUser { Name = "Two" }
            };

            // Act
            await _repository.InsertRangeAsync(users);

            // Assert
            var count = await _repository.CountAsync<DataForgeTestUser>(x => users.Select(i => i.Id).Contains(x.Id));
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task ToggleAsync_ShouldToggleEntities()
        {
            // Arrange
            var entities = new List<DataForgeTestUser>
            {
                new DataForgeTestUser { Name = "ToggleMany", IsDeprecated = true },
                new DataForgeTestUser { Name = "ToggleMany1" }
            };

            await _repository.InsertRangeAsync(entities);

            // Act
            await _repository.ToggleAsync(entities);

            // Assert
            Assert.False(entities[0].IsDeprecated);
            Assert.True(entities[1].IsDeprecated);
        }

        [Fact]
        public async Task DeleteRangeAsync_ShouldDeleteEntities()
        {
            // Arrange
            var entities = new List<DataForgeTestUser>
            {
                new DataForgeTestUser { Name = "DeleteMany" },
                new DataForgeTestUser { Name = "DeleteMany1" }
            };

            await _repository.InsertRangeAsync(entities);

            // Act
            await _repository.DeleteRangeAsync(entities);
            var count = await _repository.CountAsync<DataForgeTestUser>(u => entities.Select(e => e.Id).Contains(u.Id));

            // Assert
            Assert.True(count == 0);
        }

        [Fact]
        public async Task UpdateRangeAsync_ShouldUpdateEntities()
        {
            // Arrange
            var entities = new List<DataForgeTestUser>
            {
                new DataForgeTestUser { Name = "UpdateMany" },
                new DataForgeTestUser { Name = "UpdateMany" }
            };

            await _repository.InsertRangeAsync(entities);
            entities[0].Name = "New UpdateMany";

            // Act
            await _repository.UpdateRangeAsync(entities);

            // Assert
            Assert.Equal("New UpdateMany", entities[0].Name);
        }

        [Fact]
        public async Task SaveAsync_Should_Save_Entity()
        {
            // Arrange
            var entity = new DataForgeTestUser { Name = "Item1" };
            await _repository.InsertAsync(entity, false);

            // Act
            await _repository.SaveAsync();

            // Assert
            var savedEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(entity.Id));
            Assert.NotNull(savedEntity);
            Assert.Equal(entity.Id, savedEntity.Id);
        }
    }
}