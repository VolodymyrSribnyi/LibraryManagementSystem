using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> AdminDashboard()
        {
            return View("AdminDashboard");
        }
    }
}
