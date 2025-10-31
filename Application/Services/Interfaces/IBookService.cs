using Application.DTOs.Authors;
using Application.DTOs.Books;
using Application.ErrorHandling;
using Application.Filters;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{

    /// <summary>
    /// Defines a contract for managing book-related operations, including creating, updating, retrieving, and
    /// deleting books.
    /// </summary>
    /// <remarks>This service provides methods for performing CRUD operations on books, as well as filtering,
    /// searching, and managing book-related data such as availability, ratings, and pictures. Implementations of this
    /// interface are expected to handle the underlying data access and business logic.</remarks>
    public interface IBookService
    {
        /// <summary>
        /// Creates a new book in the system.
        /// </summary>
        /// <param name="createBookDTO">The data transfer object containing the information for the new book.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetBookDTO"/> representing the created book.</returns>
        Task<Result<GetBookDTO>> AddAsync(CreateBookDTO createBookDTO);

        /// <summary>
        /// Updates an existing book's information.
        /// </summary>
        /// <param name="updateBookDTO">The data transfer object containing the updated book information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetBookDTO"/> representing the updated book.</returns>
        Task<Result<GetBookDTO>> UpdateAsync(UpdateBookDTO updateBookDTO);

        /// <summary>
        /// Updates the availability status of a book.
        /// </summary>
        /// <param name="updateBookStatusDTO">The data transfer object containing the book's updated availability status.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the availability was successfully updated; otherwise, <see langword="false"/>.</returns>
        Task<Result> UpdateAvailabilityAsync(UpdateBookStatusDTO updateBookStatusDTO);

        /// <summary>
        /// Updates the rating of a book.
        /// </summary>
        /// <param name="updateBookRatingDTO">The data transfer object containing the book's updated rating information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the rating was successfully updated; otherwise, <see langword="false"/>.</returns>
        Task<Result> UpdateRatingAsync(UpdateBookRatingDTO updateBookRatingDTO);

        /// <summary>
        /// Deletes a book from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the book to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the book was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<Result> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves books that match the specified filter criteria.
        /// </summary>
        /// <param name="bookFilter">The filter object containing the criteria for filtering books.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="GetBookDTO"/> objects that match the filter criteria.</returns>
        Task<Result<IEnumerable<GetBookDTO>>> GetFilteredAsync(BookFilter bookFilter);

        /// <summary>
        /// Retrieves a book by its title.
        /// </summary>
        /// <param name="title">The title of the book to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetBookDTO"/> representing the book if found; otherwise, <see langword="null"/>.</returns>
        Task<Result<GetBookDTO>> GetByTitleAsync(string title);

        /// <summary>
        /// Retrieves a book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the book to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetBookDTO"/> representing the book if found; otherwise, <see langword="null"/>.</returns>
        Task<Result<GetBookDTO>> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all books in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="GetBookDTO"/> objects representing all books.</returns>
        Task<Result<IEnumerable<GetBookDTO>>> GetAllAsync();

        /// <summary>
        /// Retrieves all books written by a specific author.
        /// </summary>
        /// <param name="getAuthorDTO">The data transfer object representing the author whose books to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="GetBookDTO"/> objects representing the author's books.</returns>
        Task<Result<IEnumerable<GetBookDTO>>> GetAllByAuthorAsync(GetAuthorDTO getAuthorDTO);

        /// <summary>
        /// Retrieves all books published by the specified publishers.
        /// </summary>
        /// <param name="publishers">A collection of publisher names to filter by.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="GetBookDTO"/> objects from the specified publishers.</returns>
        Task<Result<IEnumerable<GetBookDTO>>> GetAllByPublisherAsync(IEnumerable<string> publishers);

        /// <summary>
        /// Retrieves all books that belong to the specified genres.
        /// </summary>
        /// <param name="genres">A collection of genres to filter by.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="GetBookDTO"/> objects that match the specified genres.</returns>
        Task<Result<IEnumerable<GetBookDTO>>> GetAllByGenresAsync(IEnumerable<Genre> genres);

        /// <summary>
        /// Retrieves the picture associated with a specific book.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book whose picture to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a byte array representing the book's picture data.</returns>
        Task<Result<byte[]>> GetBookPictureAsync(Guid bookId);
    }
}
