using EFCore.CrudKit.Library.Data.Implementations;
using EFCore.CrudKit.Library.Settings;
using EFCore.CrudKit.Tests.Data;
using EFCore.CrudKit.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCore.CrudKit.Tests
{
    public class DataForgeTest
    {
        private readonly EFCoreCrudKit<DataForgeTestDbContext> _repository;
        private readonly EFCoreMongoCrudKit _mongoRepository;
        private readonly DataForgeTestDbContext _dbContext;

        public DataForgeTest()
        {
            var options = new DbContextOptionsBuilder<DataForgeTestDbContext>()
            .UseInMemoryDatabase("DataForgeTestDb")
            .Options;

            var context = new DataForgeTestDbContext(options);
            _dbContext = context;
            _repository = new EFCoreCrudKit<DataForgeTestDbContext>(context);

            _mongoRepository = new EFCoreMongoCrudKit(new EFCoreDataForgeOptions
            {
                MongoDb = new MongoDbOptions
                {
                    ConnectionString = "mongodb://localhost:27017/",
                    DatabaseName = "EFCoreDataForgeTestDB"
                }
            });
        }

        #region MongoDB
        [Fact]
        public async Task Mongo_InsertAsync_Should_Add_Entity()
        {
            // Arrange
            var entity = new MongoDataForgeTestUser { Name = "Item1" };

            // Act
            await _mongoRepository.InsertAsync(entity);

            // Assert
            var exists = await _mongoRepository.ExistsAsync<MongoDataForgeTestUser>(u => u.Id.Equals(entity.Id));
            Assert.True(exists);
        }

        [Fact]
        public async Task Mongo_InsertRangeAsync_Should_Add_Multiple_Entities()
        {
            // Arrange
            var entities = new List<MongoDataForgeTestUser>
            {
                new MongoDataForgeTestUser { Name = "A" },
                new MongoDataForgeTestUser { Name = "B" }
            };

            // Act
            await _mongoRepository.InsertRangeAsync(entities);

            // Assert
            Assert.Equal(2, await _mongoRepository
                .CountAsync<MongoDataForgeTestUser>(u => entities.Select(e => e.Id).Contains(u.Id)));
        }

        [Fact]
        public async Task Mongo_FindMany_Should_Return_Multiple_Entities()
        {
            // Arrange
            var entities = new List<MongoDataForgeTestUser>
            {
                new MongoDataForgeTestUser { Name = "FindMany1" },
                new MongoDataForgeTestUser { Name = "FindMany2" }
            };

            // Act
            await _mongoRepository.InsertRangeAsync(entities);
            var addedEntities = await _mongoRepository
                .FindAsync<MongoDataForgeTestUser>(u => entities.Select(e => e.Id).Contains(u.Id));

            // Assert
            Assert.Equal(entities.Count, addedEntities.Count);
        }

        [Fact]
        public async Task Mongo_AsQueryable_Should_Return_Multiple_Entities()
        {
            // Arrange
            var entities = new List<MongoDataForgeTestUser>
            {
                new MongoDataForgeTestUser { Name = "FindMany1" },
                new MongoDataForgeTestUser { Name = "FindMany2" }
            };

            // Act
            await _mongoRepository.InsertRangeAsync(entities);
            var addedEntities = _mongoRepository
                .AsQueryable<MongoDataForgeTestUser>(u => entities.Select(e => e.Id).Contains(u.Id));

            // Assert
            Assert.Equal(entities.Count, addedEntities.Count());
        }

        [Fact]
        public async Task Mongo_ReplaceOneAsync_Should_Modify_Entity()
        {
            // Arrange
            var entity = new MongoDataForgeTestUser { Name = "ReplaceOld" };
            await _mongoRepository.InsertAsync(entity);

            // Act
            entity.Name = "ReplaceNew";
            await _mongoRepository.ReplaceAsync(e => e.Id.Equals(entity.Id), entity);

            // Assert
            var updated = await _mongoRepository.FindOneAsync<MongoDataForgeTestUser>(e => e.Id.Equals(entity.Id));
            Assert.NotNull(updated);
            Assert.Equal("ReplaceNew", updated.Name);
        }

        [Fact]
        public async Task Mongo_UpdateAsync_Should_Modify_Entity()
        {
            // Arrange
            var entity = new MongoDataForgeTestUser { Name = "Old" };
            await _mongoRepository.InsertAsync(entity);

            // Act
            entity.Name = "New";
            await _mongoRepository.UpdateOneAsync(entity, e => e.Id.Equals(entity.Id));

            // Assert
            var updated = await _mongoRepository.FindOneAsync<MongoDataForgeTestUser>(e => e.Id.Equals(entity.Id));
            Assert.NotNull(updated);
            Assert.Equal("New", updated.Name);
        }

        [Fact]
        public async Task Mongo_DeleteAsync_Should_Remove_Entity()
        {
            // Arrange
            var entity = new MongoDataForgeTestUser { Name = "Delete" };
            await _mongoRepository.InsertAsync<MongoDataForgeTestUser>(entity);

            // Act
            await _mongoRepository.DeleteAsync<MongoDataForgeTestUser>(e => e.Id.Equals(entity.Id));

            // Assert
            var deleted = await _mongoRepository.FindOneAsync<MongoDataForgeTestUser>(u => u.Id == entity.Id);
            Assert.True(deleted is null);
        }

        [Fact]
        public async Task Mongo_ExistsAsync_Should_Return_True_If_Entity_Exists()
        {
            var id = Guid.NewGuid();
            await _mongoRepository.InsertAsync(new MongoDataForgeTestUser { Id = id, Name = "Exists" });

            var exists = await _mongoRepository.ExistsAsync<MongoDataForgeTestUser>(x => x.Id == id);

            Assert.True(exists);
        }

        [Fact]
        public async Task Mongo_CountAsync_Should_Return_Correct_Number()
        {
            // Arrange
            var users = new List<MongoDataForgeTestUser>
            {
                new MongoDataForgeTestUser { Name = "One" },
                new MongoDataForgeTestUser { Name = "Two" }
            };

            // Act
            await _mongoRepository.InsertRangeAsync(users);

            // Assert
            var count = await _mongoRepository.CountAsync<MongoDataForgeTestUser>(x => users.Select(i => i.Id).Contains(x.Id));
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Mongo_DeleteRangeAsync_ShouldDeleteEntities()
        {
            // Arrange
            var entities = new List<MongoDataForgeTestUser>
            {
                new MongoDataForgeTestUser { Name = "DeleteMany" },
                new MongoDataForgeTestUser { Name = "DeleteMany1" }
            };

            await _mongoRepository.InsertRangeAsync(entities);

            // Act
            await _mongoRepository.DeleteRangeAsync<MongoDataForgeTestUser>(e => entities.Select(i => i.Id).ToList().Contains(e.Id));
            var count = await _mongoRepository.CountAsync<MongoDataForgeTestUser>(u => entities.Select(e => e.Id).Contains(u.Id));

            // Assert
            Assert.True(count == 0);
        }
        #endregion

        #region SQL
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
        #endregion
    }
}