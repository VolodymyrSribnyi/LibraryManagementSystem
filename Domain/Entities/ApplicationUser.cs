using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
//    + FirstName: string
//+ Surname: string
//+ MiddleName?: string
//+ Age: int
//+ LibraryCard: ILibraryCard
//+ ReservedBooks: List<IReservation>
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string? MiddleName { get; set; }
        public int Age { get; set; }
        public LibraryCard? LibraryCard { get; set; }
        public List<Reservation> ReservedBooks { get; set; } = [];
        public List<Notification> Notifications { get; set; } = [];
        public DateTime CreatedAt { get; set; } 
    }
}