namespace Domain.Entities
{
//    + Id : Guid
//+ UserId: Guid
//~User: IUser
//+ Book: IBook
//+ ReservedAt: DateTime
//+ EndsAt: DateTime
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