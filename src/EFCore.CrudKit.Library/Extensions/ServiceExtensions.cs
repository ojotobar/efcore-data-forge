using EFCore.CrudKit.Library.Data.Implementations;
using EFCore.CrudKit.Library.Data.Interfaces;
using EFCore.CrudKit.Library.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.CrudKit.Library.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registers the EFCore.DataForge in the DI
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="asSingleton"></param>
        public static void ConfigureEFCoreDataForge<TContext>(this IServiceCollection services, bool asSingleton = true) where TContext : DbContext
        {
            if (asSingleton)
            {
                services.AddSingleton<IEFCoreCrudKit, EFCoreCrudKit<TContext>>();
            }
            else
            {
                services.AddScoped<IEFCoreCrudKit, EFCoreCrudKit<TContext>>();
            }
        }

        /// <summary>
        /// Registers the Mongo EFCore.DataForge in the DI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="asSingleton"></param>
        public static void ConfigureMongoEFCoreDataForge(this IServiceCollection services, bool asSingleton = true)
        {
            if (asSingleton)
            {
                services.AddSingleton<IEFCoreMongoCrudKit, EFCoreMongoCrudKit>();
            }
            else
            {
                services.AddScoped<IEFCoreMongoCrudKit, EFCoreMongoCrudKit>();
            }
        }

        /// <summary>
        /// Registers the EFCore.DataForge Manager in the DI. Yu only need to call this method 
        /// if need to connected using both SQL and MongoDB
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <param name="asSingleton"></param>
        public static void ConfigureEFCoreDataForgeManager<TContext>(this IServiceCollection services, 
            IConfiguration configuration, string sectionName = "EFCoreDataForge", bool asSingleton = true) where TContext : DbContext
        {
            services.Configure<EFCoreDataForgeOptions>(configuration.GetSection(sectionName));

            if (asSingleton)
            {
                services.AddSingleton<IEFCoreDataForgeManager, EFCoreDataForgeManager<TContext>>();
            }
            else
            {
                services.AddScoped<IEFCoreDataForgeManager, EFCoreDataForgeManager<TContext>>();
            }
        }

        /// <summary>
        /// Registers the EFCore.DataForge Manager in the DI. Yu only need to call this method 
        /// if need to connected using both SQL and MongoDB
        /// You should have an instance of IMongoDatabase already registered if you prefer this set up.
        /// This method does not require you to have the EFCoreDataForge options in your appsettings
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="asSingleton"></param>
        public static void ConfigureEFCoreDataForgeManager<TContext>(this IServiceCollection services, bool asSingleton = true) where TContext : DbContext
        {
            if (asSingleton)
            {
                services.AddSingleton<IEFCoreDataForgeManager, EFCoreDataForgeManager<TContext>>();
            }
            else
            {
                services.AddScoped<IEFCoreDataForgeManager, EFCoreDataForgeManager<TContext>>();
            }
        }
    }
}