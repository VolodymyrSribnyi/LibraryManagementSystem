using Application.DTOs.Books;

namespace Application.DTOs.Authors
{
    public class GetAuthorDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string? MiddleName { get; set; }
        public string FullName
        {
            get
            {
                if (MiddleName == null)
                    return FirstName + " " + Surname;
                else
                    return FirstName + " " + MiddleName + " " + Surname;
            }
        }
        public int Age { get; set; }
        public string Description { get; set; }
        public List<GetBookDTO> Books { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
