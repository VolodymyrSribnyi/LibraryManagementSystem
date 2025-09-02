using Application.DTOs.Notitfications;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface INotificationService
    {
        public Task<GetNotificationDTO> CreateNotification(CreateNotificationDTO createNotificationDTO);
        public Task<GetNotificationDTO> SendBookAvailableNotificationAsync(Guid userId, Guid bookId);
        //{
        //    // Logic to send notification about book availability
        //    return new Notification
        //    {
        //        UserId = userId,
        //        BookId = bookId,
        //        Message = "The book you requested is now available.",
        //        NotificationType = Enums.NotificationType.BookAvailable
        //    };
        //}
        public Task<GetNotificationDTO> SendReservationConfirmationAsync(Guid reservationId, Guid userId, Guid bookId);
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
        public Task<IEnumerable<GetNotificationDTO>> GetUserNotificationsAsync(Guid userId);
        public Task<GetNotificationDTO> SendBookDueReminder(Guid userId, Guid bookId);
        public Task<GetNotificationDTO> SendNewBookArrival(Guid userId, Guid bookId);
        public Task<GetNotificationDTO> SendLibraryCardExpiry(Guid userId);
        public Task<GetNotificationDTO> SendGeneralUpdate(Guid userId);
        public Task<GetNotificationDTO> SendSystemAlert(Guid userId);
        //{
    }
}
