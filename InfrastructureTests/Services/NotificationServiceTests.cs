using Abstractions.Repositories;
using Application.DTOs.Notitfications;
using Application.ErrorHandling;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureTests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _notificationRepoMock;
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly Mock<IReservingBookRepository> _reservingBookRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<NotificationService>> _loggerMock;

        private readonly NotificationService _service;

        public NotificationServiceTests()
        {
            _notificationRepoMock = new Mock<INotificationRepository>();
            _bookRepoMock = new Mock<IBookRepository>();
            _reservingBookRepoMock = new Mock<IReservingBookRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<NotificationService>>();

            _service = new NotificationService(
                _notificationRepoMock.Object,
                _reservingBookRepoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _bookRepoMock.Object);
        }

        // ---------------------------- //
        //       CREATE NOTIFICATION    //
        // ---------------------------- //

        [Fact]
        public async Task CreateNotification_ShouldReturnFailure_WhenDtoIsNull()
        {
            var result = await _service.CreateNotification(null);

            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.NullData, result.Error);
        }

        [Fact]
        public async Task CreateNotification_ShouldReturnSuccess_WhenCreatedSuccessfully()
        {
            // Arrange
            var dto = new CreateNotificationDTO { Message = "Test" };
            var entity = new Notification();
            var createdEntity = new Notification();
            var mappedDto = new GetNotificationDTO();

            _mapperMock.Setup(m => m.Map<Notification>(dto)).Returns(entity);
            _notificationRepoMock.Setup(r => r.CreateAsync(entity))
                                 .ReturnsAsync(createdEntity);
            _mapperMock.Setup(m => m.Map<GetNotificationDTO>(createdEntity))
                       .Returns(mappedDto);

            // Act
            var result = await _service.CreateNotification(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mappedDto, result.Value);
        }

        [Fact]
        public async Task CreateNotification_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            var dto = new CreateNotificationDTO();
            var entity = new Notification();

            _mapperMock.Setup(m => m.Map<Notification>(dto)).Returns(entity);
            _notificationRepoMock.Setup(r => r.CreateAsync(entity)).ReturnsAsync((Notification)null);

            var result = await _service.CreateNotification(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.NotificationCreationFailed, result.Error);
        }

        // ---------------------------- //
        //     GET USER NOTIFICATIONS   //
        // ---------------------------- //

        [Fact]
        public async Task GetUserNotifications_ShouldReturnEmpty_WhenUserIdIsEmpty()
        {
            var result = await _service.GetUserNotificationsAsync(Guid.Empty);

            Assert.False(result.Any());
        }

        [Fact]
        public async Task GetUserNotifications_ShouldReturnMappedList_WhenValid()
        {
            var userId = Guid.NewGuid();
            var notifications = new List<Notification> { new Notification() };
            var mappedList = new List<GetNotificationDTO> { new GetNotificationDTO() };

            _notificationRepoMock.Setup(r => r.GetUserNotificationsAsync(userId))
                                 .ReturnsAsync(notifications);
            _mapperMock.Setup(m => m.Map<IEnumerable<GetNotificationDTO>>(notifications))
                       .Returns(mappedList);

            var result = await _service.GetUserNotificationsAsync(userId);

            Assert.True(result.Any());
        }

        // ---------------------------- //
        //     MARK AS READ             //
        // ---------------------------- //

        [Fact]
        public async Task MarkNotificationAsRead_ShouldReturnFailure_WhenIdIsEmpty()
        {
            var result = await _service.MarkNotificationAsReadAsync(Guid.Empty);
            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.NullData, result.Error);
        }

        [Fact]
        public async Task MarkNotificationAsRead_ShouldReturnFailure_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _notificationRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Notification)null);

            var result = await _service.MarkNotificationAsReadAsync(id);

            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.NotificationNotFound, result.Error);
        }

        [Fact]
        public async Task MarkNotificationAsRead_ShouldReturnSuccess_WhenUpdated()
        {
            var id = Guid.NewGuid();
            var notification = new Notification { Id = id };
            var mappedDto = new GetNotificationDTO();

            _notificationRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(notification);
            _notificationRepoMock.Setup(r => r.MarkNotificationAsReadAsync(id)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<GetNotificationDTO>(notification)).Returns(mappedDto);

            var result = await _service.MarkNotificationAsReadAsync(id);

            Assert.True(result.IsSuccess);
        }

        // ---------------------------- //
        //       DELETE NOTIFICATION    //
        // ---------------------------- //

        [Fact]
        public async Task DeleteNotification_ShouldReturnFailure_WhenIdIsEmpty()
        {
            var result = await _service.DeleteNotificationAsync(Guid.Empty);

            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.NullData, result.Error);
        }

        [Fact]
        public async Task DeleteNotification_ShouldReturnSuccess_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _notificationRepoMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

            var result = await _service.DeleteNotificationAsync(id);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteNotification_ShouldReturnFailure_WhenDeleteFails()
        {
            var id = Guid.NewGuid();
            _notificationRepoMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(false);

            var result = await _service.DeleteNotificationAsync(id);

            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.NotificationDeletionFailed, result.Error);
        }

        // ---------------------------- //
        //   SEND BOOK AVAILABLE TESTS  //
        // ---------------------------- //

        [Fact]
        public async Task SendBookAvailableNotification_ShouldReturnFailure_WhenBookNotFound()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _bookRepoMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync((Book)null);

            var result = await _service.SendBookAvailableNotificationAsync(userId, bookId);

            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.BookNotFound, result.Error);
        }

        [Fact]
        public async Task SendBookAvailableNotification_ShouldReturnSuccess_WhenValid()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Test Book" };
            var notification = new Notification();
            var mappedDto = new GetNotificationDTO();

            _bookRepoMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
            _notificationRepoMock.Setup(r => r.CreateAsync(It.IsAny<Notification>()))
                                 .ReturnsAsync(notification);
            _mapperMock.Setup(m => m.Map<GetNotificationDTO>(notification))
                       .Returns(mappedDto);

            var result = await _service.SendBookAvailableNotificationAsync(userId, bookId);

            Assert.True(result.IsSuccess);
        }
    }
}
