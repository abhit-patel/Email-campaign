using EmailCampaign.Domain.Interfaces.Log;
using Microsoft.AspNetCore.Mvc;

namespace EmailCampaign.WebApplication.Controllers
{
    public class LogController : Controller
    {
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public LogController(IErrorLogRepository errorLogRepository, IActivityLogRepository activityLogRepository)
        {
            _errorLogRepository = errorLogRepository;
            _activityLogRepository = activityLogRepository;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ActivityLog()
        {
            var logs = _activityLogRepository.GetActivityLog();

            return View(logs);
        }


        public IActionResult ErrorLog()
        {
            var logs = _errorLogRepository.GetErrorLog();

            return View(logs);
        }
    }
}
