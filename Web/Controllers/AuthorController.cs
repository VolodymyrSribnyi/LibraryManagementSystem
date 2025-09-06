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
            _authorService = authorService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
            {
                var authors = await _authorService.GetAllAsync();
                return View("GetAllAuthors",authors);
            }
            catch (AuthorNotFoundException ex)
            {
                // Log the exception (ex) here as needed
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(400, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet]
        public IActionResult AddAuthor()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAuthorAsync(CreateAuthorDTO createAuthorDTO)
        {
            try
            {
                await _authorService.AddAsync(createAuthorDTO);
                return RedirectToAction("GetAllAuthors");
            }
            catch (AuthorNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet]
        public IActionResult GetAuthorsId()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAuthorByIdAsync(Guid id)
        {
            try
            {
                var author = await _authorService.GetByIdAsync(id);
                return View("GetAuthorByIdAsync", author);
            }
            catch (AuthorNotFoundException ex)
            {
                // Log the exception (ex) here as needed
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(400, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet]
        public IActionResult UpdateAuthor()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAuthorAsync(UpdateAuthorDTO updateAuthorDTO)
        {
            try
            {
                await _authorService.UpdateAsync(updateAuthorDTO);
                return RedirectToAction("GetAllAuthors");
            }
            catch (AuthorNotFoundException ex)
            {
                // Log the exception (ex) here as needed
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(400, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAuthorAsync(Guid id)
        {
            try
            {
                await _authorService.DeleteAsync(id);
                return RedirectToAction("GetAllAuthors");
            }
            catch (AuthorNotFoundException ex)
            {
                // Log the exception (ex) here as needed
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        public async Task<IActionResult> GetAuthorByFullNameAsync(string surname)
        {
            try
            {
                var author = await _authorService.GetByFullNameAsync(surname);
                return View("GetAuthorById", author);
            }
            catch (AuthorNotFoundException ex)
            {
                // Log the exception (ex) here as needed
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(400, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
