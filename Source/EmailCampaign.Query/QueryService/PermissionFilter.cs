//using EmailCampaign.Domain.Entities;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace EmailCampaign.Query.QueryService
//{
//    public class PermissionFilter : AuthorizeAttribute, IAsyncAuthorizationFilter 
//    {
//        private readonly string _permissionType;
//        private readonly IAuthorizationService _authorizationService;
//        private readonly IPermissionService _permissionService;

//        public PermissionFilter(string permissionType, IAuthorizationService authorizationService, IPermissionService permissionService)
//        {
//            _permissionType = permissionType;
//            _authorizationService = authorizationService;
//            _permissionService = permissionService;
//        }


//        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
//        {
//            var user = context.HttpContext.User;

//            if (!user.Identity.IsAuthenticated)
//            {
//                context.Result = new UnauthorizedResult();
//                return;
//            }

//            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            var controllerName = context.ActionDescriptor.RouteValues["controller"];
//            var actionName = context.ActionDescriptor.RouteValues["action"];

//            var rolePermission = await _permissionService.GetRolePermissionAsync(Guid.Parse(userId), controllerName, actionName);

//            if (rolePermission == null || !HasPermission(rolePermission, actionName))
//            {
//                context.Result = new ForbidResult();
//            }
//        }

//        private bool HasPermission(RolePermission rolePermission, string actionName)
//        {
//            return actionName switch
//            {
//                "View" => rolePermission.IsView,
//                "AddEdit" => rolePermission.IsAddEdit,
//                "Delete" => rolePermission.IsDelete,
//                _ => false
//            };
//        }
//    }
//}
