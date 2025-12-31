using NetCore.AutoRegisterDi;
using System.Reflection;

namespace Hms.WebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebAppServices(this IServiceCollection services)
        {
            var webAppAssembly = Assembly.Load("Hms.WebApp");

            // Auto-register ALL Services
            services.RegisterAssemblyPublicNonGenericClasses(webAppAssembly)
                   .Where(x => x.Name.EndsWith("Service"))
                   .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

            // Auto-register ALL Helpers
            services.RegisterAssemblyPublicNonGenericClasses(webAppAssembly)
                   .Where(x => x.Name.EndsWith("Helper"))
                   .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

            return services;
        }
    }
}
