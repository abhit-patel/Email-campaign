using AutoMapper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmailCampaign.WebApplication.Controllers
{
    //[Authorize(Policy = "PermissionPolicy")]

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

        //[Authorize("ViewPermission")]
        public async Task<IActionResult> Index()
        {
            List<User> userList = await _userRepository.GetAllUserAsync();

            await LoadRoles();

            return View(userList);
        }


        //[Authorize("AddEditPermission")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            User user = await _userRepository.GetUserAsync(id);

            UserRegisterVM  userVM = _mapper.Map<UserRegisterVM>(user);

            await LoadRoles();

            return View("UpdateUser", userVM);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize("AddEditPermission")]
        //[CustomAuthorize("User/AddUser")]
        public async Task<IActionResult> AddUser()
        {
            await LoadRoles();

            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddUser")]
        //[Authorize("AddEditPermission")]
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateUser")]
        //[Authorize("AddEditPermission")]
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [ActionName("IsActiveToggle")]
        public async Task<IActionResult> IsActiveToggleAsync(string email)
        {
            var user = await _userRepository.ActiveToggleAsync(email);

            if(user == null)
            {
                TempData["Message"] = "";
                return View("AddUser");
            }

            return RedirectToAction("Index");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize("DeletePermission")]
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



        [ActionName("ViewProfile")]
        public async Task<IActionResult> GetUserProfileInfo()
        {
            User user = await _userRepository.GetUserInfoForProfile();

            await LoadRoles();

            if (user == null)
            {
                TempData["Message"] = "";
                return View("Index", "Home");
            }

            return View("UserProfileInfo", user);
        }


        public IActionResult SelectProfile()
        {
            return View();
        }

        [HttpPost]
        [ActionName("UpdateProfilePic")]
        public async Task<IActionResult> UpdateProfilePic(IFormFile profilePicture)
        {
            if (profilePicture == null)
            {
                return Content("File not selected");
            }
            var user = await _userRepository.UpdateProfilePic(profilePicture);

            if(user.ProfilePicture == null) {
                TempData["Message"] = "";
                return View("Index", "Home");
            }

            return RedirectToAction("ViewProfile");
        }


        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ActionName("UpdatePassword")]
        public async Task<IActionResult> ChangePassword(UpdatePasswordVM model)
        {
            var user = await _userRepository.ChangePasswordAsync(model.Password);

            if (user.ProfilePicture == null)
            {
                TempData["Message"] = "";
                return View("Index", "Home");
            }

            return View("UserProfileInfo", user);
        }

        [ActionName("UpdateProfile")]
        public async Task<IActionResult> UpdatePrfoile(ProfileVM model)
        {
            var user = await _userRepository.UpdateProfileAsync(model);

            if (user == null)
            {
                TempData["Message"] = "";
                return View("Index", "Home");
            }

            return View("UserProfileInfo", user);
        }


        private async Task LoadRoles()
        {
            var roles = await _roleRepository.GetRolesAsSelectListItemsAsync();

            ViewBag.Roles = roles.ToDictionary(r => r.Value, r => r.Text);
        }



    }
}
