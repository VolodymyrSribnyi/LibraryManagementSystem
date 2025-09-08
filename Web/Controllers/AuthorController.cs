using Application.DTOs.Authors;
using Application.Services.Interfaces;
using Domain.Exceptions;
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
            return View("GetAllAuthors", authors);
        }
        [HttpGet]
        public IActionResult AddAuthor()
        {
            return View();
        }
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
        public async Task<IActionResult> GetAuthorByIdAsync(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);

            return View("GetAuthorByIdAsync", author);
        }
        [HttpGet]
        public IActionResult UpdateAuthor()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAuthor(UpdateAuthorDTO updateAuthorDTO)
        {
            await _authorService.UpdateAsync(updateAuthorDTO);
            return RedirectToAction("GetAllAuthors");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            await _authorService.DeleteAsync(id);
            return RedirectToAction("GetAllAuthors");
        }
        public async Task<IActionResult> GetAuthorByFullNameAsync(string surname)
        {
            var author = await _authorService.GetByFullNameAsync(surname);
            return View("GetAuthorById", author);
        }
    }
}
