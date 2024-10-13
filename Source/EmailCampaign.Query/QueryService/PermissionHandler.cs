using EmailCampaign.Domain.Entities;
using EmailCampaign.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Query.QueryService
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private readonly IPermissionService _permissionService;

        public PermissionHandler(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext, IPermissionService permissionService)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement )
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return;
            }

            Guid userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            bool isSuperAdmin = await _dbContext.User.AnyAsync(p => p.IsSuperAdmin == true && p.ID == userId);

            if(isSuperAdmin)
            {
                context.Succeed(requirement);
                return;
            }

            var roleId =  _dbContext.User.Where(p => p.ID == userId && p.IsDeleted == false).Select(p => p.RoleId.ToString()).SingleOrDefault() ;


            var controllerName = _httpContextAccessor.HttpContext.GetRouteData().Values["controller"].ToString();
            var actionName = _httpContextAccessor.HttpContext.GetRouteData().Values["action"].ToString();

            var rolePermission = await _permissionService.GetRolePermissionAsync(userId, controllerName, actionName);

            if (rolePermission != null)
            {

                //var permission = await _dbContext.RolePermission.Where(p => p.Id == Guid.Parse(roleId.ToString().ToUpper()) && p.Permission.ActionName == requirement.PermissionType).FirstOrDefaultAsync();

                bool hasPermission = false;

                switch (requirement.PermissionType)
                {
                    case "View":
                        hasPermission = rolePermission.IsView;
                        break;
                    case "AddEdit":
                        hasPermission = rolePermission.IsAddEdit;
                        break;
                    case "Delete":
                        hasPermission = rolePermission.IsDelete;
                        break;
                }


                if (hasPermission)
                {
                    context.Succeed(requirement);
                }
            }

        }

    }
}
