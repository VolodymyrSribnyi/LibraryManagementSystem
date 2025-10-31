using Application.DTOs.Authors;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;
        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService ?? throw new ArgumentNullException(nameof(authorService));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAsync();

            return View("GetAllAuthors", authors.Value);
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult AddAuthor()
        {
            return View();
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> AddAuthor(CreateAuthorDTO createAuthorDTO)
        {
            var authorResult = await _authorService.AddAsync(createAuthorDTO);
            if(authorResult.IsFailure)
            {
                TempData["ErrorMessage"] = authorResult.Error.Description;
                return View(createAuthorDTO);
            }
            TempData["SuccessMessage"] = "Author added successfully!";
            return RedirectToAction("GetAllAuthors");
        }
        [HttpGet]
        public IActionResult GetAuthorsId()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);

            if(author.IsFailure)
            {
                TempData["ErrorMessage"] = author.Error.Description;
                return View("GetAllAuthors");
            }

            return View("GetAuthorById", author.Value);
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> UpdateAuthor(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);

            if(author.IsFailure)
            {
                TempData["ErrorMessage"] = author.Error.Description;
                return RedirectToAction("GetAllAuthors");
            }

            return View(_authorService.MapToUpdateAuthorDTO(author.Value));
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> UpdateAuthor(UpdateAuthorDTO updateAuthorDTO)
        {
            var authorResult = await _authorService.UpdateAsync(updateAuthorDTO);

            if(authorResult.IsFailure)
            {
                TempData["ErrorMessage"] = authorResult.Error.Description;
                return View(updateAuthorDTO);
            }

            TempData["SuccessMessage"] = "Author updated successfully!";
            return RedirectToAction("GetAllAuthors");
        }
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var authorResult = await _authorService.DeleteAsync(id);

            if(authorResult.IsFailure)
            {
                TempData["ErrorMessage"] = authorResult.Error.Description;
                return RedirectToAction("GetAllAuthors");
            }

            TempData["SuccessMessage"] = "Author deleted successfully!";
            return RedirectToAction("GetAllAuthors");
        }
        public async Task<IActionResult> GetAuthorByFullName(string surname)
        {
            var author = await _authorService.GetByFullNameAsync(surname);
            return View("GetAuthorById", author.Value);
        }
    }
}
