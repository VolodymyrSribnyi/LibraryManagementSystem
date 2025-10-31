using Application.Services.Interfaces;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IBookNotificationRequestService _bookRequestService;
        private readonly INotificationService _notificationService;
        private readonly IBookService _bookService;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public NotificationController(IBookNotificationRequestService bookRequestService, INotificationService notificationService, 
            UserManager<ApplicationUser> userManager,IBookService bookService)
        {
            _bookRequestService = bookRequestService ?? throw new ArgumentNullException(nameof(bookRequestService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        }
        public async Task<IActionResult> SubscribeToBookAvailability(Guid bookId)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            var result = await _bookRequestService.CreateBookNotificationAsync(userId, bookId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error.Description;
                // Handle error (e.g., show an error message to the user)
                return RedirectToAction("GetAllBooks","Book");
            }
            var subscriptions = await _bookRequestService.GetUserSubscriptionsAsync(userId);
            return View("MySubscriptions", subscriptions.Value);
        }
        public async Task<IActionResult> UnsubscribeFromBook(Guid requestId)
        {
            var result = await _bookRequestService.RemoveSubscriptionAsync(requestId);

            if(result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error.Description;
                // Handle error (e.g., show an error message to the user)
                return RedirectToAction("MySubscriptions");
            }
            return RedirectToAction("AccountDashboard", "User");
        }
        public async Task<IActionResult> MySubscriptions()
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var subscriptions = await _bookRequestService.GetUserSubscriptionsAsync(userId);

            if (subscriptions.IsFailure)
            {
                TempData["ErrorMessage"] = subscriptions.Error.Description;
                // Handle error (e.g., show an error message to the user)
                return RedirectToAction("AccountDashboard", "User");
            }
            return View(subscriptions.Value);
        }
        public async Task<IActionResult> MyNotifications()
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);

            return View(notifications);
        }
    }
}
