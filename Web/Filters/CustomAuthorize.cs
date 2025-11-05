using Application.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Filters
{
    public class CustomAuthorizeAttribute : Attribute, IActionFilter
    {
        public CustomAuthorizeAttribute()
        {
        }
        public CustomAuthorizeAttribute(string policy)
        {
            Policy = policy;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated != true)
            {
                context.HttpContext.Items["AlertMessage"] = "You must be logged in to perform this action.";

                // Instead of redirecting, re-render the same view if possible
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    controller.TempData["AlertMessage"] = "You must be logged in to perform this action.";

                    context.Result = new RedirectToActionResult("GetAllBooks", "Book", null);
                }
            }
            else if (!string.IsNullOrEmpty(Policy))
            {
                // Here you would typically check the user's roles/claims against the policy
                // For demonstration, let's assume a simple role check
                if (Policy == "AdminOnly" && !user.IsInRole("Admin"))
                {
                    context.HttpContext.Items["AlertMessage"] = "You do not have permission to perform this action.";
                    var controller = context.Controller as Controller;
                    if (controller != null)
                    {
                        controller.TempData["AlertMessage"] = "You do not have permission to perform this action.";
                        context.Result = new RedirectToActionResult("GetAllBooks", "Book", null);
                    }
                }
            }
        }
        

        /// <summary>
        /// Gets or sets the policy name that determines access to the resource.
        /// </summary>
        public string? Policy { get; set; }
    }
}
