using AutoMapper;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using EmailCampaign.Infrastructure.Data.Repositories;
using MediatR;
using EmailCampaign.Application.Features.Permission.Queries;
using EmailCampaign.Application.Features.Permission.Commands;
using EmailCampaign.Application.Features.User.Commands;
using EmailCampaign.Application.Features.RoleWithPermission.Queries;

namespace EmailCampaign.WebApplication.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public PermissionController(IPermissionRepository permissionRepository, IMapper mapper, IRolePermissionRepository rolePermissionRepository, IMediator mediator)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
            _rolePermissionRepository = rolePermissionRepository;
            _mediator = mediator;
        }

        [Authorize("ViewPermission")]
        public async Task<IActionResult> Index()
        {
            var permissionList = await _mediator.Send(new GetAllPermissionQuery());
            return View(permissionList);
        }

        //[Authorize("AddEditPermission")]
        public async Task<IActionResult> GetPermissionById(Guid id, CancellationToken cancellationToken)
        {
            GetPermissionByIdQuery query = new GetPermissionByIdQuery
            {
                Id = id
            };

            var permission = await _mediator.Send(query, cancellationToken);

            UpdatePermissionCommand updateUserCommand = _mapper.Map<UpdatePermissionCommand>(permission);

            //PermissionVM permissionVM = _mapper.Map<PermissionVM>(permission);

            return View("UpdatePermission", updateUserCommand);
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
        public async Task<IActionResult> AddPermission(CreatePermissionCommand model, CancellationToken cancellationToken)
        {
            //if (ModelState.IsValid) { return View("Index"); }

            var permission = await _mediator.Send(model, cancellationToken);

            if (permission == null)
            {
                TempData["ErrorMessage"] = "Permission create process failed !!";
                return View("AddPermission");
            }

            TempData["SuccessMessage"] = "Permission successfully created.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        [Authorize("AddEditPermission")]
        public async Task<IActionResult> Update(Guid id, UpdatePermissionCommand permissionModel, CancellationToken cancellationToken)
        {

            var permission = await _mediator.Send(permissionModel, cancellationToken);

            return RedirectToAction("Index");
        }


        [Authorize("DeletePermission")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            DeletePermissionCommand deletePermissionCommand = new DeletePermissionCommand
            {
                Id = id
            };

            GetRolePermissionByPermissionIdQuery getRolePermissionByPermissionIdQuery = new GetRolePermissionByPermissionIdQuery { permissionId = id };

            RolePermission rolePermission = await _mediator.Send(getRolePermissionByPermissionIdQuery, cancellationToken);

            if (rolePermission != null)
            {
                TempData["ErrorMessage"] = "Permission already mapped with RolePermission. so can not be delete.";
                return RedirectToAction("Index");
            }

            var permission = await _mediator.Send(deletePermissionCommand, cancellationToken);

            if (!permission.IsDeleted)
            {
                TempData["ErrorMessage"] = "Permission delete process failed.";
                return View("Index");
            }

            TempData["SuccessMessage"] = "Permission deleted successfully.";
            return RedirectToAction("Index");
        }


    }
}