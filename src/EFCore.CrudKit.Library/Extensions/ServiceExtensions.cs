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
using EFCore.CrudKit.Library.Data.Factories;

namespace EFCore.CrudKit.Library.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registers the EFCore DataForge CRUD kit for the specified <see cref="DbContext"/> 
        /// into the dependency injection container.
        /// </summary>
        /// <typeparam name="TContext">
        /// The <see cref="DbContext"/> type used by EFCore DataForge.
        /// </typeparam>
        /// <param name="services">
        /// The service collection to register dependencies with.
        /// </param>
        /// <param name="asSingleton">
        /// Determines the service lifetime:
        /// <list type="bullet">
        /// <item><description><c>true</c> - registers <see cref="IEFCoreCrudKit"/> as a singleton.</description></item>
        /// <item><description><c>false</c> - registers <see cref="IEFCoreCrudKit"/> as scoped.</description></item>
        /// </list>
        /// Default is <c>true</c>.
        /// </param>
        public static void ConfigureEFCoreDataForge<TContext>(this IServiceCollection services,
                                                              bool asSingleton = true) where TContext : DbContext
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
        /// Configures and registers the EFCoreMongoCrudKit for MongoDB operations.
        /// </summary>
        /// <param name="services">The service collection to add the registration to.</param>
        /// <param name="configuration">The application configuration containing MongoDB settings.</param>
        /// <param name="sectionName">The configuration section name that holds EFCoreDataForge options (default is "EFCoreDataForge").</param>
        /// <param name="asSingleton">
        /// Determines the service lifetime:
        /// <list type="bullet">
        /// <item><description><c>true</c> - registers <see cref="IEFCoreMongoCrudKit"/> as a singleton.</description></item>
        /// <item><description><c>false</c> - registers <see cref="IEFCoreMongoCrudKit"/> as scoped.</description></item>
        /// </list>
        /// </param>
        /// <param name="idSerializationMode">
        /// Defines how entity IDs are serialized (default is <see cref="IdSerializationMode.ObjectId"/>).
        /// </param>
        public static void ConfigureMongoEFCoreDataForge(this IServiceCollection services,
                                                         IConfiguration configuration,
                                                         string sectionName = "EFCoreDataForge",
                                                         bool asSingleton = true,
                                                         IdSerializationMode idSerializationMode = IdSerializationMode.ObjectId)
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
        /// Registers the DataForge raw CRUD kit and SQL connection factory 
        /// into the service collection for dependency injection.
        /// </summary>
        /// <param name="services">The service collection to add the services to.</param>
        /// <param name="configuration">The application configuration, used to resolve the connection string.</param>
        /// <param name="connectionStringName">
        /// The name of the connection string in configuration (defaults to "DefaultConnection").
        /// </param>
        /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
        public static IServiceCollection ConfigureDataForgeRawCrudKit(this IServiceCollection services,
                                                        IConfiguration configuration,
                                                        string connectionStringName = "DefaultConnection")
        {
            services.AddSingleton<ISqlConnectionFactory>(sp =>
            {
                return new SqlConnectionFactory(configuration, connectionStringName);
            });

            services.AddScoped<IDataForgeRawCrudKit, DataForgeRawCrudKit>();
            return services;
        }

        /// <summary>
        /// Registers the EFCore DataForge Manager in the dependency injection container.  
        /// Use this method when you need to work with both SQL (via EFCore) and MongoDB 
        /// through a unified manager.
        /// </summary>
        /// <typeparam name="TContext">
        /// The <see cref="DbContext"/> type used by EFCore for SQL database operations.
        /// </typeparam>
        /// <param name="services">
        /// The service collection to register dependencies with.
        /// </param>
        /// <param name="configuration">
        /// The application configuration used to bind <see cref="EFCoreDataForgeOptions"/>.
        /// </param>
        /// <param name="sectionName">
        /// The configuration section name for EFCore DataForge options. Defaults to <c>"EFCoreDataForge"</c>.
        /// </param>
        /// <param name="asSingleton">
        /// Determines the service lifetime:
        /// <list type="bullet">
        /// <item><description><c>true</c> - registers <see cref="IEFCoreDataForgeManager"/> as a singleton.</description></item>
        /// <item><description><c>false</c> - registers <see cref="IEFCoreDataForgeManager"/> as scoped.</description></item>
        /// </list>
        /// Default is <c>true</c>.
        /// </param>
        /// <param name="idSerializationMode">
        /// Specifies how entity IDs should be serialized when working with MongoDB.  
        /// Defaults to <see cref="IdSerializationMode.ObjectId"/>.
        /// </param>
        public static void ConfigureEFCoreDataForgeManager<TContext>(this IServiceCollection services,
                                                                     IConfiguration configuration,
                                                                     string sectionName = "EFCoreDataForge",
                                                                     bool asSingleton = true,
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

        private static void ConfigureIdSerialization(IdSerializationMode mode)
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