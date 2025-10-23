namespace Domain.Entities
{
    /// <summary>
    /// Represents an author, including their personal details, authored books, and metadata.
    /// </summary>
    /// <remarks>This class provides properties to store information about an author, such as their name, age,
    /// and the books they have written. It also includes metadata like the creation timestamp and  a flag indicating
    /// whether the author record is marked as deleted.</remarks>
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
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }
    }
}