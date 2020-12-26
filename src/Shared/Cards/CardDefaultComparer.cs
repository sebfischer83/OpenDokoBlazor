using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.Mechanics;

namespace OpenDokoBlazor.Shared.Cards
{
    /// <summary>
    /// Comparer for a default rule set to sort the cards in for the user interface.
    /// </summary>
    public class CardDefaultComparer : IComparer<ICard>
    {
        public int Compare(ICard? x, ICard? y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (ReferenceEquals(x, null))
                return 1;
            if (ReferenceEquals(y, null))
                return -1;

            // both no trump
            if (!x.IsTrumpCard(null) && !y.IsTrumpCard(null))
            {
                return x.SortValue.CompareTo(y.SortValue);
            }
            // one side is trump
            if (x.IsTrumpCard(null) && !y.IsTrumpCard(null))
            {
                return 1;
            }
            if (!x.IsTrumpCard(null) && y.IsTrumpCard(null))
            {
                return -1;
            }
            // both trump
            // can be sorted normally except the "Dullen"
            if (x.Suit == Suit.Hearts && y.Suit == Suit.Hearts && x.Value == 10 && y.Value == 10)
            {
                return 0;
            }
            if (x.Suit == Suit.Hearts && x.Value == 10)
            {
                return 1;
            }
            if (y.Suit == Suit.Hearts && y.Value == 10)
            {
                return -1;
            }
            
            if (x.Value == y.Value)
            {
                return x.Suit.CompareTo(y.Suit);
            }
            
            return x.SortValue.CompareTo(y.SortValue);
        }
    }
}
