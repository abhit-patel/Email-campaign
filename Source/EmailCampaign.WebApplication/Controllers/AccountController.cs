using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.AspNetCore.Authorization;

namespace EmailCampaign.WebApplication.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IAuthRepository _authRepository;

        public AccountController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("UserLogin")]
        public async Task<IActionResult> login(string email, string password)
        {
            User userItem = await _authRepository.validateUser(email, password);

            if (userItem == null)
            {
                TempData["Message"] = "Invalid login attempt.";
                return RedirectToAction("Index");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userItem.ID.ToString()),
                new Claim(ClaimTypes.Name, userItem.Email),
                // Add additional claims based on user roles, permissions, etc. (optional)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var userPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(userPrincipal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddHours(24) // Set a reasonable expiration for the cookie (optional)
            });

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ActionName("UserLogout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index" );
        }

    }
}
