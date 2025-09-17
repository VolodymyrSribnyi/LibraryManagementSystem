using Application.DTOs.Reservations;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ReservingBookController : Controller
    {
        private readonly IReservingBookService _reservingBookService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservingBookController(IReservingBookService reservingBookService, UserManager<ApplicationUser> userManager)
        {
            _reservingBookService = reservingBookService;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult ReserveBook(Guid bookId)
        {
            return View(new CreateReservationDTO { BookId = bookId,UserId = Guid.Parse(_userManager.GetUserId(HttpContext.User)) });
        }
        [HttpPost]
        public async Task<IActionResult> ReserveBook(CreateReservationDTO createReservationDTO)
        {
            var reservation = await _reservingBookService.ReserveBookAsync(createReservationDTO);
            
            return RedirectToAction("GetAllReservations");
        }
        public async Task<IActionResult> ReturnBook(Guid Id)
        {
            var reservation = await _reservingBookService.ReturnBookAsync(Id);

            return RedirectToAction("GetAllReservations");
        }
        public async Task<IActionResult> Details(Guid Id)
        {
            var reservation = await _reservingBookService.GetByIdAsync(Id);

            return View(reservation);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _reservingBookService.GetAllAsync();

            return View(reservations);
        }
        public async Task<IActionResult> GetAllActiveReservations()
        {
            var reservations = await _reservingBookService.GetActiveReservationsAsync();

            return View(reservations);
        }
        public async Task<IActionResult> GetUserActiveReservations()
        {
            Guid userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var reservations = await _reservingBookService.GetByUserIdAsync(userId);

            return View(reservations);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
