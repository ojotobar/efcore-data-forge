using EFCore.CrudKit.Library.Data.Interfaces;
using EFCore.CrudKit.Library.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EFCore.CrudKit.Library.Data.Implementations
{
    public sealed class EFCoreDataForgeManager<TContext> : IEFCoreDataForgeManager where TContext : DbContext
    {
        private readonly Lazy<IEFCoreCrudKit> _sqlrudKit;
        private readonly Lazy<IEFCoreMongoCrudKit> _mongoCrudKit;

        public EFCoreDataForgeManager(TContext dbContext, IOptions<EFCoreDataForgeOptions> dataForgeSettings)
        {
            _sqlrudKit = new Lazy<IEFCoreCrudKit>(() =>
                new EFCoreCrudKit<TContext>(dbContext));

            _mongoCrudKit = new Lazy<IEFCoreMongoCrudKit>(() =>
                new EFCoreMongoCrudKit(dataForgeSettings.Value));
        }

        public IEFCoreCrudKit SQL => _sqlrudKit.Value;
        public IEFCoreMongoCrudKit Mongo => _mongoCrudKit.Value;
    }
}
