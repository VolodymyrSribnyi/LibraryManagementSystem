using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Filters;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> AdminDashboard()
        {
            return View("AdminDashboard");
        }
    }
}
