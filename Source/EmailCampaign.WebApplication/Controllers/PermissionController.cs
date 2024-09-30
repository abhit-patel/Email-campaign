using AutoMapper;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using EmailCampaign.Infrastructure.Data.Repositories;

namespace EmailCampaign.WebApplication.Controllers
{
    //[Authorize]
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        public PermissionController(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<Permission> roleList = await _permissionRepository.GetAllPermissionAsync();
            return View(roleList);
        }


        public async Task<IActionResult> GetPermissionById(Guid id)
        {
            Permission permission = await _permissionRepository.GetPermissionAsync(id);

            PermissionVM permissionVM = _mapper.Map<PermissionVM>(permission);

            return View("UpdatePermission", permissionVM);
        }

        [HttpGet]
        public IActionResult AddPermission()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AddPermission")]
        public async Task<IActionResult> AddPermission(PermissionVM model)
        {
            //if (ModelState.IsValid) { return View("Index"); }

            var permission = await _permissionRepository.CreatePermissionAsync(model);

            if (permission == null)
            {
                TempData["Message"] = "";
                return View("AddPermission");
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Update(Guid id, PermissionVM permissionModel)
        {

            var permission = await _permissionRepository.UpdatePermissionAsync(id, permissionModel);

            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _permissionRepository.DeletePermissionAsync(id);

            if (!isDeleted)
            {
                TempData["Message"] = "";
                return View("Index");
            }
            return RedirectToAction("Index");
        }


    }
}
