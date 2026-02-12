using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Diagnostics;

namespace Cucina_De_Corazon.Attributes
{
    public class AuthAttribute : ActionFilterAttribute
    {
        private string[]? Roles;

        public AuthAttribute(string role)
        {
            Roles = role?.Split(',');
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var role = httpContext.Session.GetString("Role");
            var actionname = context.RouteData.Values["action"]?.ToString();

            if (/*Roles.Any(x => x == "")*/ string.IsNullOrEmpty(role) && actionname != "Login")
            {
                var tempData = context.HttpContext.RequestServices
                    .GetService(typeof(Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory))
                    as Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory;

                var tempDataDictionary = tempData?.GetTempData(httpContext);
                tempDataDictionary["AuthMessage"] = "You need to log in to access this page.";
                context.Result = new RedirectToActionResult("Login", "User", null);
                return;
            }
            if (!Roles.Contains(role)
                && actionname != "Login")
            {
                // Store message using TempData
                var tempData = context.HttpContext.RequestServices
                    .GetService(typeof(Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory))
                    as Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory;

                var tempDataDictionary = tempData?.GetTempData(httpContext);
                tempDataDictionary["AuthMessage"] = "You are not authorized to access this page.";

                // Get previous page
                var referer = httpContext.Request.Headers["Referer"].ToString();

                if (!string.IsNullOrEmpty(referer))
                {
                    context.Result = new RedirectResult(referer);
                }
                else
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }
            }
        }
    }
}
