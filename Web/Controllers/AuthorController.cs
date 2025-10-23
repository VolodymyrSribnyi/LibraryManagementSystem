using Application.DTOs.Authors;
using Application.Services.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
            return View("GetAllAuthors", authors);
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
            await _authorService.AddAsync(createAuthorDTO);
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

            return View("GetAuthorById", author);
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> UpdateAuthor(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);

            if(author == null)
                return NotFound();

            return View(_authorService.MapToUpdateAuthorDTO(author));
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> UpdateAuthor(UpdateAuthorDTO updateAuthorDTO)
        {
            await _authorService.UpdateAsync(updateAuthorDTO);
            return RedirectToAction("GetAllAuthors");
        }
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            await _authorService.DeleteAsync(id);
            return RedirectToAction("GetAllAuthors");
        }
        public async Task<IActionResult> GetAuthorByFullName(string surname)
        {
            var author = await _authorService.GetByFullNameAsync(surname);
            return View("GetAuthorById", author);
        }
    }
}
