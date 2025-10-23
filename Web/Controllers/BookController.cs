using Abstractions.Repositories;
using Application.DTOs.Authors;
using Application.DTOs.Books;
using Application.Filters;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly LibraryContext _context;

        public BookController(IBookService bookService,LibraryContext context, IAuthorService authorService)
        {
            _bookService = bookService;
            _context = context;
            _authorService = authorService;
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult AddBook()
        {
            return View(new CreateBookDTO { Authors = _context.Authors.ToList()});
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> AddBook(CreateBookDTO bookDTO)
        {
            await _bookService.AddAsync(bookDTO);

            return RedirectToAction("GetAllBooks");
        }
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _bookService.DeleteAsync(id);
            return RedirectToAction("GetAllBooks");
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> UpdateBook(Guid bookId)
        {
            var book = await _bookService.GetByIdAsync(bookId);
            var updateBookDTO = new UpdateBookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Genre = book.Genre,
                PublishingYear = book.PublishingYear,
                Publisher = book.Publisher,
                AuthorId = book.Author.Id,
                Authors = _context.Authors.ToList()
            };
            return View(updateBookDTO);
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> UpdateBook(UpdateBookDTO updateBookDTO)
        {
            await _bookService.UpdateAsync(updateBookDTO);
            return RedirectToAction("GetAllBooks");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBookRating(UpdateBookRatingDTO updateBookDTO)
        {
            await _bookService.UpdateRatingAsync(updateBookDTO);
            return RedirectToAction("GetAllBooks");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBookAvailability(UpdateBookStatusDTO updateBookStatusDTO)
        {
            await _bookService.UpdateAvailabilityAsync(updateBookStatusDTO);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);

            return View("GetBookById", book);
        }
        /// <summary>
        /// Викликається коли потрібно отримати зображення книги у вигляді файлу(браузер сам підвантажує ресурс)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("books/{id}/picture")]
        public async Task<IActionResult> GetPicture(Guid id)
        {
            var book = await _bookService.GetBookPictureAsync(id);

            return File(book, "image/jpeg");
        }
        [HttpPost]
        public async Task<IActionResult> GetFilteredBooks(BookFilter bookFilter)
        {
            await PopulateViewBagAsync();

            var books = await _bookService.GetFilteredAsync(bookFilter);
            return View("GetAllBooks", books);
        }
        private async Task PopulateViewBagAsync()
        {
            ViewBag.Authors = await _authorService.GetAllAsync();
            ViewBag.Genres = Enum.GetValues(typeof(Genre)).Cast<Genre>().ToList();
            var allBooks = await _bookService.GetAllAsync();
            ViewBag.Publishers = allBooks.Select(b => b.Publisher).Distinct().ToList();
            ViewBag.Years = allBooks.Select(b => b.PublishingYear).Distinct().OrderBy(y => y).ToList();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            await PopulateViewBagAsync();
            var books = await _bookService.GetAllAsync();
            return View("GetAllBooks", books);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooksByAuthor(GetAuthorDTO authorDTO)
        {
            var books = await _bookService.GetAllByAuthorAsync(authorDTO);
            
            return View(books);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooksByGenres(IEnumerable<Genre> genres)
        {
            var books = await _bookService.GetAllByGenresAsync(genres);
            return View(books);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooksByPublisher(IEnumerable<string> publishers)
        {
            var books = await _bookService.GetAllByPublisherAsync(publishers);
            return View(books);
        }
        [HttpPost]
        public async Task<IActionResult> GetBookByTitle(string title)
        {
            var book = await _bookService.GetByTitleAsync(title);
            return RedirectToAction("GetBookById", new { id = book.Id } );
        }
    }
}
