using Abstractions.Repositories;
using Application.DTOs.Books;
using Application.ErrorHandling;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InfrastructureTests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IAuthorRepository> _authorRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BookService>> _loggerMock;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _authorRepositoryMock = new Mock<IAuthorRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BookService>>();

            _bookService = new BookService(
                _bookRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _authorRepositoryMock.Object);
        }

        // -------------------- ADD --------------------

        [Fact]
        public async Task AddAsync_NullCreateBookDTO_ReturnsFailureResult()
        {
            // Act
            var result = await _bookService.AddAsync(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.NullData, result.Error);
        }

        [Fact]
        public async Task AddAsync_BookAlreadyExists_ReturnsFailureResult()
        {
            // Arrange
            var dto = new CreateBookDTO { Title = "Existing Book", AuthorId = Guid.NewGuid() };
            var book = new Book { Title = "Existing Book" };

            _mapperMock.Setup(m => m.Map<Book>(dto)).Returns(book);
            _bookRepositoryMock.Setup(r => r.GetByTitleAsync("Existing Book"))
                .ReturnsAsync(book);

            // Act
            var result = await _bookService.AddAsync(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.BookExists, result.Error);
        }

        [Fact]
        public async Task AddAsync_ValidBook_ReturnsSuccessResult()
        {
            // Arrange
            var dto = new CreateBookDTO
            {
                Title = "New Book",
                AuthorId = Guid.NewGuid(),
                Picture = null
            };

            var mappedBook = new Book { Title = "New Book" };
            var addedBook = new Book { Id = Guid.NewGuid(), Title = "New Book" };
            var mappedGetDto = new GetBookDTO { Title = "New Book" };

            _mapperMock.Setup(m => m.Map<Book>(dto)).Returns(mappedBook);
            _bookRepositoryMock.Setup(r => r.GetByTitleAsync("New Book"))
                .ReturnsAsync((Book)null);
            _authorRepositoryMock.Setup(r => r.GetByIdAsync(dto.AuthorId))
                .ReturnsAsync(new Author { Id = dto.AuthorId });
            _bookRepositoryMock.Setup(r => r.AddAsync(mappedBook))
                .ReturnsAsync(addedBook);
            _mapperMock.Setup(m => m.Map<GetBookDTO>(addedBook))
                .Returns(mappedGetDto);

            // Act
            var result = await _bookService.AddAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("New Book", result.Value.Title);
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_RepositoryReturnsNull_ReturnsFailureResult()
        {
            // Arrange
            var dto = new CreateBookDTO { Title = "Failed Book", AuthorId = Guid.NewGuid() };
            var mappedBook = new Book { Title = "Failed Book" };

            _mapperMock.Setup(m => m.Map<Book>(dto)).Returns(mappedBook);
            _bookRepositoryMock.Setup(r => r.GetByTitleAsync("Failed Book"))
                .ReturnsAsync((Book)null);
            _authorRepositoryMock.Setup(r => r.GetByIdAsync(dto.AuthorId))
                .ReturnsAsync(new Author { Id = dto.AuthorId });
            _bookRepositoryMock.Setup(r => r.AddAsync(mappedBook))
                .ReturnsAsync((Book)null);

            // Act
            var result = await _bookService.AddAsync(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.BookCreationFailed, result.Error);
        }

        // -------------------- GET BY ID --------------------

        [Fact]
        public async Task GetByIdAsync_EmptyGuid_ReturnsFailureResult()
        {
            // Act
            var result = await _bookService.GetByIdAsync(Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.NullData, result.Error);
        }

        [Fact]
        public async Task GetByIdAsync_BookNotFound_ReturnsFailureResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Book)null);

            // Act
            var result = await _bookService.GetByIdAsync(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.BookNotFound, result.Error);
        }

        [Fact]
        public async Task GetByIdAsync_BookFound_ReturnsSuccessResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var book = new Book { Id = id, Title = "Some Book" };
            var getDto = new GetBookDTO { Title = "Some Book" };

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(book);
            _mapperMock.Setup(m => m.Map<GetBookDTO>(book)).Returns(getDto);

            // Act
            var result = await _bookService.GetByIdAsync(id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Some Book", result.Value.Title);
        }

        // -------------------- DELETE --------------------

        [Fact]
        public async Task DeleteAsync_EmptyGuid_ReturnsFailureResult()
        {
            // Act
            var result = await _bookService.DeleteAsync(Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.NullData, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_BookNotFound_ReturnsFailureResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Book)null);

            // Act
            var result = await _bookService.DeleteAsync(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.BookNotFound, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryReturnsFalse_ThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var book = new Book { Id = id };

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _bookService.DeleteAsync(id));
        }

        [Fact]
        public async Task DeleteAsync_Success_ReturnsSuccessResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var book = new Book { Id = id };

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _bookService.DeleteAsync(id);

            // Assert
            Assert.True(result.IsSuccess);
            _bookRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        // -------------------- GET ALL --------------------

        [Fact]
        public async Task GetAllAsync_EmptyList_ReturnsEmptyResult()
        {
            // Arrange
            _bookRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Book>());

            // Act
            var result = await _bookService.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllAsync_WithBooks_ReturnsMappedResult()
        {
            // Arrange
            var books = new List<Book> { new Book { Title = "Book A" } };
            var dtos = new List<GetBookDTO> { new GetBookDTO { Title = "Book A" } };

            _bookRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(books);
            _mapperMock.Setup(m => m.Map<IEnumerable<GetBookDTO>>(books)).Returns(dtos);

            // Act
            var result = await _bookService.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
            Assert.Equal("Book A", result.Value.First().Title);
        }
    }
}
