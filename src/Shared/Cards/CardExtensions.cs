using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.ViewModels.Card;

namespace OpenDokoBlazor.Shared.Cards
{
    public static class CardExtensions
    {
        public static CardViewModel ToViewModel(this BaseCard card)
        {
            return card switch
            {
                TenCard => new CardViewModel(card.Suit, card.Id, CardViewModelType.Ten),
                NineCard => new CardViewModel(card.Suit, card.Id, CardViewModelType.Nine),
                KingCard => new CardViewModel(card.Suit, card.Id, CardViewModelType.King),
                AceCard => new CardViewModel(card.Suit, card.Id, CardViewModelType.Ace),
                JackCard => new CardViewModel(card.Suit, card.Id, CardViewModelType.Jack),
                QueenCard => new CardViewModel(card.Suit, card.Id, CardViewModelType.Queen),
                _ => throw new ArgumentOutOfRangeException(nameof(card))
            };
        }

        public static CardViewModel ToViewModel(this ICard card)
        {
            return ((BaseCard) card).ToViewModel();
        }
    }
}
