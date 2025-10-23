using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    /// <summary>
    /// Represents the types of notifications that can be sent by the library system.
    /// </summary>
    /// <remarks>This enumeration defines the various categories of notifications that the system can
    /// generate. Each value corresponds to a specific type of notification, such as reminders, alerts, or
    /// updates.</remarks>
    public enum NotificationType
    {
        BookAvailable = 1,
        BookDueReminder,
        NewBookArrival,
        LibraryCardExpiry,
        GeneralUpdate,
        SystemAlert,
        ReservationConfirmation
    }
}
