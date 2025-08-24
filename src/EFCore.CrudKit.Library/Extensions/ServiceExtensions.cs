using EFCore.CrudKit.Library.Data.Implementations;
using EFCore.CrudKit.Library.Data.Interfaces;
using EFCore.CrudKit.Library.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using EFCore.CrudKit.Library.Models.Enums;
using Microsoft.Extensions.Options;

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
        public static void ConfigureMongoEFCoreDataForge(this IServiceCollection services, IConfiguration configuration, 
            string sectionName = "EFCoreDataForge", bool asSingleton = true, IdSerializationMode idSerializationMode = IdSerializationMode.ObjectId)
        {
            services.Configure<EFCoreDataForgeOptions>(configuration.GetSection(sectionName));
            ConfigureIdSerialization(idSerializationMode);

            if (asSingleton)
            {
                services.AddSingleton<IEFCoreMongoCrudKit>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<EFCoreDataForgeOptions>>();
                    return new EFCoreMongoCrudKit(options.Value);
                });
            }
            else
            {
                services.AddScoped<IEFCoreMongoCrudKit>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<EFCoreDataForgeOptions>>();
                    return new EFCoreMongoCrudKit(options.Value);
                });
            }
        }

        /// <summary>
        /// Registers the EFCore.DataForge Manager in the DI. You will need only this method 
        /// if you need to connect using both SQL and MongoDB
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <param name="asSingleton"></param>
        public static void ConfigureEFCoreDataForgeManager<TContext>(this IServiceCollection services, 
            IConfiguration configuration, string sectionName = "EFCoreDataForge", bool asSingleton = true, 
            IdSerializationMode idSerializationMode = IdSerializationMode.ObjectId) where TContext : DbContext
        {
            services.Configure<EFCoreDataForgeOptions>(configuration.GetSection(sectionName));
            ConfigureIdSerialization(idSerializationMode);

            if (asSingleton)
            {
                services.AddSingleton<IEFCoreDataForgeManager>(sp =>
                {
                    var context = sp.GetRequiredService<TContext>();
                    var options = sp.GetRequiredService<IOptions<EFCoreDataForgeOptions>>();
                    return new EFCoreDataForgeManager<TContext>(context, options);
                });
            }
            else
            {
                services.AddScoped<IEFCoreDataForgeManager>(sp =>
                {
                    var context = sp.GetRequiredService<TContext>();
                    var options = sp.GetRequiredService<IOptions<EFCoreDataForgeOptions>>();
                    return new EFCoreDataForgeManager<TContext>(context, options);
                });
            }
        }

        public static void ConfigureIdSerialization(IdSerializationMode mode)
        {
            switch (mode)
            {
                case IdSerializationMode.ObjectId:
                    // Default, store as ObjectId (no need to override usually)
                    BsonSerializer.RegisterSerializer(
                        typeof(ObjectId),
                        new ObjectIdSerializer(BsonType.ObjectId)
                    );
                    break;

                case IdSerializationMode.String:
                    // Store ObjectIds as strings
                    BsonSerializer.RegisterSerializer(
                        typeof(ObjectId),
                        new ObjectIdSerializer(BsonType.String)
                    );
                    break;

                case IdSerializationMode.Guid:
                    // Store Guid in Standard BSON binary format
                    BsonSerializer.RegisterSerializer(
                        typeof(Guid),
                        new GuidSerializer(GuidRepresentation.Standard)
                    );
                    break;
            }
        }
    }
}