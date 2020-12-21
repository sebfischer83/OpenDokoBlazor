using System;
using System.Linq;
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
            if (_game.GameType == GameType.Default)
            {
                var cardsList = cards.ToList();
                // is only one trump card? then the winner is clear
                var trumps = cardsList.Where(card => card.Card.IsTrumpCard(_game.Rules)).ToList();
                if (trumps.Count == 1)
                {
                    return trumps.First();
                }

                // when no trump card is in trick, then the first highest card wins
                if (trumps.Count == 0)
                {
                    // the first card with the highest value wins
                    var maxValue = cardsList.Max(card => card.Card.Value);
                    var winningCard = cardsList.First(card => card.Card.Value == maxValue);

                    return winningCard;
                }

                //  more than one trump
                
            }

            throw new Exception();
        }
    }
    
    
}