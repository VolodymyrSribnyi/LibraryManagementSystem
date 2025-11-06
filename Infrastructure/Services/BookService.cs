using Abstractions.Repositories;
using Application.DTOs.Authors;
using Application.DTOs.Books;
using Application.ErrorHandling;
using Application.Filters;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;
        private readonly IAuthorRepository _authorRepository;
        private const int _pictureSize = 2097152; // 2MB

        public BookService(IBookRepository bookRepository, IMapper mapper, ILogger<BookService> logger, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
        }

        public async Task<Result<GetBookDTO>> AddAsync(CreateBookDTO createBookDTO)
        {
            if (createBookDTO == null)
            {
                _logger.LogWarning("AddAsync called with null CreateBookDTO.");
                return Result<GetBookDTO>.Failure(Errors.NullData);
            }

            var bookToCreate = _mapper.Map<Book>(createBookDTO);
            var existingBook = await _bookRepository.GetByTitleAsync(bookToCreate.Title);

            if (existingBook != null)
            {
                _logger.LogInformation($"Book with title {bookToCreate.Title} already exists.");
                return Result<GetBookDTO>.Failure(Errors.BookExists);
            }

            var existingAuthor = await _authorRepository.GetByIdAsync(createBookDTO.AuthorId);


            if (createBookDTO.Picture != null)
            {
                var pictureResult = await ConvertIFormFileToByteArray(createBookDTO.Picture);

                if (pictureResult.IsFailure)
                {
                    _logger.LogWarning($"Failed to convert picture: {pictureResult.Error.Description}");
                    return Result<GetBookDTO>.Failure(pictureResult.Error);
                }

                bookToCreate.PictureSource = pictureResult.Value;
            }

            var book = await _bookRepository.AddAsync(bookToCreate);

            if (book == null)
            {
                _logger.LogError($"Failed to create book with title: {createBookDTO.Title}");
                return Result<GetBookDTO>.Failure(Errors.BookCreationFailed);
            }

            _logger.LogInformation($"Successfully created book with ID: {book.Id}");

            return Result<GetBookDTO>.Success(_mapper.Map<GetBookDTO>(book));
        }
        private async Task<Result<Byte[]>> ConvertIFormFileToByteArray(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No picture file provided or file is empty.");
                return Result<byte[]>.Success(Array.Empty<byte>());
            }
            if (file.Length > _pictureSize)
            {
                _logger.LogWarning("Picture size exceeds the 2MB limit.");
                return Result<byte[]>.Failure(Errors.PictureTooLarge);
            }

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                return Result<byte[]>.Success(ms.ToArray());
            }
        }

        public async Task<Result<GetBookDTO>> UpdateAsync(UpdateBookDTO updateBookDTO)
        {
            if (updateBookDTO == null)
            {
                _logger.LogWarning("UpdateAsync called with null UpdateBookDTO.");
                return Result<GetBookDTO>.Failure(Errors.NullData);
            }

            var bookToUpdate = await _bookRepository.GetByTitleAsync(updateBookDTO.Title);

            if (bookToUpdate == null)
            {
                _logger.LogInformation($"Book with title '{updateBookDTO.Title}' not found.");
                return Result<GetBookDTO>.Failure(Errors.BookNotFound);
            }

            updateBookDTO.Id = bookToUpdate.Id;
            _mapper.Map(updateBookDTO, bookToUpdate);

            if (updateBookDTO.Picture != null)
            {
                var pictureResult = await ConvertIFormFileToByteArray(updateBookDTO.Picture);

                if (pictureResult.IsFailure)
                {
                    _logger.LogWarning($"Failed to convert picture during update: {pictureResult.Error.Description}");
                    return Result<GetBookDTO>.Failure(pictureResult.Error);
                }

                bookToUpdate.PictureSource = pictureResult.Value;
            }

            var updatedBook = await _bookRepository.UpdateAsync(bookToUpdate);

            _logger.LogInformation("Successfully updated book with ID: {BookId}", updateBookDTO.Id);

            return Result<GetBookDTO>.Success(_mapper.Map<GetBookDTO>(bookToUpdate));
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("DeleteAsync called with empty GUID.");
                return Result.Failure(Errors.NullData);
            }

            var bookToDelete = await _bookRepository.GetByIdAsync(id);

            if (bookToDelete == null)
            {
                _logger.LogInformation($"Book with ID {id} not found for deletion.");
                return Result.Failure(Errors.BookNotFound);
            }

            var result = await _bookRepository.DeleteAsync(bookToDelete.Id);

            if (!result)
            {
                _logger.LogError($"Repository returned false when deleting book with id {id}");
                throw new InvalidOperationException($"Failed to delete book with id {id}");
            }

            _logger.LogInformation("Successfully deleted book with ID: {BookId}", id);
            return Result.Success();
        }
        public async Task<Result<GetBookDTO>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetByIdAsync called with empty GUID.");
                return Result<GetBookDTO>.Failure(Errors.NullData);
            }

            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogInformation($"Book with ID {id} not found.");
                return Result<GetBookDTO>.Failure(Errors.BookNotFound);
            }

            return Result<GetBookDTO>.Success(_mapper.Map<GetBookDTO>(book));
        }
        public async Task<Result<IEnumerable<GetBookDTO>>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();

            if (books == null || !books.Any())
            {
                _logger.LogInformation("No books found in repository.");
                return Result<IEnumerable<GetBookDTO>>.Success(Enumerable.Empty<GetBookDTO>());
            }

            return Result<IEnumerable<GetBookDTO>>.Success(_mapper.Map<IEnumerable<GetBookDTO>>(books));
        }

        public async Task<Result<IEnumerable<GetBookDTO>>> GetAllByAuthorAsync(GetAuthorDTO getAuthorDTO)
        {
            if (getAuthorDTO == null)
            {
                _logger.LogWarning("GetAllByAuthorAsync called with null GetAuthorDTO.");
                return Result<IEnumerable<GetBookDTO>>.Success(Enumerable.Empty<GetBookDTO>());
            }

            var books = await _bookRepository.GetAllByAuthorAsync(_mapper.Map<Author>(getAuthorDTO));

            if (books == null || !books.Any())
            {
                _logger.LogInformation($"No books found for author {getAuthorDTO.FirstName} {getAuthorDTO.Surname}.");
                return Result<IEnumerable<GetBookDTO>>.Success(Enumerable.Empty<GetBookDTO>());
            }

            return Result<IEnumerable<GetBookDTO>>.Success(_mapper.Map<IEnumerable<GetBookDTO>>(books));
        }

        public async Task<Result<IEnumerable<GetBookDTO>>> GetAllByGenresAsync(IEnumerable<Genre> genres)
        {
            if (genres == null || !genres.Any())
            {
                _logger.LogWarning("GetAllByGenresAsync called with null or empty genres.");
                return Result<IEnumerable<GetBookDTO>>.Failure(Errors.NullData);
            }

            var books = await _bookRepository.GetAllByGenresAsync(genres);

            if (books == null || !books.Any())
            {
                _logger.LogInformation($"No books found for the specified genres: {string.Join(", ", genres)}");
                return Result<IEnumerable<GetBookDTO>>.Failure(Errors.BookNotFound);
            }

            return Result<IEnumerable<GetBookDTO>>.Success(_mapper.Map<IEnumerable<GetBookDTO>>(books));
        }

        public async Task<Result<IEnumerable<GetBookDTO>>> GetAllByPublisherAsync(IEnumerable<string> publishers)
        {
            if (publishers == null || !publishers.Any())
            {
                _logger.LogWarning("GetAllByPublisherAsync called with null or empty publishers.");
                return Result<IEnumerable<GetBookDTO>>.Failure(Errors.NullData);
            }

            var books = await _bookRepository.GetAllByPublisherAsync(publishers);

            if (books == null || !books.Any())
            {
                _logger.LogInformation($"No books found for publishers: {string.Join(", ", publishers)}");
                return Result<IEnumerable<GetBookDTO>>.Failure(Errors.BookNotFound);
            }

            return Result<IEnumerable<GetBookDTO>>.Success(_mapper.Map<IEnumerable<GetBookDTO>>(books));
        }

        public async Task<Result<GetBookDTO>> GetByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                _logger.LogWarning("GetByTitleAsync called with null or empty title.");
                return Result<GetBookDTO>.Failure(Errors.NullData);
            }

            var book = await _bookRepository.GetByTitleAsync(title);

            if (book == null)
            {
                _logger.LogInformation($"Book with title '{title}' not found.");
                return Result<GetBookDTO>.Failure(Errors.BookNotFound);
            }

            return Result<GetBookDTO>.Success(_mapper.Map<GetBookDTO>(book));
        }



        public async Task<Result> UpdateAvailabilityAsync(UpdateBookStatusDTO updateBookStatusDTO)
        {
            if (updateBookStatusDTO == null)
            {
                _logger.LogWarning("UpdateAvailabilityAsync called with null UpdateBookStatusDTO.");
                return Result.Failure(Errors.NullData);
            }

            var existingBook = await _bookRepository.GetByIdAsync(updateBookStatusDTO.BookId);

            if (existingBook == null)
            {
                _logger.LogInformation($"Book with ID {updateBookStatusDTO.BookId} not found for availability update.");
                return Result.Failure(Errors.BookNotFound);
            }

            var bookToUpdate = new Book
            {
                Id = updateBookStatusDTO.BookId,
                IsAvailable = updateBookStatusDTO.IsAvailable
            };

            var result = await _bookRepository.UpdateAvailabilityAsync(bookToUpdate);

            if (!result)
            {
                _logger.LogError($"Repository returned false when updating availability for book {updateBookStatusDTO.BookId}");
                throw new InvalidOperationException($"Failed to update book availability with id {updateBookStatusDTO.BookId}");
            }

            _logger.LogInformation($"Successfully updated book availability for book with id{updateBookStatusDTO.BookId}");

            return Result.Success();
        }

        public async Task<Result> UpdateRatingAsync(UpdateBookRatingDTO updateBookRatingDTO)
        {
            if (updateBookRatingDTO == null)
            {
                _logger.LogWarning("UpdateRatingAsync called with null UpdateBookRatingDTO.");
                return Result.Failure(Errors.NullData);
            }

            if (((int)updateBookRatingDTO.Rating) < 0 || ((int)updateBookRatingDTO.Rating) > 5)
            {
                _logger.LogWarning($"Invalid rating value: {updateBookRatingDTO.Rating}. Rating must be between 0 and 5.");
                return Result.Failure(Errors.InvalidRating);
            }

            var existingBook = await _bookRepository.GetByIdAsync(updateBookRatingDTO.BookId);

            if (existingBook == null)
            {
                _logger.LogInformation($"Book with ID {updateBookRatingDTO.BookId} not found for rating update.");
                return Result.Failure(Errors.BookNotFound);
            }

            var bookToUpdate = new Book
            {
                Id = updateBookRatingDTO.BookId,
                Rating = updateBookRatingDTO.Rating
            };

            var result = await _bookRepository.UpdateRatingAsync(bookToUpdate);

            if (!result)
            {
                _logger.LogError($"Repository returned false when updating rating for book {updateBookRatingDTO.BookId}");
                throw new InvalidOperationException($"Failed to update book rating for book with id {updateBookRatingDTO.BookId}");
            }

            _logger.LogInformation("Successfully updated rating for book ID: {BookId}", updateBookRatingDTO.BookId);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<GetBookDTO>>> GetFilteredAsync(BookFilter bookFilter)
        {
            if (bookFilter == null)
            {
                _logger.LogWarning("GetFilteredAsync called with null BookFilter.");
                return Result<IEnumerable<GetBookDTO>>.Success(Enumerable.Empty<GetBookDTO>());
            }

            bool? isAvailable = null;

            if (!string.IsNullOrEmpty(bookFilter.IsAvailable))
            {
                isAvailable = bool.Parse(bookFilter.IsAvailable);
            }

            Expression<Func<Book, bool>> expr = b =>
                (bookFilter.Years == null || !bookFilter.Years.Any() || bookFilter.Years.Contains(b.PublishingYear)) &&
                (bookFilter.Genres == null || !bookFilter.Genres.Any() || bookFilter.Genres.Contains(b.Genre)) &&
                (bookFilter.Publishers == null || !bookFilter.Publishers.Any() || bookFilter.Publishers.Contains(b.Publisher)) &&
                (!bookFilter.MinRating.HasValue || b.Rating >= bookFilter.MinRating) &&
                (bookFilter.AuthorsId == null || !bookFilter.AuthorsId.Any() || bookFilter.AuthorsId.Contains(b.AuthorId)) &&
                (!isAvailable.HasValue || b.IsAvailable == isAvailable);

            var filteredBooks = await _bookRepository.GetFilteredAsync(expr);

            if (filteredBooks == null || !filteredBooks.Any())
            {
                _logger.LogInformation("No books found matching the specified filter criteria.");
                return Result<IEnumerable<GetBookDTO>>.Success(Enumerable.Empty<GetBookDTO>());
            }

            return Result<IEnumerable<GetBookDTO>>.Success(_mapper.Map<IEnumerable<GetBookDTO>>(filteredBooks));
        }
        public async Task<Result<byte[]>> GetBookPictureAsync(Guid bookId)
        {
            if (bookId == Guid.Empty)
            {
                _logger.LogWarning("GetBookPictureAsync called with empty GUID.");
                return Result<byte[]>.Failure(Errors.NullData);
            }

            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null)
            {
                _logger.LogInformation($"Book with ID {bookId} not found when retrieving picture.");
                return Result<byte[]>.Failure(Errors.BookNotFound);
            }

            if (book.PictureSource == null || book.PictureSource.Length == 0)
            {
                _logger.LogInformation($"Book with ID {bookId} has no picture.");
                return Result<byte[]>.Failure(Errors.PictureNotFound);
            }

            return Result<byte[]>.Success(book.PictureSource);
        }
    }
}
