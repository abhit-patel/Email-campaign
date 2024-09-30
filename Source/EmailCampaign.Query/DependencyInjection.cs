using EmailCampaign.Query.QueryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Query
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewPermission", policy =>
                    policy.Requirements.Add(new PermissionRequirement("View")));

                options.AddPolicy("EditPermission", policy =>
                    policy.Requirements.Add(new PermissionRequirement("AddEdit")));

                options.AddPolicy("DeletePermission", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Delete")));
            });

            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IPermissionService, PermissionService>();


            return services;
        }
    }
}
