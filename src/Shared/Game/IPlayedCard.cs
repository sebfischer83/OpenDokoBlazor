using OpenDokoBlazor.Shared.Cards;
using OpenDokoBlazor.Shared.Player;

namespace OpenDokoBlazor.Shared.Game
{
    public interface IPlayedCard
    {
        public IPlayer Player { get; }

        public ICard Card { get; }
        
        public int Order { get; }
    }

    public class PlayedCard : IPlayedCard
    {
        public IPlayer Player { get; }
        public ICard Card { get; }
        public int Order { get; }

        public PlayedCard(IPlayer player, ICard card, int order)
        {
            Player = player;
            Card = card;
            Order = order;
        }
    }
}