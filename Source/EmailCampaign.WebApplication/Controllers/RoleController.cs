using AutoMapper;
using EmailCampaign.Application.Features.RoleWithPermission.Commands;
using EmailCampaign.Application.Features.RoleWithPermission.Queries;
using EmailCampaign.Application.Features.User.Queries;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace EmailCampaign.WebApplication.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public RoleController(IRoleRepository roleRepository, IMapper mapper, IPermissionRepository permissionRepository, IRolePermissionRepository rolePermissionRepository, IUserRepository userRepository, IMediator mediator)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        [Authorize("ViewPermission")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var roleList = await _mediator.Send(new GetAllRoleQuery());

            List<RoleWithUserCount> model = new List<RoleWithUserCount>();

            foreach(var role in roleList)
            {
                GetActiveUserForRoleQuery getActiveUserForRoleQuery = new GetActiveUserForRoleQuery { roleId = role.Id };
                var activeUserList = await _mediator.Send(getActiveUserForRoleQuery, cancellationToken);

                model.Add(new RoleWithUserCount
                {
                    Id = role.Id,
                    RoleName = role.Name,
                    isActive = role.IsActive,
                    activeUser = activeUserList.Count()
                });
            }

            return View(model);
        }


        public async Task<IActionResult> GetActiveUser(Guid id, CancellationToken cancellationToken)
        {
            GetActiveUserForRoleQuery getActiveUserForRoleQuery = new GetActiveUserForRoleQuery { roleId =  id };

            var userList = await _mediator.Send(getActiveUserForRoleQuery, cancellationToken);

            return View(userList);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize("AddEditPermission")]
        [ActionName("GetRoleByID")]
        public async Task<IActionResult> GetRoleById(Guid id, CancellationToken cancellationToken)
        {
            GetRoleByIdQuery getRoleByIdQuery = new GetRoleByIdQuery
            {
                Id = id
            };

            var role = await _mediator.Send(getRoleByIdQuery, cancellationToken);

            RoleVM roleVM = _mapper.Map<RoleVM>(role);

            return View("UpdateUser", roleVM);
        }

        [ActionName("GetRolePermission")]
        public async Task<IActionResult> GetRolePermission(Guid Id, CancellationToken cancellationToken)
        {
            GetRolePermissionByIdQuery rolePermissionQuery = new GetRolePermissionByIdQuery
            {
                Id = Id,
            };

            var rolePermissionList = await _mediator.Send(rolePermissionQuery, cancellationToken);

            await LoadPermission();

            return View("UpdateRolePermission", rolePermissionList);
        }

        [HttpGet]
        [Authorize("AddEditPermission")]
        public async Task<IActionResult> AddRolePermission()
        {
            await LoadPermission();

            var model = new RolePermissionVM
            {
                PermissionList = new List<PermissionListVM>()
            };

            foreach (var permission in ViewBag.Permission)
            {
                model.PermissionList.Add(new PermissionListVM { PermissionId = Guid.Parse(permission.Key) });
            }

            CreateRolePermissionCommand createRolePermissionCommand = _mapper.Map<CreateRolePermissionCommand>(model);

            return View(createRolePermissionCommand);
        }

        [HttpPost]
        [ActionName("AddRolePermission")]
        [Authorize("AddEditPermission")]
        public async Task<IActionResult> Post(CreateRolePermissionCommand model, CancellationToken cancellationToken)
        {
            //if (ModelState.IsValid) { return View("Index"); }

            var rolePermissionVM = await _mediator.Send(model, cancellationToken);

            if (rolePermissionVM == null)
            {
                TempData["ErrorMessage"] = "Failed to Create role permission mapping.";
                return RedirectToAction("index");
            }

            var activityLogAttribute = new ActivityLogAttribute("Create", "New Role created  with name {0} and mapped permission. Created by {1}.", rolePermissionVM.RoleName );
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );

            await activityLogAttribute.LogActivityAsync(context);


            TempData["SuccessMessage"] = "Role created successfully and mapped with permission.";
            return RedirectToAction("Index");
        }



        [HttpPost]
        [ActionName("UpdateRolePermission")]
        public async Task<IActionResult> Update(UpdateRolePermissionCommand model , CancellationToken cancellationToken)
        {
            var rolePermissionModel = await _mediator.Send(model, cancellationToken);

            if (rolePermissionModel == null)
            {
                TempData["ErrorMessage"] = "Failed to update role permission mapping.";
                return RedirectToAction("index");
            }

            var activityLogAttribute = new ActivityLogAttribute("Update", " {0} role name or mapped permission updated by {1}.", model.RoleName + " (" + model.RoleId + ")" );
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);


            TempData["SuccessMessage"] = "Role permission mapping updated successfully.";
            return RedirectToAction("Index","RolePermission");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ActionName("IsActiveToggle")]
        public async Task<IActionResult> IsActiveToggleAsync(string name, CancellationToken cancellationToken)
        {
            RoleActiveToggleCommand updateActiveToggle = new RoleActiveToggleCommand
            {
                name = name
            };

            var role = await _mediator.Send(updateActiveToggle, cancellationToken);

            if (role == null)
            {
                TempData["ErrorMessage"] = "User status has not been toggled successfully.";
                return RedirectToAction("AddUser");
            }


            var activityLogAttribute = new ActivityLogAttribute("IsActive toggle", "Active status changed of user {0}. Updated by {1}. ", role.Name + " (" + role.Id + ") to Active status " + role.IsActive + " ");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);


            TempData["SuccessMessage"] = "User status has been toggled successfully.";
            return RedirectToAction("Index");
        }



        [Authorize("DeletePermission")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            GetRolePermissionByRoleIdQuery getRolePermissionByRoleIdQuery = new GetRolePermissionByRoleIdQuery { roleId = id };   
            var rolePermission = await _mediator.Send(getRolePermissionByRoleIdQuery, cancellationToken);

            GetUserByRoleIDQuery getUserByRoleIDQuery = new GetUserByRoleIDQuery { roleId = id};
            var user = await _mediator.Send(getUserByRoleIDQuery, cancellationToken);

            if (rolePermission != null || user != null)
            {
                TempData["ErrorMessage"] = "Role already mapped with User or Permission. so can not be delete.";


                return RedirectToAction("Index");
            }

            DeleteRoleCommand deleteRoleCommand = new DeleteRoleCommand
            {
                Id = id
            };

            var role = await _mediator.Send(deleteRoleCommand, cancellationToken);

            if (!role.IsDeleted)
            {
                TempData["ErrorMessage"] = "Process can't be completed";
                return View("Index");
            }


            var activityLogAttribute = new ActivityLogAttribute("Delete", "{0} Role and mapped permission deleted by {1}", role.Name + " (" + role.Id + ")");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);


            TempData["SuccessMessage"] = "Role deleted successfully.";
            return RedirectToAction("Index");
        }



        private async Task LoadPermission()
        {
            var permission = await _mediator.Send(new GetPermissionAsSelectListItemQuery());

            ViewBag.Permission = permission.ToDictionary(p => p.Value, p => p.Text);

        }

    }
}
