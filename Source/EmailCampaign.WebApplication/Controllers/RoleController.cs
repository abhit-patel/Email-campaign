using AutoMapper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace EmailCampaign.WebApplication.Controllers
{
    //[Microsoft.AspNetCore.Authorization.Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        public RoleController(IRoleRepository roleRepository, IMapper mapper, IPermissionRepository permissionRepository , IRolePermissionRepository rolePermissionRepository)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
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

            //var model = new RolePermissionVM
            //{
            //    PermissionList = new List<PermissionListVM>()
            //};

            //foreach (var permission in ViewBag.Permission)
            //{
            //    model.PermissionList.Add(new PermissionListVM { PermissionId = Guid.Parse(permission.Key) });
            //}

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
                TempData["Message"] = "";
                return View("AddRole");
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
                        TempData["Message"] = "";
                        return View("AddRolePermission");
                    }
                }
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        [ActionName("UpdateRolePermission")]
        public async Task<IActionResult> Update(RolePermissionVM model)
        {
            var role = await _roleRepository.GetRoleByNameAsync(model.RoleName);

            if (role == null)
            {
                TempData["Message"] = "";
                return View("index");
            }

            foreach (var permissions in model.PermissionList)
            {
                RolePermissionDBVM dbModel = _mapper.Map<RolePermissionDBVM>(permissions);
                dbModel.RoleId = role.Id;

                var item = await _rolePermissionRepository.UpdateAsync(dbModel);

                if (item == null)
                {
                    TempData["Message"] = "";   
                    return View("Index");
                }
            }

            return RedirectToAction("Index","RolePermission");
        }


        [ActionName("IsActiveToggle")]
        public async Task<IActionResult> IsActiveToggleAsync(string name)
        {
            var user = ""; //await _userRepository.ActiveToggleAsync(email);

            if (user == null)
            {
                TempData["Message"] = "";
                return View("AddUser");
            }

            return RedirectToAction("Index");
        }


        [Authorize("DeletePermission")]
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


        private async Task LoadPermission()
        {
            var permission = await _permissionRepository.GetPermissionAsSelectListItemsAsync();

            ViewBag.Permission = permission.ToDictionary(p => p.Value, p => p.Text);

        }

    }
}
