using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Repositories
{
    //    + SendBookAvailableNotification(userId, bookId) : INotification
    //+ SendReservationConfirmation(reservationId) : INotification
    //+ GetUserNotifications(userId) : List<INotification>
    public interface INotificationRepository
    {
        public Task<Notification> CreateAsync(Notification notification);
        //public Task<Notification> SendBookAvailableAsync(Guid userId, Guid bookId);
        ////{
        ////    // Logic to send notification about book availability
        ////    return new Notification
        ////    {
        ////        UserId = userId,
        ////        BookId = bookId,
        ////        Message = "The book you requested is now available.",
        ////        NotificationType = Enums.NotificationType.BookAvailable
        ////    };
        ////}
        //public Task<Notification> SendReservationConfirmationAsync(Guid reservationId, Guid userId, Guid bookId);
        //{
        //    // Logic to send reservation confirmation
        //    return new Notification
        //    {
        //        Id = reservationId,
        //        Message = "Your reservation has been confirmed.",
        //        NotificationType = Enums.NotificationType.GeneralUpdate
        //    };
        //}
        public Task<bool> MarkNotificationAsReadAsync(Guid notificationId);
        public Task<Notification> GetByIdAsync(Guid notificationId);
        public Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId);

        //{
        //    // Logic to retrieve user notifications
        //    // This is a placeholder implementation
        //    return new List<Notification>
        //    {
        //        new Notification
        //        {
        //            UserId = userId,
        //            Message = "You have a new notification.",
        //            NotificationType = Enums.NotificationType.GeneralUpdate
        //        }
        //    };
        //}
    }
}
