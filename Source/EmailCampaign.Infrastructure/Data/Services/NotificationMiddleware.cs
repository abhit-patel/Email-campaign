using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Services
{
    public class NotificationMiddleware
    {
        private readonly RequestDelegate _next;
        public NotificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, INotificationRepository notificationRepository)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var notifications = await notificationRepository.GetNotificationForIconAsync();

                var count = await notificationRepository.GetUnreadNotificationCountAsync();

                context.Items["Notifications"] = notifications;
                context.Items["Count"] = count;

            }
            await _next(context);
        }
    }
}
