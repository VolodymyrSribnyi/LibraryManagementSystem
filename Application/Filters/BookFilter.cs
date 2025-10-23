using Domain.Enums;

namespace Application.Filters
{
    public class BookFilter
    {
        public List<Guid>? AuthorsId { get; set; } = [];
        public List<string>? Publishers { get; set; } = [];
        public Rating? MinRating { get; set; }
        public List<int>? Years { get; set; } = [];
        public string? IsAvailable { get; set; }
        public List<Genre>? Genres { get; set; } = [];
    }
}
