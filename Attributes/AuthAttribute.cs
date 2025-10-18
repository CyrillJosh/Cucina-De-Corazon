using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Cucina_De_Corazon.Attributes
{
    public class AuthAttribute : ActionFilterAttribute
    {
        private string[]? Role;

        public AuthAttribute(string role)
        {
            Role = role?.Split(',');
        }

        public override void OnActionExecuting(ActionExecutingContext context)
            {
            var httpContext = context.HttpContext;
            //Get Session
            var user = httpContext.Session.GetString("User");
            var role = httpContext.Session.GetString("Role");
            //var user = "user";
            //var role = "Manager";
            if (string.IsNullOrEmpty(user) || role.Length == 0 || role == null || !Role.Contains(role))
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
            }
        }
    }
}
