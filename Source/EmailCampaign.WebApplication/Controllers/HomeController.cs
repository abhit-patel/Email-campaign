using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Interfaces.Log;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using EmailCampaign.Query.QueryService;
using EmailCampaign.WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Web.Http;

namespace EmailCampaign.WebApplication.Controllers
{
    //[LogActivity]
    //[ServiceFilter(typeof(ActivityLogAttribute))]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        

        public HomeController(ILogger<HomeController> logger, INotificationRepository notificationRepository)
        {
            _logger = logger;
            
        }

        public async Task<IActionResult> Index()
        {
            //List<Notification> item = await _notificationRepository.GetNotificationForIconAsync();

            //ViewBag.Notifications = item.Select(n => new { n.Id, n.Header }).ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
