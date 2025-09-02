using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
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
