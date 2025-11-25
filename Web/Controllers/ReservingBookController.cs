using Application.DTOs.Books;
using Application.DTOs.Reservations;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Filters;

namespace Web.Controllers
{
    public class ReservingBookController : Controller
    {
        private readonly IReservingBookService _reservingBookService;
        private readonly IBookService _bookService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservingBookController(IReservingBookService reservingBookService, UserManager<ApplicationUser> userManager, IBookService bookService)
        {
            _reservingBookService = reservingBookService;
            _userManager = userManager;
            _bookService = bookService;
        }
        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> ReserveBook(Guid bookId)
        {
            var book = await _bookService.GetByIdAsync(bookId);

            if (book.IsFailure)
            {
                TempData["ErrorMessage"] = book.Error.Description;
                return RedirectToAction("GetAllBooks", "Book");
            }

            return View(new CreateReservationDTO { BookId = bookId, UserId = Guid.Parse(_userManager.GetUserId(HttpContext.User)), Book = book.Value });
        }
        [HttpPost]
        [CustomAuthorize]
        public async Task<IActionResult> ReserveBook(CreateReservationDTO createReservationDTO)
        {
            var result = await _reservingBookService.ReserveBookAsync(createReservationDTO);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Description;
                return RedirectToAction("GetBookById","Book",new { id = createReservationDTO.BookId });
            }

            return RedirectToAction("GetUserActiveReservations");
        }
        [CustomAuthorize]
        public async Task<IActionResult> ReturnBook(Guid Id)
        {
            await _reservingBookService.ReturnBookAsync(Id);

            return RedirectToAction("GetUserActiveReservations");
        }
        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> Details(Guid Id)
        {
            var reservation = await _reservingBookService.GetByIdAsync(Id);

            if (reservation.IsFailure)
            {
                TempData["ErrorMessage"] = reservation.Error.Description;
                return RedirectToAction("GetAllBooks", "Book");
            }

            return View(reservation.Value);
        }
        [HttpGet]
        [CustomAuthorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _reservingBookService.GetAllAsync();

            return View(reservations);
        }
        [HttpGet]
        [CustomAuthorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllActiveReservations()
        {
            var reservations = await _reservingBookService.GetActiveReservationsAsync();

            return View(reservations);
        }
        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetUserReturnedReservations()
        {
            Guid userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var reservations = await _reservingBookService.GetReturnedByUserIdAsync(userId);

            return View(reservations);
        }
        [CustomAuthorize]
        public async Task<IActionResult> GetUserActiveReservations()
        {
            Guid userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var reservations = await _reservingBookService.GetActiveByUserIdAsync(userId);

            return View(reservations);
        }
    }
}
