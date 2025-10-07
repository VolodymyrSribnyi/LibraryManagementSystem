using Abstractions.Repositories;
using Application.DTOs.Authors;
using Application.DTOs.Books;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly LibraryContext _context;

        public BookController(IBookService bookService,LibraryContext context)
        {
            _bookService = bookService;
            _context = context;
        }
        [HttpGet]
        public IActionResult AddBook()
        {
            return View(new CreateBookDTO { Authors = _context.Authors.ToList()});
        }
        [HttpPost]
        public async Task<IActionResult> AddBook(CreateBookDTO bookDTO)
        {
            var book = await _bookService.AddAsync(bookDTO);

            return RedirectToAction("GetAllBooks");
        }
        
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _bookService.DeleteAsync(id);
            return RedirectToAction("GetAllBooks");
        }
        [HttpGet]
        public IActionResult UpdateBook()
        {
            return View(new UpdateBookDTO { Authors = _context.Authors.ToList() });
        }
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
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
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
        public async Task<IActionResult> GetAllBooksByAuthor(IEnumerable<Genre> genres)
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
        [HttpGet]
        public async Task<IActionResult> GetBookByTitle(string title)
        {
            var book = await _bookService.GetByTitleAsync(title);
            return View(book);
        }
    }
}
