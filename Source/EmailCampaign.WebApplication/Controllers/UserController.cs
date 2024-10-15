using AutoMapper;
using EmailCampaign.Application.Features.RoleWithPermission.Queries;
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

        public UserController(IUserRepository userRepository, IMapper mapper, IRoleRepository roleRepository, IUserService userService, IMediator mediator)
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

            UpdateUserCommand userVM = _mapper.Map<UpdateUserCommand>(user);

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

            CheckRegisteredEmailQuery checkRegisteredEmailQuery = new CheckRegisteredEmailQuery { email = model.Email };
            var IsEmailPresent = await _mediator.Send(checkRegisteredEmailQuery, cancellationToken);

            if (IsEmailPresent)
            {
                TempData["ErrorMessage"] = "User Email is already present, please use another email.";
                return RedirectToAction("AddUser");
            }

            //CreateUserCommand finalModel = _mapper.Map<CreateUserCommand>(model);
            var user = await _mediator.Send(model, cancellationToken);

            //var user = await _userRepository.CreateUserAsync(model);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User create process failed, please try again.";
                return RedirectToAction("AddUser");
            }


            var activityLogAttribute = new ActivityLogAttribute("Create", "New user created with name {0} Created by {1}", user.FirstName + " " + user.LastName + " (" + user.ID + ").");
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
            if (userModel.Id != id)
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
        public async Task<IActionResult> IsActiveToggleAsync(string email, CancellationToken cancellationToken)
        {
            ContactActiveToggleCommand updateActiveToggle = new ContactActiveToggleCommand
            {
                email = email
            };

            var user = await _mediator.Send(updateActiveToggle, cancellationToken);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User status has not been toggled successfully.";
                return RedirectToAction("AddUser");
            }


            var activityLogAttribute = new ActivityLogAttribute("IsActive toggle", "Active status changed of user {0}. Updated by {1}. ", user.FirstName + " " + user.LastName + " (" + user.ID + ") to Active status " + user.IsActive + " ");
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
            User user = await _mediator.Send(new GetUserInfoForProfileQuery());

            await LoadRoles();

            if (user == null)
            {
                TempData["Message"] = "";
                return RedirectToAction("Index", "Home");
            }

            return View("UserProfileInfo", user);
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


        public async Task<IActionResult> GetProfileForUpdate(Guid id, CancellationToken cancellationToken)
        {
            GetUserByIdQuery getUserByIdQuery = new GetUserByIdQuery { Id = id };
            User user = await _mediator.Send(getUserByIdQuery, cancellationToken);

            //UpdateUserProfileInfoCommand model = _mapper.Map<UpdateUserProfileInfoCommand>(user);

            UpdateUserProfileInfoCommand model = new UpdateUserProfileInfoCommand
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Birthdate = user.BirthDate
            };


            return View("UpdateProfileInfo", model);
        }


        [HttpPost]
        [ActionName("UpdateProfileInfo")]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileInfoCommand model, CancellationToken cancellationToken)
        {

            var user = await _mediator.Send(model, cancellationToken);

            if (user == null)
            {
                TempData["ErrorMessage"] = "";
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
            var roles = await _mediator.Send(new GetRoleAsSelectListItemQuery());

            ViewBag.Roles = roles.ToDictionary(r => r.Value, r => r.Text);
        }



    }
}