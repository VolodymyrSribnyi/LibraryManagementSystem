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
using static System.Reflection.Metadata.BlobBuilder;

namespace Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;
        private readonly IAuthorService _authorService;
        private const int _pictureSize = 2097152;
        private const string BookImageContainer = "book-images";

        public BookService(IBookRepository bookRepository, IMapper mapper, ILogger<BookService> logger, IAuthorService authorService,
            IBlobStorageService blobStorageService)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authorService = authorService ?? throw new ArgumentNullException(nameof(authorService));
            _blobStorageService = blobStorageService;
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

            var existingAuthor = await _authorService.GetByIdAsync(createBookDTO.AuthorId);

            if (existingAuthor == null)
            {
                _logger.LogWarning($"Author with id: {createBookDTO.AuthorId} doesn't exist");
                return Result<GetBookDTO>.Failure(Errors.AuthorNotFound);

            }

            if (createBookDTO.Picture != null)
            {
                var blobName = await _blobStorageService.UploadImageAsync(createBookDTO.Picture, BookImageContainer);
                bookToCreate.PictureBlobName = blobName;
                bookToCreate.PictureUrl = _blobStorageService.GetImageUrl(blobName, BookImageContainer);

                //var pictureResult = await ConvertIFormFileToByteArray(createBookDTO.Picture);

                //if (pictureResult.IsFailure)
                //{
                //    _logger.LogWarning($"Failed to convert picture: {pictureResult.Error.Description}");
                //    return Result<GetBookDTO>.Failure(pictureResult.Error);
                //}

                //bookToCreate.PictureSource = pictureResult.Value;
            }

            var book = await _bookRepository.AddAsync(bookToCreate);

            if (book == null)
            {
                if (!string.IsNullOrEmpty(bookToCreate.PictureBlobName))
                {
                    await _blobStorageService.DeleteImageAsync(
                        bookToCreate.PictureBlobName,
                        BookImageContainer);
                }
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
            if (bookToUpdate.PictureBlobName == null)
            {
                var blobName = await _blobStorageService.UploadImageAsync(updateBookDTO.Picture, BookImageContainer);
                bookToUpdate.PictureBlobName = blobName; bookToUpdate.PictureBlobName = string.Empty;
            }

            string oldBlobName = bookToUpdate.PictureBlobName;
            updateBookDTO.Id = bookToUpdate.Id;
            _mapper.Map(updateBookDTO, bookToUpdate);

            if (updateBookDTO.Picture != null)
            {
                var blobName = await _blobStorageService.UploadImageAsync(updateBookDTO.Picture, BookImageContainer);
                bookToUpdate.PictureBlobName = blobName;
                bookToUpdate.PictureUrl = _blobStorageService.GetImageUrl(blobName, BookImageContainer);

                // Delete old image after successful upload
                if (!string.IsNullOrEmpty(oldBlobName))
                {
                    await _blobStorageService.DeleteImageAsync(
                        oldBlobName,
                        BookImageContainer);
                }
                //var pictureResult = await ConvertIFormFileToByteArray(updateBookDTO.Picture);

                //if (pictureResult.IsFailure)
                //{
                //    _logger.LogWarning($"Failed to convert picture during update: {pictureResult.Error.Description}");
                //    return Result<GetBookDTO>.Failure(pictureResult.Error);
                //}

                //bookToUpdate.PictureSource = pictureResult.Value;
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

            if (!string.IsNullOrEmpty(bookToDelete.PictureBlobName))
            {
                await _blobStorageService.DeleteImageAsync(
                    bookToDelete.PictureBlobName,
                    BookImageContainer);
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

            string imageUrl = string.Empty;

            if (!string.IsNullOrEmpty(book.PictureBlobName))
            {
                imageUrl = _blobStorageService.GetImageUrl(book.PictureBlobName, "book-images");
            }

            var getBookDTO = _mapper.Map<GetBookDTO>(book);
            getBookDTO.PictureUrl = imageUrl;

            return Result<GetBookDTO>.Success(getBookDTO);
        }
        public async Task<Result<IEnumerable<GetBookDTO>>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();

            if (books == null || !books.Any())
            {
                _logger.LogInformation("No books found in repository.");
                return Result<IEnumerable<GetBookDTO>>.Success(Enumerable.Empty<GetBookDTO>());
            }

            foreach (var book in books)
            {
                if (!string.IsNullOrEmpty(book.PictureBlobName))
                {
                    book.PictureUrl = _blobStorageService.GetImageUrl(book.PictureBlobName, BookImageContainer);
                }
            }
            var bookDTOs = _mapper.Map<IEnumerable<GetBookDTO>>(books);
            foreach (var bookDTO in bookDTOs)
            {
                var correspondingBook = books.FirstOrDefault(b => b.Id == bookDTO.Id);
                if (correspondingBook != null)
                {
                    bookDTO.PictureUrl = correspondingBook.PictureUrl;
                }
            }

            return Result<IEnumerable<GetBookDTO>>.Success(bookDTOs);
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

            foreach (var book in filteredBooks)
            {
                if (!string.IsNullOrEmpty(book.PictureBlobName))
                {
                    book.PictureUrl = _blobStorageService.GetImageUrl(book.PictureBlobName, BookImageContainer);
                }
            }

            var bookDTOs = _mapper.Map<IEnumerable<GetBookDTO>>(filteredBooks);

            foreach (var bookDTO in bookDTOs)
            {
                var correspondingBook = filteredBooks.FirstOrDefault(b => b.Id == bookDTO.Id);
                if (correspondingBook != null)
                {
                    bookDTO.PictureUrl = correspondingBook.PictureUrl;
                }
            }

            return Result<IEnumerable<GetBookDTO>>.Success(bookDTOs);
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

        
        public async Task<Result<string>> GetBookPictureAsync(Guid bookId)
        {
            if (bookId == Guid.Empty)
            {
                _logger.LogWarning("GetBookPictureAsync called with empty GUID.");
                return Result<string>.Failure(Errors.NullData);
            }

            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null)
            {
                _logger.LogInformation($"Book with ID {bookId} not found when retrieving picture.");
                return Result<string>.Failure(Errors.BookNotFound);
            }

            if (string.IsNullOrEmpty(book.PictureBlobName))
            {
                _logger.LogInformation($"Book with ID {bookId} has no picture.");
                return Result<string>.Failure(Errors.PictureNotFound);
            }

            var imageUrl = _blobStorageService.GetImageUrl(
                book.PictureBlobName,
                BookImageContainer ?? "book-images"
            );

            return Result<string>.Success(imageUrl);
        }
    }
}
