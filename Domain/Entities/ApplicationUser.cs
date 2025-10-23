using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    /// <summary>
    /// Represents an application user with extended profile information, including personal details,  library-related
    /// data, and notification preferences.
    /// </summary>
    /// <remarks>This class extends the <see cref="IdentityUser{TKey}"/> class with additional properties 
    /// specific to the application's domain, such as personal details (e.g., first name, surname),  library card
    /// information, and collections for managing reservations, notifications, and book subscriptions.</remarks>
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string? MiddleName { get; set; }
        public int Age { get; set; }
        public LibraryCard? LibraryCard { get; set; }
        public List<Reservation> ReservedBooks { get; set; } = [];
        public List<Notification> Notifications { get; set; } = [];
        public List<BookNotificationRequest> BookSubscriptions { get; set; } = [];
        public DateTime CreatedAt { get; set; } 
    }
}