using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Net;
using System.Threading.Tasks;

namespace EmailCampaign.Query.QueryService
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(IPermissionService permissionService , IHttpContextAccessor httpContextAccessor)
        {
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
        }

        public void MyMethod()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var controllerName = httpContext.GetRouteData().Values["controller"].ToString();
            // Now you have the controller name
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var httpContext = (context.Resource as AuthorizationFilterContext)?.HttpContext;

            var roleId = context.User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;

            if (roleId == null)
            {
                context.Fail();
                return;
            }

            // Get the controller and action names from the route
            //var controllerName = httpContext.Request RouteValues["controller"]?.ToString();
            //var actionName = httpContext.Request.RouteValues["action"]?.ToString();

            // Query the permissions from the database
            bool hasPermission = await _permissionService.HasPermission(Guid.Parse(roleId), "User", "AddEdit", requirement.PermissionType);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
