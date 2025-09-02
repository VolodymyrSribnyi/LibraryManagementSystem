using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
//    +Id : Guid
//+UserId: Guid
//~User: IUser
//+ IsValid: bool
//+ ValidTo : DateTime
    public class LibraryCard
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public bool IsValid { get; set; }
        public DateTime ValidTo { get; set; }

    }
}
