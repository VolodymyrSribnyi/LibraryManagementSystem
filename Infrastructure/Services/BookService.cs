using Abstractions.Repositories;
using Application.DTOs.Authors;
using Application.DTOs.Books;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;
        public BookService(IBookRepository bookRepository, IMapper mapper, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<GetBookDTO> AddAsync(CreateBookDTO createBookDTO)
        {
            if (createBookDTO == null)
                throw new ArgumentNullException(nameof(createBookDTO));

            var bookToCreate = _mapper.Map<Book>(createBookDTO);
            var existingBook = await _bookRepository.GetByIdAsync(bookToCreate.Id);
            if (existingBook != null)
            {
                _logger.LogWarning($"Book with title {bookToCreate.Title} exists");
                throw new BookExistsException(bookToCreate.Title);
            }

            var book = await _bookRepository.AddAsync(bookToCreate);

            if (book == null)
            {
                _logger.LogWarning($"Failed to create book with title: {createBookDTO.Title}");
                throw new InvalidOperationException("Failed to create book");
            }

            _logger.LogInformation($"Successfully created book with ID: {book.Id}");
            return _mapper.Map<GetBookDTO>(book);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            var bookToDelete = await _bookRepository.GetByIdAsync(id);

            if (bookToDelete == null)
            {
                throw new BookNotFoundException(id);
            }

            bookToDelete.IsDeleted = true;
            var result = await _bookRepository.DeleteAsync(bookToDelete.Id);

            if (!result)
            {
                _logger.LogWarning($"Failed to delete book with id {id}");
                throw new InvalidOperationException("Failed to delete book");
            }

            _logger.LogInformation("Successfully deleted book with ID: {BookId}", id);
            return true;
        }
        public async Task<GetBookDTO> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("id");

            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogInformation($"No book with id {id} found in repository");
                throw new BookNotFoundException(id);
            }

            return _mapper.Map<GetBookDTO>(book);
        }
        public async Task<IEnumerable<GetBookDTO>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();

            if (books == null || !books.Any())
            {
                _logger.LogInformation("No books found in repository");
                return Enumerable.Empty<GetBookDTO>();
            }

            return _mapper.Map<IEnumerable<GetBookDTO>>(books);
        }

        public async Task<IEnumerable<GetBookDTO>> GetAllByAuthorAsync(GetAuthorDTO getAuthorDTO)
        {
            if (getAuthorDTO == null)
                throw new ArgumentNullException(nameof(getAuthorDTO));

            var books = await _bookRepository.GetAllByAuthorAsync(_mapper.Map<Author>(getAuthorDTO));

            if (books == null || !books.Any())
            {
                _logger.LogInformation($"No books found for author {getAuthorDTO.FirstName} {getAuthorDTO.Surname} in repository");
                return Enumerable.Empty<GetBookDTO>();
            }

            return _mapper.Map<IEnumerable<GetBookDTO>>(books);
        }

        public async Task<IEnumerable<GetBookDTO>> GetAllByGenresAsync(IEnumerable<Genre> genres)
        {
            if (genres == null || !genres.Any())
                throw new ArgumentNullException("At least one genre must be specified", nameof(genres));

            var books = await _bookRepository.GetAllByGenresAsync(genres);

            if (books == null || !books.Any())
            {
                _logger.LogInformation("No books found for the specified genres");
                throw new BookNotFoundException(genres);
            }

            return _mapper.Map<IEnumerable<GetBookDTO>>(books);
        }

        public async Task<IEnumerable<GetBookDTO>> GetAllByPublisherAsync(IEnumerable<string> publishers)
        {
            if (publishers == null || !publishers.Any())
                throw new ArgumentNullException("At least one publisher must be specified", nameof(publishers));
            var books = await _bookRepository.GetAllByPublisherAsync(publishers);

            if (books == null || !books.Any())
            {
                _logger.LogInformation("No books found for the specified publishers");
                throw new BookNotFoundException("No books found for the specified publishers");
            }

            return _mapper.Map<IEnumerable<GetBookDTO>>(books);
        }



        public async Task<GetBookDTO> GetByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            var book = await _bookRepository.GetByTitleAsync(title);

            if (book == null)
            {
                _logger.LogInformation($"Book with title {title} not found");
                throw new BookNotFoundException($"Book with title {title} not found");
            }

            return _mapper.Map<GetBookDTO>(book);
        }

        public async Task<GetBookDTO> UpdateAsync(UpdateBookDTO updateBookDTO)
        {
            if (updateBookDTO == null)
                throw new ArgumentNullException(nameof(updateBookDTO));

            var bookExists = await _bookRepository.GetByIdAsync(updateBookDTO.Id);

            if (bookExists == null)
            {
                _logger.LogInformation($"Book with title {updateBookDTO.Title} not found");
                throw new BookNotFoundException(updateBookDTO.Id);
            }

            _mapper.Map(updateBookDTO, bookExists);

            var updatedBook = await _bookRepository.UpdateAsync(bookExists);

            if (updatedBook == null)
            {
                _logger.LogWarning($"Failed to update book with id: {updateBookDTO.Id}");
                throw new InvalidOperationException("Failed to update book");
            }
            _logger.LogInformation("Successfully updated book with ID: {BookId}", updateBookDTO.Id);
            return _mapper.Map<GetBookDTO>(bookExists);
        }

        public async Task<bool> UpdateAvailabilityAsync(UpdateBookStatusDTO updateBookStatusDTO)
        {
            if (updateBookStatusDTO == null)
                throw new ArgumentNullException(nameof(updateBookStatusDTO));

            var existingBook = await _bookRepository.GetByIdAsync(updateBookStatusDTO.BookId);

            if (existingBook == null)
            {
                _logger.LogInformation($"Book with id {updateBookStatusDTO.BookId} not found");
                throw new BookNotFoundException(updateBookStatusDTO.BookId);
            }

            var bookToUpdate = new Book
            {
                Id = updateBookStatusDTO.BookId,
                IsAvailable = updateBookStatusDTO.IsAvailable
            };

            var result = await _bookRepository.UpdateAvailabilityAsync(bookToUpdate);

            if (!result)
            {
                _logger.LogWarning($"Failed to update book availability with id {updateBookStatusDTO.BookId}");
                throw new InvalidOperationException("Failed to update book availability");
            }

            _logger.LogInformation($"Successfully updated book availability for book with id{updateBookStatusDTO.BookId}");
            return true;
        }

        public async Task<bool> UpdateRatingAsync(UpdateBookRatingDTO updateBookRatingDTO)
        {
            if (updateBookRatingDTO == null)
                throw new ArgumentNullException(nameof(updateBookRatingDTO));

            var existingBook = await _bookRepository.GetByIdAsync(updateBookRatingDTO.BookId);

            if (existingBook == null)
            {
                _logger.LogInformation($"Book with id {updateBookRatingDTO.BookId} not found");
                throw new BookNotFoundException(updateBookRatingDTO.BookId);

            }

            var bookToUpdate = new Book
            {
                Id = updateBookRatingDTO.BookId,
                Rating = updateBookRatingDTO.Rating
            };

            var result = await _bookRepository.UpdateRatingAsync(bookToUpdate);

            if (!result)
            {
                _logger.LogWarning($"Failed to update book rating for book with id {updateBookRatingDTO.BookId}");
                throw new InvalidOperationException("Failed to update book rating");
            }

            _logger.LogInformation("Successfully updated rating for book ID: {BookId}", updateBookRatingDTO.BookId);
            return true;
        }
    }
}
