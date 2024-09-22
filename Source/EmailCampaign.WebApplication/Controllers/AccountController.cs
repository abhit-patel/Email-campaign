using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmailCampaign.WebApplication.Controllers
{
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

        public async Task<IActionResult> login(string email, string password)
        {
            var isValid = await _authRepository.validateUser(email, password);

            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }

            return RedirectToAction("Index", "Home");  
        }


    }
}
