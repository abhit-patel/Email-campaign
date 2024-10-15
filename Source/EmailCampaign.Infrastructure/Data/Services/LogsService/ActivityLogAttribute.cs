using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces.Log;
using EmailCampaign.Domain.Services;
using EmailCampaign.Infrastructure.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EmailCampaign.Infrastructure.Data.Services.LogsService
{
    public class ActivityLogAttribute : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute
    {

        public string _activityType { get; }
        public string _message { get; }
        public string _additionalData { get; }


        public ActivityLogAttribute(string activityType, string message, string additionalData)
        {
            _activityType = activityType;
            _message = message;
            _additionalData = additionalData;

        }


        public async Task LogActivityAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {

            var _logRepository = (IActivityLogRepository)context.HttpContext.RequestServices.GetService(typeof(IActivityLogRepository));
            var _userContextService = (IUserContextService)context.HttpContext.RequestServices.GetService(typeof(IUserContextService));

            var currentUserEmail = _userContextService.GetUserEmail().ToString();
            var currentUserName = _userContextService.GetUserName().ToString();
            var currentUserId = _userContextService.GetUserId().ToString();

            var activityLog = new ActivityLog
            {
                Id = Guid.NewGuid(),
                Type = _activityType,
                EntityType = context.Controller.GetType().Name,
                Message = string.Format(_message, _additionalData, currentUserName + " (" + currentUserId + ")"),
                Email = currentUserEmail,
                UserId = Guid.Parse(currentUserId),
                Slug = context.RouteData.Values["controller"].ToString() + "/" + context.RouteData.Values["action"].ToString(),
                IPAddress = context.HttpContext.Connection.LocalIpAddress.ToString(),
                Device = "",
                AddedOn = DateTime.UtcNow,

            };

            await _logRepository.LogActivityAsync(activityLog);


        }

    }
}




















//    public class ActivityLogAttribute : Attribute, IAsyncActionFilter
////Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute
//{
//    //private readonly IActivityLogRepository _logRepository;
//    //private readonly IUserContextService _userContextService;
//    public string _type { get; set; }
//    public string _Message { get; set; }

//    public ActivityLogAttribute( string Type, string Message)
//    {

//        _type = Type;
//        _Message = Message;
//    }

//    public async Task OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context, ActionExecutionDelegate next)
//    {
//        var result = await next();

//        if ( result.Exception == null)
//        {
//            var logRepository = (IActivityLogRepository)context.HttpContext.RequestServices.GetService(typeof(IActivityLogRepository));
//            var userContextService = (IUserContextService)context.HttpContext.RequestServices.GetService(typeof(IUserContextService));


//            var activityLog = new ActivityLog
//            {
//                //Type = context.ActionDescriptor.DisplayName,
//                Id = Guid.NewGuid(),
//                Type = _type,
//                EntityType = context.Controller.GetType().Name,
//                Message = _Message,
//                Email = userContextService.GetUserEmail().ToString(),
//                UserId = Guid.Parse(userContextService.GetUserId()),
//                Slug = context.RouteData.Values["controller"].ToString() + "/" + context.RouteData.Values["action"].ToString(),
//                IPAddress=context.HttpContext.Connection.LocalIpAddress.ToString(),
//                Device ="" ,
//                AddedOn = DateTime.UtcNow,                    

//            };

//            await logRepository.LogActivityAsync(activityLog);
//        }

//        await next();
//    }
//}

