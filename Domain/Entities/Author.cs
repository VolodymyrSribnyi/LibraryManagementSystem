namespace Domain.Entities
{
    //    + FirstName: string
    //+ Surname: string
    //+ MiddleName?: string
    //+ Age: int
    //+ Books: List<IBook>
    public class Author
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string? MiddleName { get; set; }
        public string FullName
        {
            get
            {
                if(MiddleName == null)
                    return FirstName + " "  + Surname;
                else
                    return FirstName + " " + MiddleName + " " + Surname;
            }
        }
        public int Age { get; set; }
        public List<Book> Books { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }
    }
}