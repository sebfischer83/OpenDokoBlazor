using System;
using System.Reflection.Metadata.Ecma335;
using OpenDokoBlazor.Shared.Cards;

namespace OpenDokoBlazor.Shared.ViewModels.Card
{
    public class CardViewModel
    {
        public CardViewModel(Suit suit, Guid id, CardViewModelType type)
        {
            Suit = suit;
            Id = id;
            Type = type;
        }

        public CardViewModelType Type { get; }

        public Suit Suit { get; }

        public Guid Id { get; }
    }
}