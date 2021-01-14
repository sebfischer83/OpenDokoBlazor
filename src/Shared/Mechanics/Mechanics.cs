using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using OpenDokoBlazor.Shared.Cards;
using OpenDokoBlazor.Shared.Deck;
using OpenDokoBlazor.Shared.Game;
using OpenDokoBlazor.Shared.Player;
using OpenDokoBlazor.Shared.Rules;

namespace OpenDokoBlazor.Shared.Mechanics
{
    public class Mechanics : IMechanics
    {
        private readonly IGame _game;

        public Mechanics(IGame game)
        {
            _game = game;
        }


        public IPlayedCard DetermineTrickWinner(IOrderedEnumerable<IPlayedCard> cards)
        {
            IPlayedCard FindFirstCardWithHighestSuit(IList<IPlayedCard> cardList)
            {
                var first = cardList.First(card => card.Card.Suit == cardList.Max(c => c.Card.Suit));
                return first;
            }
            
            if (_game.GameType == GameType.Default)
            {
                var cardsList = cards.ToList();
                // is only one trump card? then the winner is clear
                var trumps = cardsList.Where(card => card.Card.IsTrumpCard(_game)).ToList();
                switch (trumps.Count)
                {
                    case 1:
                        return trumps.First();
                    // when no trump card is in trick, then the first highest card wins
                    case 0:
                    {
                        // the first card with the highest value wins
                        var maxValue = cardsList.Max(card => (int) (card.Card.Value + card.Card.Suit));
                        var winningCard = cardsList.First(card => (card.Card.Value + (int) card.Card.Suit) == maxValue);

                        return winningCard;
                    }
                }

                //  more than one trump
                var dulle = trumps.FindAll(card => card.Card.Value == 10 && card.Card.Suit == Suit.Hearts);
                if (dulle.Count > 0)
                {
                    if (dulle.Count == 2 && _game.Rules.DulleBeatsDulle)
                        return dulle[1];
                    return dulle[0];
                }

                // find the highest trump
                var queenTrumps = trumps.FindAll(card => card.Card is QueenCard);
                switch (queenTrumps.Count)
                {
                    case 1:
                        return queenTrumps[0];
                    case > 1:
                        return FindFirstCardWithHighestSuit(queenTrumps);
                }

                var jackTrumps = trumps.FindAll(card => card.Card is JackCard);
                switch (jackTrumps.Count)
                {
                    case 1:
                        return queenTrumps[0];
                    case > 1:
                        return FindFirstCardWithHighestSuit(jackTrumps);
                }

                var maxTrumpValue = trumps.Max(card => (int)(card.Card.Value + card.Card.Suit));
                var winningTrumpCard = trumps.First(card => (card.Card.Value + (int)card.Card.Suit) == maxTrumpValue);

                return winningTrumpCard;
            }

            throw new Exception();
        }

        public bool CanCardBePlaced(ICard card, IPlayerDeck playerDeck)
        {
            var currentTrick = _game.CurrentTrick;
            if (currentTrick.Count == 0)
                return true;
            var playerTrumps = playerDeck.Cards.Where(card1 => card1.IsTrumpCard(_game)).ToList();
            var playerPlains = playerDeck.Cards.Where(card1 => !card1.IsTrumpCard(_game)).ToList();

            // no trump, we need to lay the same color if we have one
            // when not every other card is valid
            var firstTrickCard = currentTrick.First(playedCard => playedCard.Order == 1);
            if (!firstTrickCard.Card.IsTrumpCard(_game))
            {
                // no trump and same suit as the first card
                if (!card.IsTrumpCard(_game) &&
                    card.Suit == firstTrickCard.Card.Suit)
                {
                    return true;
                }

                // no trump, check if the plain card can be placed
                if (!card.IsTrumpCard(_game) &&
                    card.Suit != firstTrickCard.Card.Suit)
                {
                    if (playerPlains.Any(card1 => card1.Suit == firstTrickCard.Card.Suit))
                    {
                        return false;
                    }

                    return true;
                }

                // trump can only placed when we got no other card with this suit
                if (card.IsTrumpCard(_game) && playerPlains.All(card1 => card1.Suit != firstTrickCard.Card.Suit))
                    return true;
                return false;
            }

            if (card.IsTrumpCard(_game))
                return true;
            if (playerTrumps.Count == 0)
                return true;

            return false;
        }

        public IList<ICard> GetAllPlaceableCards(IPlayer player)
        {
            var playerDeck = _game.GetCardsForPlayer(player);
            List<ICard> cards = new List<ICard>();
            cards.AddRange(playerDeck.Cards.Where(c => CanCardBePlaced(c, playerDeck)));

            return cards;
        }
    }
    
    
}