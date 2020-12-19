using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.Cards
{
    public enum CardVisualAppearance
    {
        French,
        German
    }

    public abstract record BaseCard : ICard
    {
        public Suit Suit { get; init; }

        public abstract int Value { get; }

        protected BaseCard(Suit suit)
        {
            Suit = suit;
        }

        public override int GetHashCode()
        {
            return ((int) Suit) * 100 + Value;
        }
    }
}
