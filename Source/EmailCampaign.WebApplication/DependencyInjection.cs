using EmailCampaign.Application;
using EmailCampaign.Domain;
using EmailCampaign.Core;
using EmailCampaign.Infrastructure;
using EmailCampaign.Query;

namespace EmailCampaign.WebApplication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDomainDI().AddQueryDI().AddCoreDI().AddInfraDI().AddApplicationDI();

            return services;
        }
    }
}
