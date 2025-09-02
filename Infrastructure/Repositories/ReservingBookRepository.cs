using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReservingBookRepository : IReservingBookRepository
    {
        public Task<bool> CancelReservationAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Reservation>> GetActiveReservationsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Reservation>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Reservation> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation> ReserveBookAsync(Reservation reservation)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation> UpdateStatusAsync(Reservation reservation)
        {
            throw new NotImplementedException();
        }
    }
}
