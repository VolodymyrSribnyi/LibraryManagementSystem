using Abstractions.Repositories;
using Application.DTOs.Reservations;
using Application.ErrorHandling;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Events;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureTests.Services
{
    public class ReservingBookServiceTests
    {
        private readonly Mock<IReservingBookRepository> _reservingBookRepositoryMock = new();
        private readonly Mock<IBookRepository> _bookRepositoryMock = new();
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<ReservingBookService>> _loggerMock = new();
        private readonly Mock<IDomainEventPublisher> _domainEventPublisherMock = new();
        private readonly Mock<INotificationService> _notificationServiceMock = new();

        private readonly ReservingBookService _service;

        public ReservingBookServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _service = new ReservingBookService(
                _reservingBookRepositoryMock.Object,
                _mapperMock.Object,
                _bookRepositoryMock.Object,
                _userManagerMock.Object,
                _loggerMock.Object,
                _domainEventPublisherMock.Object,
                _notificationServiceMock.Object
            );
        }

        // -----------------------------
        // ReserveBookAsync tests
        // -----------------------------
        [Fact]
        public async Task ReserveBookAsync_ShouldReturnSuccess_WhenReservationIsCreated()
        {
            var dto = new CreateReservationDTO { UserId = Guid.NewGuid(), BookId = Guid.NewGuid() };
            var reservation = new Reservation { Id = Guid.NewGuid(), UserId = dto.UserId, BookId = dto.BookId };
            var user = new ApplicationUser { Id = dto.UserId };
            var book = new Book { Id = dto.BookId, Title = "Clean Code" };

            _mapperMock.Setup(m => m.Map<Reservation>(dto)).Returns(reservation);
            _reservingBookRepositoryMock.Setup(r => r.IsReservationExists(reservation)).ReturnsAsync(false);
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(dto.BookId)).ReturnsAsync(book);
            _userManagerMock.Setup(u => u.FindByIdAsync(dto.UserId.ToString())).ReturnsAsync(user);
            _reservingBookRepositoryMock.Setup(r => r.ReserveBookAsync(reservation)).ReturnsAsync(reservation);
            _mapperMock.Setup(m => m.Map<GetReservationDTO>(reservation)).Returns(new GetReservationDTO { Id = reservation.Id });

            var result = await _service.ReserveBookAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            _notificationServiceMock.Verify(n => n.SendReservationConfirmationAsync(reservation.Id, user.Id, book.Id), Times.Once);
        }

        [Fact]
        public async Task ReserveBookAsync_ShouldFail_WhenReservationAlreadyExists()
        {
            var dto = new CreateReservationDTO { UserId = Guid.NewGuid(), BookId = Guid.NewGuid() };
            var reservation = new Reservation { UserId = dto.UserId, BookId = dto.BookId };

            _mapperMock.Setup(m => m.Map<Reservation>(dto)).Returns(reservation);
            _reservingBookRepositoryMock.Setup(r => r.IsReservationExists(reservation)).ReturnsAsync(true);

            var result = await _service.ReserveBookAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.ReservationExists, result.Error);
        }

        [Fact]
        public async Task ReserveBookAsync_ShouldFail_WhenBookNotFound()
        {
            var dto = new CreateReservationDTO { UserId = Guid.NewGuid(), BookId = Guid.NewGuid() };
            var reservation = new Reservation { UserId = dto.UserId, BookId = dto.BookId };

            _mapperMock.Setup(m => m.Map<Reservation>(dto)).Returns(reservation);
            _reservingBookRepositoryMock.Setup(r => r.IsReservationExists(reservation)).ReturnsAsync(false);
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(dto.BookId)).ReturnsAsync((Book?)null);

            var result = await _service.ReserveBookAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.BookNotFound, result.Error);
        }

        [Fact]
        public async Task ReserveBookAsync_ShouldFail_WhenUserNotFound()
        {
            var dto = new CreateReservationDTO { UserId = Guid.NewGuid(), BookId = Guid.NewGuid() };
            var reservation = new Reservation { UserId = dto.UserId, BookId = dto.BookId };
            var book = new Book { Id = dto.BookId };

            _mapperMock.Setup(m => m.Map<Reservation>(dto)).Returns(reservation);
            _reservingBookRepositoryMock.Setup(r => r.IsReservationExists(reservation)).ReturnsAsync(false);
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(dto.BookId)).ReturnsAsync(book);
            _userManagerMock.Setup(u => u.FindByIdAsync(dto.UserId.ToString())).ReturnsAsync((ApplicationUser?)null);

            var result = await _service.ReserveBookAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.UserNotFound, result.Error);
        }

        // -----------------------------
        // ReturnBookAsync tests
        // -----------------------------
        [Fact]
        public async Task ReturnBookAsync_ShouldReturnSuccess_WhenBookIsReturned()
        {
            var reservationId = Guid.NewGuid();
            var reservation = new Reservation { Id = reservationId, BookId = Guid.NewGuid(), Book = new Book { Id = Guid.NewGuid(), Title = "Book" } };

            _reservingBookRepositoryMock.Setup(r => r.GetByIdAsync(reservationId)).ReturnsAsync(reservation);
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(reservation.BookId)).ReturnsAsync(reservation.Book);
            _reservingBookRepositoryMock.Setup(r => r.ReturnBookAsync(reservationId)).ReturnsAsync(true);

            var result = await _service.ReturnBookAsync(reservationId);

            Assert.True(result.IsSuccess);
            _domainEventPublisherMock.Verify(p => p.PublishAsync(It.IsAny<BookBecameAvailableEvent>()), Times.Once);
        }

        [Fact]
        public async Task ReturnBookAsync_ShouldFail_WhenReservationNotFound()
        {
            var reservationId = Guid.NewGuid();
            _reservingBookRepositoryMock.Setup(r => r.GetByIdAsync(reservationId)).ReturnsAsync((Reservation?)null);

            var result = await _service.ReturnBookAsync(reservationId);

            Assert.False(result.IsSuccess);
            Assert.Equal(Errors.ReservationNotFound, result.Error);
        }

        // -----------------------------
        // GetByUserIdAsync tests
        // -----------------------------
        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnReservations_WhenTheyExist()
        {
            var userId = Guid.NewGuid();
            var reservations = new List<Reservation> { new() { Id = Guid.NewGuid(), UserId = userId } };
            var mapped = new List<GetReservationDTO> { new() { Id = reservations[0].Id } };

            _reservingBookRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(reservations);
            _mapperMock.Setup(m => m.Map<IEnumerable<GetReservationDTO>>(reservations)).Returns(mapped);

            var result = await _service.GetByUserIdAsync(userId);

            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnEmpty_WhenUserIdIsEmpty()
        {
            var result = await _service.GetByUserIdAsync(Guid.Empty);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnEmpty_WhenNoReservationsFound()
        {
            var userId = Guid.NewGuid();
            _reservingBookRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new List<Reservation>());

            var result = await _service.GetByUserIdAsync(userId);

            Assert.Empty(result);
        }

        // -----------------------------
        // GetReturnedByUserIdAsync tests
        // -----------------------------
        [Fact]
        public async Task GetReturnedByUserIdAsync_ShouldReturnReservations_WhenTheyExist()
        {
            var userId = Guid.NewGuid();
            var reservations = new List<Reservation> { new() { Id = Guid.NewGuid(), UserId = userId } };
            var mapped = new List<GetReservationDTO> { new() { Id = reservations[0].Id } };

            _reservingBookRepositoryMock.Setup(r => r.GetReturnedByUserIdAsync(userId)).ReturnsAsync(reservations);
            _mapperMock.Setup(m => m.Map<IEnumerable<GetReservationDTO>>(reservations)).Returns(mapped);

            var result = await _service.GetReturnedByUserIdAsync(userId);

            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetReturnedByUserIdAsync_ShouldReturnEmpty_WhenUserIdIsEmpty()
        {
            var result = await _service.GetReturnedByUserIdAsync(Guid.Empty);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetReturnedByUserIdAsync_ShouldReturnEmpty_WhenNoReservationsFound()
        {
            var userId = Guid.NewGuid();
            _reservingBookRepositoryMock.Setup(r => r.GetReturnedByUserIdAsync(userId)).ReturnsAsync(new List<Reservation>());

            var result = await _service.GetReturnedByUserIdAsync(userId);

            Assert.Empty(result);
        }

        // -----------------------------
        // GetActiveByUserIdAsync tests
        // -----------------------------
        [Fact]
        public async Task GetActiveByUserIdAsync_ShouldReturnReservations_WhenTheyExist()
        {
            var userId = Guid.NewGuid();
            var reservations = new List<Reservation> { new() { Id = Guid.NewGuid(), UserId = userId } };
            var mapped = new List<GetReservationDTO> { new() { Id = reservations[0].Id } };

            _reservingBookRepositoryMock.Setup(r => r.GetActiveByUserIdAsync(userId)).ReturnsAsync(reservations);
            _mapperMock.Setup(m => m.Map<IEnumerable<GetReservationDTO>>(reservations)).Returns(mapped);

            var result = await _service.GetActiveByUserIdAsync(userId);

            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetActiveByUserIdAsync_ShouldReturnEmpty_WhenUserIdIsEmpty()
        {
            var result = await _service.GetActiveByUserIdAsync(Guid.Empty);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetActiveByUserIdAsync_ShouldReturnEmpty_WhenNoReservationsFound()
        {
            var userId = Guid.NewGuid();
            _reservingBookRepositoryMock.Setup(r => r.GetActiveByUserIdAsync(userId)).ReturnsAsync(new List<Reservation>());

            var result = await _service.GetActiveByUserIdAsync(userId);

            Assert.Empty(result);
        }
    }
}
