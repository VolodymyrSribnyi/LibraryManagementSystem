using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    /// <summary>
    /// Represents the various genres of literature or media.
    /// </summary>
    /// <remarks>This enumeration provides a set of predefined genres that can be used to categorize books,
    /// movies, or other forms of media. Each genre is represented by a unique value, starting from 1.</remarks>
    public enum Genre
    {
        Fiction = 1,
        NonFiction,
        Mystery,
        ScienceFiction,
        Fantasy,
        Biography,
        History,
        Romance,
        Thriller,
        Horror,
        SelfHelp,
        Health,
        Travel,
        Children,
        YoungAdult,
        Poetry,
        Classic
    }
}
