using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EmailCampaign.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using EmailCampaign.Infrastructure.Data.Services;

namespace EmailCampaign.WebApplication.Controllers
{
    //[Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public RoleController(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        //[CustomAuthorize("Role/Index")]
        public async Task<IActionResult> Index()
        {
            List<Role> roleList = await _roleRepository.GetAllRoleAsync();
            return View(roleList);
        }


        public async Task<IActionResult> GetRoleById(Guid id)
        {
            Role role = await _roleRepository.GetRoleAsync(id);

            RoleVM roleVM = _mapper.Map<RoleVM>(role);

            return View("UpdateUser", roleVM);
        }

        [HttpGet]
        //[CustomAuthorize("Role/AddRole")]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AddRole")]
        //[CustomAuthorize("Role/AddRole")]
        public async Task<IActionResult> Post(RoleVM model)
        {
            //if (ModelState.IsValid) { return View("Index"); }

            var role = await _roleRepository.CreateRoleAsync(model);

            if (role == null)
            {
                TempData["Message"] = "";
                return View("AddRole");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var isdeleteed = await _roleRepository.DeleteRoleAsync(id);

            if (!isdeleteed)
            {
                TempData["Message"] = "";
                return View("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
