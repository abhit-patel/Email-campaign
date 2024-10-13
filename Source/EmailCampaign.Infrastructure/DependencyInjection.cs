using EmailCampaign.Core.SharedKernel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Interfaces.Core;
using EmailCampaign.Domain.Interfaces.Log;
using EmailCampaign.Infrastructure.Data.Repositories;
using EmailCampaign.Infrastructure.Data.Repositories.Core;
using EmailCampaign.Infrastructure.Data.Repositories.Log;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EmailCampaign.Infrastructure.Data.Services.LogsService.ActivityLogAttribute;

namespace EmailCampaign.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraDI(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();


            services.AddScoped<IAuthRepository, AuthService>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IActivityLogRepository, JsonActivityLogRepository>();
            services.AddScoped<IErrorLogRepository, JsonErrorLogRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ActivityLogAttribute>(_ => new ActivityLogAttribute ("","","") );
            services.AddScoped<ErrorLogFilter>();


            return services;
        }
    }
}
