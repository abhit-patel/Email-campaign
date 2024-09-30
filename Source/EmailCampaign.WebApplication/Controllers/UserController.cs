using AutoMapper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Query.QueryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmailCampaign.WebApplication.Controllers
{
    //[Authorize(Policy = "RequireAuthenticatedUser")]
    [ServiceFilter(typeof(PermissionFilter))]

    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserService _userService;

        public UserController(IUserRepository userRepository, IMapper mapper , IRoleRepository roleRepository, IUserService userService )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _userService = userService;
        }

        //[CustomAuthorize("User/Index")]
        //[HasPermission("View")]
        public async Task<IActionResult> Index()
        {
            List<User> userList = await _userRepository.GetAllUserAsync();

            await LoadRoles();

            return View(userList);
        }


        public async Task<IActionResult> GetUserById(Guid id)
        {
            User user = await _userRepository.GetUserAsync(id);

            UserRegisterVM  userVM = _mapper.Map<UserRegisterVM>(user);

            await LoadRoles();

            return View("UpdateUser", userVM);
        }

        [HttpGet]
        //[CustomAuthorize("User/AddUser")]
        public async Task<IActionResult> AddUser()
        {
            await LoadRoles();

            return View();
        }

        [HttpPost]
        [ActionName("AddUser")]
        //[HasPermission("AddEdit")]
        //[CustomAuthorize("User/AddUser")]
        public async Task<IActionResult> Post(UserRegisterVM model)
        {
            var user = await _userRepository.CreateUserAsync(model);

            if(user == null)
            {
                TempData["Message"] = "";
                return View("AddUser");
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ActionName("UpdateUser")]
        public async Task<IActionResult> Update(Guid id, UserRegisterVM userModel)
        {

            var user = await _userRepository.UpdateUserAsync(id, userModel);

            if(user == null)
            {
                TempData["Message"] = "";
                return View("AddUser");
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ActionName("IsActiveToggle")]

        public async Task<IActionResult> IsActiveToggleAsync(Guid userId)
        {
            var user = await _userRepository.ActiveToggleAsync(userId);

            if(user == null)
            {
                TempData["Message"] = "";
                return View("AddUser");
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _userRepository.DeleteUserAsync(id);

            if (!isDeleted)
            {
                TempData["Message"] = "";
                return View("Index");
            }
             return RedirectToAction("Index");
        }

        private async Task LoadRoles()
        {
            var roles = await _roleRepository.GetRolesAsSelectListItemsAsync();

            ViewBag.Roles = roles.ToDictionary(r => r.Value, r => r.Text);
        }


    }
}
