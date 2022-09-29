using Hangfire;
using Hangfire.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace OnePlace.Server.Extenciones
{
    public static class StartupExtenciones
    {
        public static void ConfigureHangFire(this IServiceCollection services, IConfiguration configuration)
        {
            //esto era para SQL server
            //services.AddHangfire(config => config
            //   .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //   .UseSimpleAssemblyNameTypeSerializer()
            //   .UseRecommendedSerializerSettings()
            //   .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
            //   {
            //       CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            //       SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //       QueuePollInterval = TimeSpan.Zero,
            //       UseRecommendedIsolationLevel = true,
            //       DisableGlobalLocks = true
            //   }));

            //Add Hangfire services in MySql.           
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseStorage(
                    new MySqlStorage(configuration.GetConnectionString("DefaultConnection"), new MySqlStorageOptions
                    {
                            TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                            QueuePollInterval = TimeSpan.FromSeconds(15),
                            JobExpirationCheckInterval = TimeSpan.FromHours(1),
                            CountersAggregateInterval = TimeSpan.FromMinutes(5),
                            PrepareSchemaIfNecessary = true,
                            DashboardJobListLimit = 50000,
                            TransactionTimeout = TimeSpan.FromMinutes(1),
                            TablesPrefix = "Hangfire"
                    })));

            //parametro que se le envio "opcional": limitar el número de conexiones abiertas
            services.AddHangfireServer(options => options.WorkerCount = 1);
        }
    }
}
