using Application.Services.Interfaces;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IBookRequestService _bookRequestService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(IBookRequestService bookRequestService, INotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _bookRequestService = bookRequestService ?? throw new ArgumentNullException(nameof(bookRequestService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        public async Task<IActionResult> SubscribeToBookAvailability(Guid bookId)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            await _bookRequestService.CreateBookNotificationAsync(userId, bookId);

            var subscriptions = await _bookRequestService.GetUserSubscriptionsAsync(userId);
            return View("MySubscriptions", subscriptions);
        }
        public async Task<IActionResult> UnsubscribeFromBook(Guid requestId)
        {
            await _bookRequestService.RemoveSubscriptionAsync(requestId);

            return RedirectToAction("AccountDashboard", "User");
        }
        public async Task<IActionResult> MySubscriptions()
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var subscriptions = await _bookRequestService.GetUserSubscriptionsAsync(userId);

            return View(subscriptions);
        }
        public async Task<IActionResult> MyNotifications()
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);

            return View(notifications);
        }
    }
}
