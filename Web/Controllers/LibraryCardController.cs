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
        [CustomAuthorize]
        [HttpGet]
        public async Task<IActionResult> AddLibraryCard()
        {
            var id = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(id);

            return View(user);
        }
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> AddLibraryCard(Guid userId)
        {
            var result = await _libraryCardService.CreateAsync(userId);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Description;
                return RedirectToAction("AccountDashboard", "User");
            }

            return RedirectToAction("AccountDashboard","User");
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllLibraryCards()
        {
            var libraryCards = await _libraryCardService.GetAllAsync();

            return View(libraryCards.Value);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateLibraryCard()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLibraryCard(UpdateLibraryCardDTO updateLibraryCardDTO)
        {
            var result =  await _libraryCardService.UpdateAsync(updateLibraryCardDTO);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Description;
                return RedirectToAction("UpdateLibraryCard");
            }

            return RedirectToAction("AccountDashboard", "User");
        }
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteLibraryCard(Guid userId)
        {
            var result = await _libraryCardService.DeleteAsync(userId);
            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Description;
            }
            return RedirectToAction("GetAllLibraryCards");
        }

    }
}
