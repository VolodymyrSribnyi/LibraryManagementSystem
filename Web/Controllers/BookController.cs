using Application.DTOs.Books;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task<GetBookDTO> AddBook(CreateBookDTO bookDTO)
        {
            var book = await _bookService.AddAsync(bookDTO);

            return book;
        }
    }
}
