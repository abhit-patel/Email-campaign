using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmailCampaign.WebApplication.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }


        public async Task<IActionResult> Index()
        {
            List<Notification> notification = await _notificationRepository.GetAllNotificationAsync(); 

            return View(notification);
        }

        public async Task<IActionResult> UpdateNotification(Guid id)
        {
            var notification = await _notificationRepository.UpdateNotificationAsync(id);

            if (notification == null)
            {
                TempData["ErrorMessage"] = "";
                return RedirectToAction("Index", "Home");
            }


            return RedirectToAction("Index");
        }

    }
}
