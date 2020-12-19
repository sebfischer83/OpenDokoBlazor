using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.Cards;
using OpenDokoBlazor.Shared.Player;
using OpenDokoBlazor.Shared.Rules;

namespace OpenDokoBlazor.Shared.Mechanics
{
    public interface IMechanics
    {
        IPlayedCard DetermineTrickWinner(IOrderedEnumerable<IPlayedCard> cards);
    }

    public class Mechanics : IMechanics
    {
        private readonly IRules _rules;

        public Mechanics(IRules rules)
        {
            _rules = rules;
        }


        public IPlayedCard DetermineTrickWinner(IOrderedEnumerable<IPlayedCard> cards)
        {


            throw new Exception();
        }
    }

    public static class MechanicExtensions
    {
        public static bool IsTrumpCard(this ICard card, IRules rules)
        {
            #region default rules
            // any diamond card is a trump
            if (card.Suit == Suit.Diamonds)
                return true;
            // these are always trumps
            if (card is JackCard || card is KingCard || card is QueenCard)
                return true;
            // ten heart is a trump
            if (card is TenCard && card.Suit == Suit.Hearts)
                return true;

            #endregion


            return false;
        }
    }
}
