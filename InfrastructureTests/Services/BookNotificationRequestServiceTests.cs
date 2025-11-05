using Application.DTOs.BookNotificationRequests;
using Application.DTOs.Books;
using Application.ErrorHandling;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace InfrastructureTests.Services
{
    public class BookNotificationRequestServiceTests
    {
        private readonly BookNotificationRequestService _service;
        private readonly Mock<IBookService> _bookService;
        private readonly Mock<IBookNotificationRequestRepository> _bookRequestRepository;
        private readonly Mock<ILogger<BookNotificationRequestService>> _logger;
        private readonly Mock<IMapper> _mapper;
        public BookNotificationRequestServiceTests()
        {
            _bookService = new Mock<IBookService>();
            _bookRequestRepository = new Mock<IBookNotificationRequestRepository>();
            _logger = new Mock<ILogger<BookNotificationRequestService>>();
            _mapper = new Mock<IMapper>();
            _service = new BookNotificationRequestService(_bookService.Object, _bookRequestRepository.Object,
                _logger.Object, _mapper.Object);
        }
        [Fact]
        public async Task CreateBookNotificationAsync_WithEmptyUserId_ReturnsFailureResult()
        {
            // Arrange
            var emptyUserId = Guid.Empty;
            var validBookId = Guid.NewGuid();
            // Act
            var result = await _service.CreateBookNotificationAsync(emptyUserId, validBookId);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task CreateBookNotificationAsync_WithEmptyBookId_ReturnsFailureResult()
        {
            // Arrange
            var validUserId = Guid.NewGuid();
            var emptyBookId = Guid.Empty;
            // Act
            var result = await _service.CreateBookNotificationAsync(validUserId, emptyBookId);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task CreateBookNotificationAsync_UserIsSubscribed_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookService.Setup(service => service.GetByIdAsync(bookId))
                .ReturnsAsync(Result<GetBookDTO>.Success(new GetBookDTO { Id = bookId }));
            _bookRequestRepository.Setup(repo => repo.IsUserSubscribedAsync(userId, bookId))
                .ReturnsAsync(true);
            // Act
            var result = await _service.CreateBookNotificationAsync(userId, bookId);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task CreateBookNotificationAsync_ValidInput_ReturnsSuccessResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookService.Setup(service => service.GetByIdAsync(bookId))
                .ReturnsAsync(Result<GetBookDTO>.Success(new GetBookDTO { Id = bookId }));
            _bookRequestRepository.Setup(repo => repo.IsUserSubscribedAsync(userId, bookId))
                .ReturnsAsync(false);
            _bookRequestRepository.Setup(repo => repo.CreateSubscriptionAsync(It.IsAny<BookNotificationRequest>()))
                .ReturnsAsync(new BookNotificationRequest { UserId = userId, BookId = bookId });
            _mapper.Setup(m => m.Map<BookNotificationRequestDTO>(It.IsAny<BookNotificationRequest>()))
                .Returns(new BookNotificationRequestDTO { UserId = userId, BookId = bookId });
            // Act
            var result = await _service.CreateBookNotificationAsync(userId, bookId);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.UserId);
            Assert.Equal(bookId, result.Value.BookId);
        }
        [Fact]
        public async Task IsUserSubscribedAsync_UserIsSubscribed_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookRequestRepository.Setup(repo => repo.IsUserSubscribedAsync(userId, bookId))
                .ReturnsAsync(true);
            // Act
            var result = await _service.IsUserSubscribedAsync(userId, bookId);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }
        [Fact]
        public async Task IsUserSubscribedAsync_UserIsNotSubscribed_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookRequestRepository.Setup(repo => repo.IsUserSubscribedAsync(userId, bookId))
                .ReturnsAsync(false);
            // Act
            var result = await _service.IsUserSubscribedAsync(userId, bookId);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.Value);
        }
        [Fact]
        public async Task GetUnnotifiedSubscribers_GuidEmpty_ReturnsFailureResult()
        {
            // Arrange
            var emptyBookId = Guid.Empty;
            // Act
            var result = await _service.GetUnnotifiedSubscribersAsync(emptyBookId);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task GetUnnotifiedSubscribers_BookNotFound_ReturnsFailureResult()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _bookService.Setup(service => service.GetByIdAsync(bookId))
                .ReturnsAsync(Result<GetBookDTO>.Failure(Errors.BookNotFound));
            // Act
            var result = await _service.GetUnnotifiedSubscribersAsync(bookId);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task GetUnnotifiedSubscribers_ValidBookId_ReturnsSuccessResult()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var subscribers = new List<BookNotificationRequest>
            {
                new BookNotificationRequest { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), BookId = bookId, IsNotified = false },
                new BookNotificationRequest { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), BookId = bookId, IsNotified = false }
            };
            _bookService.Setup(service => service.GetByIdAsync(bookId))
                .ReturnsAsync(Result<GetBookDTO>.Success(new GetBookDTO { Id = bookId }));
            _bookRequestRepository.Setup(repo => repo.GetUnnotifiedSubscribersAsync(bookId))
                .ReturnsAsync(subscribers);
            _mapper.Setup(m => m.Map<IEnumerable<BookNotificationRequestDTO>>(It.IsAny<IEnumerable<BookNotificationRequest>>()))
                .Returns(subscribers.Select(s => new BookNotificationRequestDTO { Id = s.Id, UserId = s.UserId, BookId = s.BookId }));
            // Act
            var result = await _service.GetUnnotifiedSubscribersAsync(bookId);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }
        [Fact]
        public async Task MarkAsNotifiedAsync_GuidEmpty_ReturnsFailureResult()
        {
            // Arrange
            var emptyRequestId = Guid.Empty;
            // Act
            var result = await _service.MarkAsNotifiedAsync(emptyRequestId);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task MarkAsNotifiedAsync_ValidRequestId_ReturnsSuccessResult()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            _bookRequestRepository.Setup(repo => repo.MarkAsNotifiedAsync(requestId))
                .ReturnsAsync(true);
            // Act
            var result = await _service.MarkAsNotifiedAsync(requestId);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }
        [Fact]
        public async Task GetAllSubscriptionsAsync_ReturnsSuccessResult()
        {
            // Arrange
            var subscriptions = new List<BookNotificationRequest>
            {
                new BookNotificationRequest { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), BookId = Guid.NewGuid() },
                new BookNotificationRequest { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), BookId = Guid.NewGuid() }
            };
            _bookRequestRepository.Setup(repo => repo.GetAllSubscriptionsAsync())
                .ReturnsAsync(subscriptions);
            _mapper.Setup(m => m.Map<IEnumerable<BookNotificationRequestDTO>>(It.IsAny<IEnumerable<BookNotificationRequest>>()))
                .Returns(subscriptions.Select(s => new BookNotificationRequestDTO { Id = s.Id, UserId = s.UserId, BookId = s.BookId }));
            // Act
            var result = await _service.GetAllSubscriptionsAsync();
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }
        [Fact]
        public async Task GetUserSubscriptionsAsync_ReturnsSuccessResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var subscriptions = new List<BookNotificationRequest>
            {
                new BookNotificationRequest { Id = Guid.NewGuid(), UserId = userId, BookId = Guid.NewGuid() },
                new BookNotificationRequest { Id = Guid.NewGuid(), UserId = userId, BookId = Guid.NewGuid() }
            };
            _bookRequestRepository.Setup(repo => repo.GetUserSubscriptionsAsync(userId))
                .ReturnsAsync(subscriptions);
            _mapper.Setup(m => m.Map<IEnumerable<BookNotificationRequestDTO>>(It.IsAny<IEnumerable<BookNotificationRequest>>()))
                .Returns(subscriptions.Select(s => new BookNotificationRequestDTO { Id = s.Id, UserId = s.UserId, BookId = s.BookId }));
            // Act
            var result = await _service.GetUserSubscriptionsAsync(userId);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }
        [Fact]
        public async Task GetUserSubscriptionsAsync_GuidEmpty_ReturnsFailureResult()
        {
            // Arrange
            var emptyUserId = Guid.Empty;
            // Act
            var result = await _service.GetUserSubscriptionsAsync(emptyUserId);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task RemoveSubscriptionAsync_EmptyGuid_ReturnsFailureResult()
        {
            // Arrange
            var emptyRequestId = Guid.Empty;
            // Act
            var result = await _service.RemoveSubscriptionAsync(emptyRequestId);
            // Assert
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task RemoveSubscriptionAsync_ValidRequestId_ReturnsSuccessResult()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            _bookRequestRepository.Setup(repo => repo.RemoveSubscriptionAsync(requestId))
                .ReturnsAsync(true);
            // Act
            var result = await _service.RemoveSubscriptionAsync(requestId);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }
    }
}
