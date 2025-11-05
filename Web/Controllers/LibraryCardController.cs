using Application.DTOs.LibraryCards;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Filters;

namespace Web.Controllers
{
    public class LibraryCardController : Controller
    {
        private readonly ILibraryCardService _libraryCardService;
        private readonly UserManager<ApplicationUser> _userManager;

        public LibraryCardController(ILibraryCardService libraryCardService,UserManager<ApplicationUser> userManager)
        {
            _libraryCardService = libraryCardService;
            _userManager = userManager;
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> CreateLibraryCard()
        {
            var id = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(id);

            return View(user);
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateLibraryCard(Guid userId)
        {
            await _libraryCardService.CreateAsync(userId);

            return RedirectToAction("GetAllLibraryCards");
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult GetAllLibraryCards()
        {
            return View();
        }
        [HttpGet]
        public IActionResult UpdateLibraryCard()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLibraryCard(UpdateLibraryCardDTO updateLibraryCardDTO)
        {
            await _libraryCardService.UpdateAsync(updateLibraryCardDTO);
            return RedirectToAction("GetAllLibraryCards");
        }
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteLibraryCard(Guid userId)
        {
            await _libraryCardService.DeleteAsync(userId);
            return View();
        }

    }
}
