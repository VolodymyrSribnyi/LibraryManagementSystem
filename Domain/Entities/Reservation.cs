namespace Domain.Entities
{
    /// <summary>
    /// Represents a reservation of a book by a user in the system.
    /// </summary>
    /// <remarks>A reservation links a user to a specific book and tracks the reservation's start and end
    /// times,  as well as whether the book has been returned. This class is typically used in library or  resource
    /// management systems to manage and monitor book reservations.</remarks>
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public Guid BookId { get; set; }
        public virtual Book Book { get; set; }
        public DateTime ReservedAt { get; set; } = DateTime.Now;
        public DateTime EndsAt { get; set; }
        public bool IsReturned { get; set; } 
    }
}