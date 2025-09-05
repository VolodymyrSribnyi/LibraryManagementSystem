using Abstractions.Repositories;
using Domain.Entities;
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
        public async Task<bool> CancelReservationAsync(Guid id)
        {
            var reservationToCancel = await _libraryContext.Reservations.FindAsync(id);

            reservationToCancel.IsReturned = true;

            await _libraryContext.SaveChangesAsync();

            return true;
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

        public async Task<Reservation> ReserveBookAsync(Reservation reservation)
        {
            var createdReservation = _libraryContext.Reservations.Add(reservation).Entity;

            await _libraryContext.SaveChangesAsync();

            return createdReservation;
        }

        public async Task<Reservation> UpdateStatusAsync(Reservation reservation)
        {
            var updatedReservation = await _libraryContext.Reservations.FindAsync(reservation.Id);

            updatedReservation.IsReturned = reservation.IsReturned;

            await _libraryContext.SaveChangesAsync();

            return updatedReservation;
        }
    }
}
