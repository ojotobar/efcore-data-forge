using EFCore.CrudKit.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCore.CrudKit.Tests.Data
{
    public class DataForgeTestDbContext : DbContext
    {
        public DbSet<DataForgeTestUser> Users { get; set; }

        public DataForgeTestDbContext(DbContextOptions<DataForgeTestDbContext> options) : base(options) { }
    }
}
