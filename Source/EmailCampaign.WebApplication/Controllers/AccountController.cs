using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using EmailCampaign.Core.SharedKernel;
using System.Text.Encodings.Web;

namespace EmailCampaign.WebApplication.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IAuthRepository _authRepository;
        private readonly EmailService _emailService ;
        private readonly IUserRepository _userRepository;


        public AccountController(IAuthRepository authRepository , IUserRepository userRepository, EmailService emailService)
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            LoginVM loginVM = new LoginVM();

            if (Request.Cookies["RememberMeEmail"] != null && Request.Cookies["RememberMePassword"] != null)
            {
                loginVM.Email = Request.Cookies["RememberMeEmail"];
                loginVM.Password = Request.Cookies["RememberMePassword"];
                loginVM.RememberMe = true;
            }

            return View(loginVM);
        }

        [HttpPost]
        [ActionName("UserLogin")]
        public async Task<IActionResult> login(LoginVM loginVM)
        {
            User userItem = await _authRepository.validateUser(loginVM.Email, loginVM.Password);

            if (userItem == null)
            {
                TempData["ErrorMessage"] = "Invalid login attempt.";
                return RedirectToAction("Index");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userItem.ID.ToString()),
                new Claim(ClaimTypes.Email, userItem.Email),
                new Claim(ClaimTypes.Name, userItem.FirstName + " " + userItem.LastName),
                // Add additional claims.
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(userPrincipal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc =  loginVM.RememberMe? DateTime.UtcNow.AddDays(5) : DateTime.UtcNow.AddHours(1) // Set expiration for the cookie
            });


            // for save in cookies
            if (loginVM.RememberMe)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Append("RememberMeEmail", loginVM.Email, cookieOptions);
                Response.Cookies.Append("RememberMePassword", loginVM.Password, cookieOptions);
            }
            else
            {
                Response.Cookies.Delete("RememberMeEmail");
                Response.Cookies.Delete("RememberMePassword");
            }


            //
            HttpContext.Session.SetString("ProfilePicturePath", userItem.ProfilePicture);


            TempData["SuccessMessage"] = "User successfully logged in";
            return RedirectToAction("Index", "Home");
        }


        [ActionName("UserLogout")]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync();
            HttpContext.Session.Clear();

            //Response.Cookies.Delete("RememberMeEmail");
            //Response.Cookies.Delete("RememberMePassword");

            TempData["SuccessMessage"] = "User logout successfully.";
            return RedirectToAction("Index" );
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVm)
        {
            if (ModelState.IsValid)
            {
                User user = await _userRepository.GetUserByEmailAsync(forgotPasswordVm.Email);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found for your entered mail id.";
                    return View("ForgotPassword");
                }

                var token = Guid.NewGuid().ToString();

                await _userRepository.SavePasswordResetTokenAsync(user.Email, token);



                var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, protocol: Request.Scheme);;

                await _emailService.SendEmailAsync(user.Email, "Reset password", $"Please reset your password by '{callbackUrl}' clicking here.");


                TempData["SuccessMessage"] = "Mail sent successfully to reset password.";
                return RedirectToAction("Index","Account");
            }

            return View("ForgotPassword");
        }


        //[FromQuery]
        public IActionResult ResetPassword( string token = null , string email = null )
        {
            return View (new ResetPasswordVM { Token = token, Email = email });
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = await _userRepository.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found for your entered mail id.";
                return View("ResetPassword");
            }

            var isValidToken = await _userRepository.ValidatePasswordResetTokenAsync(user.Email, model.Token);

            if (!isValidToken)
            {
                TempData["ErrorMessage"] = "Password rest process failed. Token not matched.";
                return View("ResetPassword");
            }

            var updatedUser = await _userRepository.ResetPasswordAsync(user.Email, model.Password);


            if (updatedUser != null)
            {
                TempData["SuccessMessage"] = "Password reset successfully.";
                return RedirectToAction("Index", "Account");
            }

            TempData["ErrorMessage"] = "Password rest process failed. Please try again.";
            return View("ResetPassword");
        }

    }
}
