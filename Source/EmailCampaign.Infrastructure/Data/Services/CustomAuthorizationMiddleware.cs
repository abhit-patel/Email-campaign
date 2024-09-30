//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EmailCampaign.Infrastructure.Data.Services
//{
//    public class CustomAuthorizationMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public CustomAuthorizationMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
//        {
//            // Get the route values for controller and action
//            var controllerName = context.Request.RouteValues["controller"]?.ToString();
//            var actionName = //context.Request.RouteValues["action"]?.ToString();

//            if (controllerName == null || actionName == null)
//            {
//                context.Response.StatusCode = StatusCodes.Status403Forbidden;
//                await context.Response.WriteAsync("Forbidden");
//                return;
//            }

//            // Get the user's role from claims
//            var roleId = context.User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;

//            if (roleId == null)
//            {
//                context.Response.StatusCode = StatusCodes.Status403Forbidden;
//                await context.Response.WriteAsync("Forbidden");
//                return;
//            }

//            // Check the user's permissions from the database
//            var hasPermission = await permissionService.HasPermission(Guid.Parse(roleId), "User", actionName, "View");

//            if (!hasPermission)
//            {
//                context.Response.StatusCode = StatusCodes.Status403Forbidden;
//                await context.Response.WriteAsync("Forbidden");
//                return;
//            }

//            // Call the next middleware in the pipeline
//            await _next(context);
//        }
//    }
//}
