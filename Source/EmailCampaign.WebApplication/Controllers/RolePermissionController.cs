using AutoMapper;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EmailCampaign.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using EmailCampaign.Application.Features.RoleWithPermission.Queries;

namespace EmailCampaign.WebApplication.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class RolePermissionController : Controller
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public RolePermissionController(IRolePermissionRepository rolePermissionRepository, IMapper mapper, IRoleRepository roleRepository, IPermissionRepository permissionRepository, IMediator mediator)
        {
            _mapper = mapper;
            _rolePermissionRepository = rolePermissionRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _mediator = mediator;
        }

        [Authorize("ViewPermission")]
        public async Task<IActionResult> Index()
        {
            var roleList = await _mediator.Send(new GetAllRolePermissionQuery());

            await LoadRolePermission();
            
            return View(roleList);
        }

        /*** not used for now 
        [Authorize("AddEditPermission")]
        public async Task<IActionResult> GetById(Guid id)
        {
            RolePermission item = await _rolePermissionRepository.GetByIdAsync(id);

            RolePermissionVM roleVM = _mapper.Map<RolePermissionVM>(item);

            await LoadRolePermission();

            return View("UpdateRolePermission", roleVM);
        }

        [HttpGet]
        [Authorize("AddEditPermission")]
        public async Task<IActionResult> AddRolePermission()
        {
            await LoadRolePermission();
            
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
        [Authorize("AddEditPermission")]
        [ActionName("AddRolePermission")]
        public async Task<IActionResult> Post(RolePermissionVM model)
        {
            if (ModelState.IsValid) { return View("Index"); }

            foreach (var permissions in model.PermissionList)
            {
                RolePermissionDBVM dbModel = _mapper.Map<RolePermissionDBVM>(permissions);
                dbModel.RoleId = model.RoleId;

                var item = await _rolePermissionRepository.CreateAsync(dbModel);

                if (item == null)
                {
                    TempData["Message"] = "";
                    return View("AddRolePermission");
                }
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Update(Guid id, RolePermissionDBVM rolePermissionVM)
        {

            await _rolePermissionRepository.UpdateAsync(rolePermissionVM);

            return RedirectToAction("Index");
        }
        */


        //[Authorize("DeletePermission")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var isDeleted = await _rolePermissionRepository.DeleteAsync(id);

        //    if (!isDeleted)
        //    {
        //        TempData["ErrorMessage"] = "Failed to delete permission mapping.";
        //        return RedirectToAction("Index");
        //    }

        //    TempData["SuccessMessage"] = "Permission mapping delete successfully.";
        //    return RedirectToAction("Index");
        //}

        private async Task LoadRolePermission()
        {

            var roles = await _mediator.Send(new GetRoleAsSelectListItemQuery());
            var permission = await _mediator.Send(new GetPermissionAsSelectListItemQuery());

            ViewBag.Roles = roles.ToDictionary(r => r.Value, r => r.Text);
            ViewBag.Permission = permission.ToDictionary(p => p.Value, p => p.Text);

            //ViewBag.Roles = new SelectList(roles, "Value", "Text");
            //ViewBag.Permission = new SelectList(permission, "Value", "Text");
        }


    }
}
