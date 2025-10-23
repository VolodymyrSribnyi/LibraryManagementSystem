using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    /// <summary>
    /// Represents a rating system with values ranging from no rating to five stars.
    /// </summary>
    /// <remarks>The <see cref="Rating"/> enumeration is commonly used to represent user feedback or
    /// evaluations in a standardized format. The values range from <see cref="NotRated"/> (indicating no rating) to
    /// <see cref="FiveStars"/> (indicating the highest rating).</remarks>
    public enum Rating
    {
        NotRated = 0,
        OneStar,
        TwoStars,
        ThreeStars,
        FourStars,
        FiveStars
    }
}
