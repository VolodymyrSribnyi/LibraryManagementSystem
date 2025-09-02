using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        public Task<Notification> CreateAsync(Notification notification)
        {
            throw new NotImplementedException();
        }

        public Task<Notification> GetByIdAsync(Guid notificationId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkNotificationAsReadAsync(Guid notificationId)
        {
            throw new NotImplementedException();
        }
    }
}
