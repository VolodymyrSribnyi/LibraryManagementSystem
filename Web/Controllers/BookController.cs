using Abstractions.Repositories;
using Application.DTOs.Authors;
using Application.DTOs.Books;
using Application.Filters;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Filters;

namespace Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IBlobStorageService _blobStorageService;

        public BookController(IBookService bookService, IAuthorService authorService, IBlobStorageService blobStorageService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _blobStorageService = blobStorageService;
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> AddBook()
        {
            var authorsResult = await _authorService.GetAllAsync();

            if (authorsResult.IsFailure)
            {
                TempData["ErrorMessage"] = authorsResult.Error.Description;
                return View(new CreateBookDTO { Authors = authorsResult.Value.ToList() });
            }

            return View(new CreateBookDTO { Authors = authorsResult.Value.ToList() });
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> AddBook(CreateBookDTO bookDTO)
        {
            var bookResult = await _bookService.AddAsync(bookDTO);

            if (bookResult.IsFailure)
            {
                TempData["ErrorMessage"] = bookResult.Error.Description;
                return RedirectToAction("AddBook");
            }
            TempData["SuccessMessage"] = "Book added successfully!";

            return RedirectToAction("GetAllBooks");
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var result = await _bookService.DeleteAsync(id);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Description;
                return RedirectToAction("GetAllBooks");
            }

            TempData["SuccessMessage"] = "Book deleted successfully!";
            return RedirectToAction("GetAllBooks");
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> UpdateBook(Guid bookId)
        {
            var bookResult = await _bookService.GetByIdAsync(bookId);

            if (bookResult.IsFailure)
            {
                TempData["ErrorMessage"] = bookResult.Error.Description;
                return RedirectToAction("GetAllBooks");
            }

            var authorsResult = await _authorService.GetAllAsync();

            if (authorsResult.IsFailure)
            {
                TempData["ErrorMessage"] =  authorsResult.Error.Description;
                return RedirectToAction("GetAllBooks");
            }

            var book = bookResult.Value;

            var updateBookDTO = new UpdateBookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Genre = book.Genre,
                PublishingYear = book.PublishingYear,
                Publisher = book.Publisher,
                AuthorId = book.Author.Id,
                Authors = authorsResult.Value.ToList()
            };

            return View(updateBookDTO);
        }
        [CustomAuthorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> UpdateBook(UpdateBookDTO updateBookDTO)
        {
            var updateResult = await _bookService.UpdateAsync(updateBookDTO);
            if (updateResult.IsFailure)
            {
                TempData["ErrorMessage"] = updateResult.Error.Description;
                return RedirectToAction("UpdateBook", new { bookId = updateBookDTO.Id });
            }

            TempData["SuccessMessage"] = "Book updated successfully!";
            return RedirectToAction("GetAllBooks");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBookRating(UpdateBookRatingDTO updateBookDTO)
        {
            var ratingResult = await _bookService.UpdateRatingAsync(updateBookDTO);

            if (ratingResult.IsFailure)
            {
                TempData["ErrorMessage"] = ratingResult.Error.Description;
                return RedirectToAction("GetBookById", new { id = updateBookDTO.BookId });
            }
            return RedirectToAction("GetAllBooks");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBookAvailability(UpdateBookStatusDTO updateBookStatusDTO)
        {
            var result = await _bookService.UpdateAvailabilityAsync(updateBookStatusDTO);
            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Description;
                return RedirectToAction("GetBookById", new { id = updateBookStatusDTO.BookId });
            }
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);

            if (book.IsFailure)
            {
                TempData["ErrorMessage"] = book.Error.Description;
                return RedirectToAction("GetAllBooks");
            }

            return View("GetBookById", book.Value);
        }
        [HttpGet("books/{id}/picture")]
        public async Task<IActionResult> GetPictureUrl(Guid id)
        {
            var book = await _bookService.GetBookPictureAsync(id);

            if (book.IsFailure)
            {
                return NotFound(book.Error.Description);
            }

            return File(book.Value, "image/jpeg");
        }
        [HttpGet("test-url")]
        public IActionResult TestBlobUrl()
        {
            var testBlobName = "23c0f5d4-6455-40d6-a5cb-d6b890bf20a3.jpeg"; // Use one of your actual blob names
            var url = _blobStorageService.GetImageUrl(testBlobName, "book-images");

            return Ok(new { url = url });
        }
        [HttpGet]
        public async Task<IActionResult> GetFilteredBooks(BookFilter bookFilter)
        {
            await PopulateViewBagAsync();

            var books = await _bookService.GetFilteredAsync(bookFilter);

            return View("GetAllBooks", books.Value);
        }
        private async Task PopulateViewBagAsync()
        {
            var authorsResult = await _authorService.GetAllAsync();

            ViewBag.Authors = authorsResult.Value;
            ViewBag.Genres = Enum.GetValues(typeof(Genre)).Cast<Genre>().ToList();

            var booksResult = await _bookService.GetAllAsync();
            var allBooks = booksResult.Value;

            ViewBag.Publishers = allBooks.Select(b => b.Publisher).Distinct().ToList();
            ViewBag.Years = allBooks.Select(b => b.PublishingYear).Distinct().OrderBy(y => y).ToList();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            await PopulateViewBagAsync();
            var books = await _bookService.GetAllAsync();
            return View("GetAllBooks", books.Value);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooksByAuthor(GetAuthorDTO authorDTO)
        {
            var books = await _bookService.GetAllByAuthorAsync(authorDTO);

            return View(books.Value);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooksByGenres(IEnumerable<Genre> genres)
        {
            var books = await _bookService.GetAllByGenresAsync(genres);

            return View(books.Value);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooksByPublisher(IEnumerable<string> publishers)
        {
            var books = await _bookService.GetAllByPublisherAsync(publishers);
            return View(books.Value);
        }
        [HttpPost]
        public async Task<IActionResult> GetBookByTitle(string title)
        {
            var book = await _bookService.GetByTitleAsync(title);

            if (book.IsFailure)
            {
                ModelState.AddModelError("", book.Error.Description);
                return RedirectToAction("GetAllBooks");
            }

            return RedirectToAction("GetBookById", new { id = book.Value.Id });
        }
    }
}
