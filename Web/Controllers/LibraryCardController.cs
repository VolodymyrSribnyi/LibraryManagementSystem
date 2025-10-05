using Application.DTOs.LibraryCards;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class LibraryCardController : Controller
    {
        private readonly ILibraryCardService _libraryCardService;
        public LibraryCardController(ILibraryCardService libraryCardService)
        {
            _libraryCardService = libraryCardService;
        }

        [HttpGet]
        public IActionResult CreateLibraryCard()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateLibraryCard(Guid userId)
        {
            await _libraryCardService.CreateAsync(userId);

            return RedirectToAction("GetAllLibraryCards");
        }
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
        public async Task<IActionResult> DeleteLibraryCard(Guid userId)
        {
            await _libraryCardService.DeleteAsync(userId);
            return View();
        }

    }
}
