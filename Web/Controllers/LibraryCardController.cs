using Application.DTOs.LibraryCards;
using Application.DTOs.Users;
using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Services;
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
        private readonly IUserService _userService;

        public LibraryCardController(ILibraryCardService libraryCardService,UserManager<ApplicationUser> userManager,IUserService userService)
        {
            _libraryCardService = libraryCardService;
            _userManager = userManager;
            _userService = userService;
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
        public async Task<IActionResult> AddLibraryCard(UpdateUserDTO getUserDTO)
        {
            
            var userId = getUserDTO.Id;

            var result = await _libraryCardService.CreateAsync(userId);

            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.Error.Description);
                return RedirectToAction("AccountDashboard", "User");
            }

            getUserDTO.LibraryCardId = result.Value.Id;
            await _userService.UpdateUserAsync(getUserDTO);
            TempData["SuccessMessage"] = "LibraryCard added successfully!";
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
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> UpdateLibraryCard(UpdateLibraryCardDTO updateLibraryCardDTO)
        {
            var result =  await _libraryCardService.UpdateAsync(updateLibraryCardDTO);

            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.Error.Description);
                return RedirectToAction("UpdateLibraryCard");
            }

            TempData["SuccessMessage"] = "LibraryCard updated successfully!";
            return RedirectToAction("AccountDashboard", "User");
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteLibraryCard(Guid userId)
        {
            var result = await _libraryCardService.DeleteAsync(userId);
            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.Error.Description);
            }
            return RedirectToAction("GetAllLibraryCards");
        }

    }
}
