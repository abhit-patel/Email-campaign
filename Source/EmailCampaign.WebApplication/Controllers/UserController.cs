using AutoMapper;
using EmailCampaign.Application.Features.User.Commands;
using EmailCampaign.Application.Features.User.Queries;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Utils;
using System.Data;
using System.Reflection;
using System.Threading;

namespace EmailCampaign.WebApplication.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    //[ServiceFilter(typeof(ActivityLogAttribute))]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserService _userService;
        private readonly IMediator _mediator;

        public UserController(IUserRepository userRepository, IMapper mapper , IRoleRepository roleRepository, IUserService userService, IMediator mediator )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _userService = userService;
            _mediator = mediator;
        }

        [Authorize("ViewPermission")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userList = await _mediator.Send(new GetAllUserQuery());

            await LoadRoles();

            return View(userList);
        }


        [Authorize("AddEditPermission")]
        public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
        {
            GetUserByIdQuery getUserByIdQuery = new GetUserByIdQuery
            {
                Id = id
            };

            var user = await _mediator.Send(getUserByIdQuery, cancellationToken);

            UpdateUserCommand  userVM = _mapper.Map<UpdateUserCommand>(user);

            await LoadRoles();

            return View("UpdateUser", userVM);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize("AddEditPermission")]
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
        [Authorize("AddEditPermission")]
        public async Task<IActionResult> Post(CreateUserCommand model, CancellationToken cancellationToken)
        {
            var IsEmailPresent = await _userRepository.CheckRegisteredEmailAsync(model.Email);

            if (IsEmailPresent)
            {
                TempData["ErrorMessage"] = "User Email is already present, please use another email.";
                return RedirectToAction("AddUser");
            }

            //CreateUserCommand finalModel = _mapper.Map<CreateUserCommand>(model);
            var user = await _mediator.Send(model, cancellationToken);

            //var user = await _userRepository.CreateUserAsync(model);

            if(user == null)
            {
                TempData["ErrorMessage"] = "User create process failed, please try again.";
                return RedirectToAction("AddUser");
            }


            var activityLogAttribute = new ActivityLogAttribute("Create", "New user created with name {0} Created by {1}" , user.FirstName + " " + user.LastName + " (" + user.ID + ").");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);


            TempData["SuccessMessage"] = "User created successfully..";
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
        [Authorize("AddEditPermission")]
        public async Task<IActionResult> Update(Guid id, UpdateUserCommand userModel, CancellationToken cancellationToken)
        {
            if(userModel.Id != id)
            {
                TempData["ErrorMessage"] = "User details data binding problem. please try again.";
                return RedirectToAction("AddUser");
            }

            var user = await _mediator.Send(userModel, cancellationToken);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User details has not be updated. please try again.";
                return RedirectToAction("AddUser");
            }


            var activityLogAttribute = new ActivityLogAttribute("Update", "Details updated of user {0}. Updated by {1}. ", user.FirstName + " " + user.LastName + " (" + user.ID + ")");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);



            TempData["SuccessMessage"] = "User details updated successfully.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [ActionName("IsActiveToggle")]
        public async Task<IActionResult> IsActiveToggleAsync(string email , CancellationToken cancellationToken)
        {
            UpdateActiveToggleCommand updateActiveToggle = new UpdateActiveToggleCommand
            {
                email = email
            };

            var user = await _mediator.Send(updateActiveToggle, cancellationToken);

            if(user == null)
            {
                TempData["ErrorMessage"] = "User status has not been toggled successfully.";
                return RedirectToAction("AddUser");
            }


            var activityLogAttribute = new ActivityLogAttribute("IsActive toggle", "Active status changed of user {0}. Updated by {1}. ", user.FirstName + " " + user.LastName + " (" + user.ID + ") to Active status " + user.IsActive +" ");
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize("DeletePermission")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {

            DeleteUserCommand deleteUserCommand = new DeleteUserCommand
            {
                Id = id,
            };

            var user = await _mediator.Send(deleteUserCommand, cancellationToken);


            if (!user.IsDeleted)
            {
                TempData["ErrorMessage"] = "User not deleted successfully.";
                return RedirectToAction("Index");
            }

            var activityLogAttribute = new ActivityLogAttribute("Delete", "{0} User deleted. Deleted by {1}. ", user.FirstName + " " + user.LastName + " (" + user.ID + ")");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);

            TempData["SuccessMessage"] = "User deleted successfully.";
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
                return RedirectToAction("Index", "Home");
            }

            return View("UserProfileInfo", user);
        }


        public IActionResult SelectProfile()
        {
            return View();
        }


        [HttpPost]
        [ActionName("UpdateProfilePic")]
        public async Task<IActionResult> UpdateProfilePic(ProfilePictureVM model)
        {
            if (model.ProfilePicture == null)
            {
                return Content("File not selected");
            }
            var user = await _userRepository.UpdateProfilePic(model.ProfilePicture);

            if(user.ProfilePicture == null) {
                TempData["Message"] = "";
                return View("Index", "Home");
            }

            var activityLogAttribute = new ActivityLogAttribute("Updated", "{0} user update profile picture. Updated by {1}.", user.FirstName + " " + user.LastName + " (" + user.ID + ")");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);

            return RedirectToAction("ViewProfile");
        }


        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        [ActionName("UpdatePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand model, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(model, cancellationToken);   

            if (user.ProfilePicture == null)
            {
                TempData["Message"] = "";
                return RedirectToAction("Index", "Home");
            }


            var activityLogAttribute = new ActivityLogAttribute("Password Change", "{0}  Changed Account password. Update By {1}. ", user.FirstName + " " + user.LastName + " (" + user.ID + ")");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);


            return View("UserProfileInfo", user);
        }


        public async Task<IActionResult> GetProfileForUpdate(Guid id)
        {
            User user = await _userRepository.GetUserAsync(id);

            ProfileVM model = _mapper.Map<ProfileVM>(user);

            return View("UpdateProfileInfo", model);
        }


        [HttpPost]
        [ActionName("UpdateProfileInfo")]
        public async Task<IActionResult> UpdateProfile(ProfileVM model)
        {
            var user = await _userRepository.UpdateProfileAsync(model);

            if (user == null)
            {
                TempData["Message"] = "";
                return RedirectToAction("Index", "Home");
            }


            var activityLogAttribute = new ActivityLogAttribute("Update", "{0} User update own profile details. Updated by {1}", user.FirstName + " " + user.LastName + " (" + user.ID + ")");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);

            await LoadRoles();

            return View("UserProfileInfo", user);
        }


        private async Task LoadRoles()
        {
            var roles = await _roleRepository.GetRolesAsSelectListItemsAsync();

            ViewBag.Roles = roles.ToDictionary(r => r.Value, r => r.Text);
        }



    }
}
