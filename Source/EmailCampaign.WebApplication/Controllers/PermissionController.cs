using AutoMapper;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using EmailCampaign.Infrastructure.Data.Repositories;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmailCampaign.WebApplication.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IMapper _mapper;
        public PermissionController(IPermissionRepository permissionRepository, IMapper mapper, IRolePermissionRepository rolePermissionRepository)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
            _rolePermissionRepository = rolePermissionRepository;
        }

        [Authorize("ViewPermission")]
        public async Task<IActionResult> Index()
        {
            List<Permission> roleList = await _permissionRepository.GetAllPermissionAsync();
            return View(roleList);
        }

        //[Authorize("AddEditPermission")]
        public async Task<IActionResult> GetPermissionById(Guid id)
        {
            Permission permission = await _permissionRepository.GetPermissionAsync(id);

            PermissionVM permissionVM = _mapper.Map<PermissionVM>(permission);

            return View("UpdatePermission", permissionVM);
        }

        [HttpGet]
        [Authorize("AddEditPermission")]
        public IActionResult AddPermission()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AddPermission")]
        [Authorize("AddEditPermission")]
        public async Task<IActionResult> AddPermission(PermissionVM model)
        {
            //if (ModelState.IsValid) { return View("Index"); }

            var permission = await _permissionRepository.CreatePermissionAsync(model);

            if (permission == null)
            {
                TempData["ErrorMessage"] = "Permission create process failed !!";
                return View("AddPermission");
            }

            var activityLogAttribute = new ActivityLogAttribute("Create", "New Permission created with name {0} Created by {1}", permission.ControllerName +  " (" + permission.Id + ")");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);


            TempData["SuccessMessage"] = "Permission successfully created.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        [Authorize("AddEditPermission")]
        public async Task<IActionResult> Update(Guid id, PermissionVM permissionModel)
        {

            var permission = await _permissionRepository.UpdatePermissionAsync(id, permissionModel);

            return RedirectToAction("Index");
        }


        [Authorize("DeletePermission")]
        public async Task<IActionResult> Delete(Guid id)
        {
            RolePermission rolePermission = await _rolePermissionRepository.GetItemByPermissionId(id);

            if(rolePermission != null)
            {
                TempData["ErrorMessage"] = "Permission already mapped with RolePermission. so can not be delete.";
                return RedirectToAction("Index");
            }

            var permission = await _permissionRepository.DeletePermissionAsync(id);

            if (permission == null)
            {
                TempData["ErrorMessage"] = "Permission delete process failed.";
                return View("Index");
            }

            var activityLogAttribute = new ActivityLogAttribute("Delete", "Permission detalis Updated with name {0} Created by {1}", permission.ControllerName + " (" + permission.Id + ")");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);

            TempData["SuccessMessage"] = "Permission deleted successfully.";
            return RedirectToAction("Index");
        }


    }
}
