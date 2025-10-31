using Abstractions.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReservingBookRepository : IReservingBookRepository
    {
        private readonly LibraryContext _libraryContext;
        public ReservingBookRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public async Task<bool> ReturnBookAsync(Guid id)
        {
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();
            try
            {
                var reservationToCancel = await _libraryContext.Reservations.FindAsync(id);
                var book = await _libraryContext.Books.FindAsync(reservationToCancel.BookId);

                book.IsAvailable = true;
                reservationToCancel.IsReturned = true;
                //var user = await _libraryContext.Users.FindAsync(reservationToCancel.UserId);

                //user.ReservedBooks.Remove(reservationToCancel);

                await _libraryContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync()
        {
            _libraryContext.Reservations.AsNoTracking();

            var activeReservations = await _libraryContext.Reservations.Where(r => r.IsReturned == false).ToListAsync();

            return activeReservations;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            _libraryContext.Reservations.AsNoTracking();

            var reservations = await _libraryContext.Reservations.ToListAsync();

            return reservations;
        }

        public async Task<Reservation> GetByIdAsync(Guid id)
        {
            var reservation = await _libraryContext.Reservations.FirstOrDefaultAsync(r => r.Id == id);

            return reservation;
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid userId)
        {
            var reservations = await _libraryContext.Reservations.Where(r => r.UserId == userId).ToListAsync();

            return reservations;
        }
        public async Task<IEnumerable<Reservation>> GetReturnedByUserIdAsync(Guid userId)
        {
            _libraryContext.Reservations.AsNoTracking();

            var returnedReservations = await _libraryContext.Reservations
                .Where(r => r.UserId == userId && r.IsReturned == true)
                .Include(r => r.Book)
                .ToListAsync();

            return returnedReservations;
        }
        public async Task<IEnumerable<Reservation>> GetActiveByUserIdAsync(Guid userId)
        {
            _libraryContext.Reservations.AsNoTracking();

            var activeReservations = await _libraryContext.Reservations
                .Where(r => r.UserId == userId && r.IsReturned == false)
                .Include(r => r.Book)
                .ToListAsync();

            return activeReservations;
        }

        public async Task<Reservation> ReserveBookAsync(Reservation reservation)
        {
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();
            try
            {
                reservation.Book = await _libraryContext.Books.FirstOrDefaultAsync(b => b.Id == reservation.BookId);
                reservation.User = await _libraryContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == reservation.UserId);
                reservation.Book.IsAvailable = false;
                reservation.User.ReservedBooks.Add(reservation);

                var createdReservation = _libraryContext.Reservations.Add(reservation).Entity;

                await _libraryContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return createdReservation;
            }
            catch 
            {
                await transaction.RollbackAsync();
                throw new BookReservationException("Unable to reserve book");
            }
        }
        public async Task<bool> IsReservationExists(Reservation reservation)
        {
            return await _libraryContext.Reservations.AnyAsync(r => r.UserId == reservation.UserId && r.BookId == reservation.BookId && r.IsReturned == false);
        }

        
    }
}
