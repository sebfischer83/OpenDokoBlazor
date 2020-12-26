using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
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
        public int SortValue
        {
            get
            {
                if ((Value == 2 || Value == 3))
                {
                    return (int)Suit * 10 + Value * 100;
                }
                
                return (int) Suit * 10 + Value;
            }
        }

        public Guid Id { get; }

        protected BaseCard(Suit suit)
        {
            Suit = suit;
            Id = Guid.NewGuid();
        }

        public override int GetHashCode()
        {
            return ((int) Suit) * 100 + Value;
        }

        public override string ToString()
        {
            string suit = Suit switch
            {
                Suit.Diamonds => "♦",
                Suit.Hearts => "♥",
                Suit.Spades => "♠",
                Suit.Clubs => "♣",
                _ => throw new ArgumentOutOfRangeException()
            };

            string value = Value switch
            {
                11 => "A",
                10 => "10",
                4 => "K",
                3 => "Q",
                2 => "J",
                0 => "9",
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return $"{suit}{value}";
        }
    }
}
