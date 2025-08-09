using EFCore.CrudKit.Library.Data.Implementations;
using EFCore.CrudKit.Tests.Data;
using EFCore.CrudKit.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCore.CrudKit.Tests
{
    public class DataForgeTest
    {
        private readonly EFCoreCrudKit<DataForgeTestUser, DataForgeTestDbContext> _repository;
        private readonly DataForgeTestDbContext _dbContext;

        public DataForgeTest()
        {
            var options = new DbContextOptionsBuilder<DataForgeTestDbContext>()
            .UseInMemoryDatabase("DataForgeTestDb")
            .Options;

            var context = new DataForgeTestDbContext(options);
            _dbContext = context;
            _repository = new EFCoreCrudKit<DataForgeTestUser, DataForgeTestDbContext> (context);
        }

        [Fact]
        public async Task CanInsertOneAsync()
        {
            var user = new DataForgeTestUser
            {
                Name = "Test",
            };
            await _repository.InsertAsync(user);

            Assert.Equal(1, await _dbContext.Users.CountAsync(x => x.Id.Equals(user.Id)));
        }

        [Fact]
        public async Task CanInsertManyAsync()
        {
            var users = new List<DataForgeTestUser>
            {
                new DataForgeTestUser
                {
                    Name = "Test1"
                },
                new DataForgeTestUser
                {
                    Name = "Test2"
                }
            };

            await _repository.InsertRangeAsync(users);

            Assert.Equal(2, await _dbContext.Users.CountAsync(x => users.Select(u => u.Id).Contains(x.Id)));
        }
    }
}