using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a library card issued to a user, providing access to library services.
    /// </summary>
    /// <remarks>A library card is associated with a specific user and includes information about its validity
    /// and status. Use this class to manage and query the state of a library card.</remarks>
    public class LibraryCard
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public bool IsValid { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsDeleted { get; set; }
    }
}
