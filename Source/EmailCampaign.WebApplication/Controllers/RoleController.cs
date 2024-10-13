using AutoMapper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
        public RoleController(IRoleRepository roleRepository, IMapper mapper, IPermissionRepository permissionRepository , IRolePermissionRepository rolePermissionRepository, IUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _userRepository = userRepository;
        }

        [Authorize("ViewPermission")]
        public async Task<IActionResult> Index()
        {
            List<Role> roleList = await _roleRepository.GetAllRoleAsync();
            return View(roleList);
        }

        [Authorize("AddEditPermission")]
        [ActionName("GetRoleByID")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            Role role = await _roleRepository.GetRoleByIdAsync(id);

            RoleVM roleVM = _mapper.Map<RoleVM>(role);

            return View("UpdateUser", roleVM);
        }

        [ActionName("GetRolePermission")]
        public async Task<IActionResult> GetRolePermission(Guid id)
        {
            RolePermissionVM rolePermissionModel = await _rolePermissionRepository.GetAllByIdAsync(id);

            await LoadPermission();

            return View("UpdateRolePermission", rolePermissionModel);
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

            return View(model);
        }

        [HttpPost]
        [ActionName("AddRolePermission")]
        [Authorize("AddEditPermission")]
        public async Task<IActionResult> Post(RolePermissionVM model)
        {
            //if (ModelState.IsValid) { return View("Index"); }

            var role = await _roleRepository.CreateRoleAsync(model.RoleName);

            if (role == null)
            {
                TempData["ErrorMessage"] = "Failed to create new Role, please try again.";
                return RedirectToAction("AddRole");
            }
            else
            {
                foreach (var permissions in model.PermissionList)
                {
                    RolePermissionDBVM dbModel = _mapper.Map<RolePermissionDBVM>(permissions);
                    dbModel.RoleId = role.Id;

                    var item = await _rolePermissionRepository.CreateAsync(dbModel);

                    if (item == null)
                    {
                        TempData["Message"] = "Mapping failed for role and permission , please try again.";
                        return RedirectToAction("AddRolePermission");
                    }
                }
            }

            var activityLogAttribute = new ActivityLogAttribute("Create", "New Role created  with name {0} and mapped permission. Created by {1}.", role.Name + " (" + role.Id + ")");
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
        public async Task<IActionResult> Update(RolePermissionVM model)
        {
            var role = await _roleRepository.GetRoleByNameAsync(model.RoleName);

            if (role == null)
            {
                TempData["Message"] = "Failed to update role permission mapping.";
                return RedirectToAction("index");
            }

            foreach (var permissions in model.PermissionList)
            {
                RolePermissionDBVM dbModel = _mapper.Map<RolePermissionDBVM>(permissions);
                dbModel.RoleId = role.Id;

                var item = await _rolePermissionRepository.UpdateAsync(dbModel);

                if (item == null)
                {
                    TempData["Message"] = "Failed to update role permission mapping.";   
                    return RedirectToAction("Index");
                }
            }


            var activityLogAttribute = new ActivityLogAttribute("Update", " {0} role name or mapped permission updated by {1}.", role.Name + " (" + role.Id + ")" );
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



        [Authorize("DeletePermission")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var rolePermission = await _rolePermissionRepository.GetItemByRoleID(id);
            var user = await _userRepository.GetItemByRoleIDAsync(id);

            if (rolePermission != null || user != null)
            {
                TempData["ErrorMessage"] = "Role already mapped with User or Permission. so can not be delete.";


                return RedirectToAction("Index");
            }

            var role = await _roleRepository.DeleteRoleAsync(id);

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
            var permission = await _permissionRepository.GetPermissionAsSelectListItemsAsync();

            ViewBag.Permission = permission.ToDictionary(p => p.Value, p => p.Text);

        }

    }
}
