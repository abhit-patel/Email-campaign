using EmailCampaign.Infrastructure.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Services
{
    public class CustomAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {

        private readonly string _role;
        private readonly string _permission;
        private readonly string _slug;
        //private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;

        public CustomAuthorizeAttribute(string slug )
        {
            //_role = role;
            //_permission = permission;
            //_permissionService = permissionService;
            _slug = slug;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {

            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            var user = context.HttpContext.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roles = await userService.GetUserRolesAsync(Guid.Parse(userId));
            var permissionsSlug = await userService.GetUserPermissionsAsync(Guid.Parse(userId));

            if (!permissionsSlug.Contains(_slug))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
