using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Thunders.TechTest.OutOfBox.Database
{
    public static class EntityFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerDbContext<TContext>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>((options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ThundersTechTestDb"));
            }, ServiceLifetime.Scoped);
            services.AddScoped<IDbConnection>(provider =>
    new SqlConnection(
        configuration.GetConnectionString("ThundersTechTestDb")
    )
);
            return services;
        }
    }
}
