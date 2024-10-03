using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Query.QueryService
{
    public static class ControllerNameManager
    {
        private const string ControllerNamesKey = "CustomControllerNames";

        public static void SetCustomControllerName(this IHttpContextAccessor httpContextAccessor, string key, string name)
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null) return;

            context.GetRouteData().Values["controller"] = "User";

            //var controllerNames = context.Items[ControllerNamesKey] as Dictionary<string, string>
            //    ?? new Dictionary<string, string>();
            //controllerNames[key] = name;
            //context.Items[ControllerNamesKey] = controllerNames;
        }

    }
}
