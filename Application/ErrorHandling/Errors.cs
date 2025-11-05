using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Application.ErrorHandling
{
    public static class Errors
    {
        public static readonly Error AuthorNotFound = new("AUTHOR_NOT_FOUND", "The specified author was not found.");
        public static readonly Error BookNotFound = new("BOOK_NOT_FOUND", "The specified book was not found.");
        public static readonly Error NullData = new("NULL_DATA", "The provided data is null.");
        public static readonly Error AuthorExists = new("AUTHOR_EXISTS", "The author already exists.");
        public static readonly Error BookExists = new("BOOK_EXISTS", "The book already exists.");
        public static readonly Error InvalidData = new("INVALID_DATA", "The provided data is invalid.");
        public static readonly Error UserSubscribed = new("USER_SUBSCRIBED", "User is already subscribed to notifications for this book.");
        public static readonly Error SubscriptionsNotFound = new Error("SUBSCRIPTIONS_EMPTY", "No subscriptions found in the system.");
        public static readonly Error BookCreationFailed = new Error("BOOK_CREATION_FAILED", "Failed to create the book.");
        public static readonly Error PictureTooLarge = new Error("PICTURE_TOO_LARGE","Picture size exceed");
        public static readonly Error InvalidRating = new Error("INVALID_RATING","Provided rating invalid,must be between 1-5");
        public static readonly Error PictureNotFound = new Error("PICTURE_NOT_FOUND","Picture not found");
        public static readonly Error NotificationCreationFailed = new Error("NOTIFICATION_CREATION_FAILED", "Failed to create the notification request.");
        public static readonly Error NotificationNotFound = new Error("NOTIFICATION_NOT_FOUND", "The specified notification request was not found.");
        public static readonly Error FailedToMarkAsRead = new Error("FAILED_TO_MARK_AS_READ", "Failed to mark the notification as read.");
        public static readonly Error ReservationNotFound = new Error("RESERVATION_NOT_FOUND", "The specified reservation was not found.");
        public static readonly Error ReservationCreationFailed = new Error("RESERVATION_CREATION_FAILED", "Failed to create the reservation.");
        public static readonly Error ReservationExists = new Error("RESERVATION_EXISTS", "The reservation already exists.");
        public static readonly Error UserNotFound = new Error("USER_NOT_FOUND", "The specified user was not found.");
        public static readonly Error FailedToReturnBook = new Error("FAILED_TO_RETURN_BOOK", "Failed to return the book.");
        public static readonly Error UserExists = new Error("USER_EXISTS", "The user already exists.");
        public static readonly Error UsersNotFoundForRole = new Error("USERS_NOT_FOUND_FOR_ROLE", "No users found for the specified role.");
        public static readonly Error InvalidLoginAttempt = new Error("INVALID_LOGIN_ATTEMPT", "Invalid username or password.");
        public static readonly Error UserHasActiveReservations = new Error("USER_HAS_ACTIVE_RESERVATIONS", "The user has active reservations and cannot be deleted.");
        public static readonly Error NotificationDeletionFailed = new Error("NOTIFICATION_DELETION_FAILED", "Failed to delete the notification request.");
    }
}
