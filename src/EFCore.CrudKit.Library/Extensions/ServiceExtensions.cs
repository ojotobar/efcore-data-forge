using EFCore.CrudKit.Library.Data.Implementations;
using EFCore.CrudKit.Library.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.CrudKit.Library.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registers the EFCore.DataForge package in the DI
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
    }
}
