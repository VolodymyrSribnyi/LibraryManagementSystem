using Application.DTOs.Authors;
using Application.DTOs.Books;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        }
        public async Task<GetBookDTO> AddAsync(CreateBookDTO createBookDTO)
        {
            var bookToCreate = new Book
            {
                Id = Guid.NewGuid(),
                Title = createBookDTO.Title,
                AuthorId = createBookDTO.AuthorId,
                Author = createBookDTO.Author,
                Genre = createBookDTO.Genre,
                Publisher = createBookDTO.Publisher,
                PublishingYear = createBookDTO.PublishingYear
            };

            var book = await _bookRepository.AddAsync(bookToCreate);

            if (book == null)
            {
                throw new Exception("Failed to create book");
            }

            return _mapper.Map<GetBookDTO>(book);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var bookToDelete = await _bookRepository.GetByIdAsync(id);

            if (bookToDelete == null)
            {
                throw new Exception("Book not found");
            }

            bookToDelete.IsDeleted = true;

            var book = await _bookRepository.DeleteAsync(bookToDelete.Id);

            if (!book)
            {
                throw new Exception("Failed to delete book");
            }

            return true;
        }

        public async Task<IEnumerable<GetBookDTO>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();

            if (books == null || !books.Any())
            {
                throw new Exception("No books found");
            }

            return _mapper.Map<IEnumerable<GetBookDTO>>(books);
        }

        public async Task<IEnumerable<GetBookDTO>> GetAllByAuthorAsync(GetAuthorDTO getAuthorDTO)
        {
            var books = await _bookRepository.GetAllByAuthorAsync(new Author
            {
                Id = getAuthorDTO.Id,
                FirstName = getAuthorDTO.FirstName,
                Surname = getAuthorDTO.Surname,
                MiddleName = getAuthorDTO.MiddleName,
                Age = getAuthorDTO.Age
            });

            if (books == null || !books.Any())
            {
                throw new Exception("No books found for the specified author");
            }

            return _mapper.Map<IEnumerable<GetBookDTO>>(books);
        }

        public async Task<IEnumerable<GetBookDTO>> GetAllByGenresAsync(IEnumerable<Genre> genres)
        {
            var books = await _bookRepository.GetAllByGenresAsync(genres);

            if (books == null || !books.Any())
            {
                throw new Exception("No books found for the specified genres");
            }

            return _mapper.Map<IEnumerable<GetBookDTO>>(books);
        }

        public async Task<IEnumerable<GetBookDTO>> GetAllByPublisherAsync(IEnumerable<string> publishers)
        {
            var books = await _bookRepository.GetAllByPublisherAsync(publishers);

            if (books == null || !books.Any())
            {
                throw new Exception("No books found for the specified publishers");
            }

            return _mapper.Map<IEnumerable<GetBookDTO>>(books);
        }

        public async Task<GetBookDTO> GetByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                throw new Exception("Book not found");
            }

            return _mapper.Map<GetBookDTO>(book);
        }

        public async Task<GetBookDTO> GetByTitleAsync(string title)
        {
            var book = await _bookRepository.GetByTitleAsync(title);

            if (book == null)
            {
                throw new Exception("Book not found");
            }

            return _mapper.Map<GetBookDTO>(book);
        }

        public async Task<GetBookDTO> UpdateAsync(UpdateBookDTO updateBookDTO)
        {
            var bookExists = await _bookRepository.GetByIdAsync(updateBookDTO.Id);

            if (bookExists == null)
                throw new Exception();

            _mapper.Map(updateBookDTO,bookExists);

            var updatedBook = await _bookRepository.UpdateAsync(bookExists);

            if (updatedBook == null)
            {
                throw new Exception("Failed to update book");
            }

            return _mapper.Map<GetBookDTO>(bookExists);
        }

        public async Task<bool> UpdateAvailabilityAsync(UpdateBookStatusDTO updateBookStatusDTO)
        {
            var bookToUpdate = new Book
            {
                Id = updateBookStatusDTO.BookId,
                IsAvailable = updateBookStatusDTO.IsAvailable
            };

            var result = await _bookRepository.UpdateAvailabilityAsync(bookToUpdate);

            if (!result)
            {
                throw new Exception("Failed to update book availability");
            }
            return true;
        }

        public async Task<bool> UpdateRatingAsync(UpdateBookRatingDTO updateBookRatingDTO)
        {
            var bookToUpdate = new Book
            {
                Id = updateBookRatingDTO.BookId,
                Rating = updateBookRatingDTO.Rating
            };

            var result = await _bookRepository.UpdateRatingAsync(bookToUpdate);

            if (!result)
            {
                throw new Exception("Failed to update book rating");
            }

            return true;
        }
    }
}
