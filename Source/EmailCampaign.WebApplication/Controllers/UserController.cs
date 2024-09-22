using Microsoft.AspNetCore.Mvc;

namespace EmailCampaign.WebApplication.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }
    }
}
