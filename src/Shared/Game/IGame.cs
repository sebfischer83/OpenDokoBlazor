using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenDokoBlazor.Shared.Deck;
using OpenDokoBlazor.Shared.Player;
using OpenDokoBlazor.Shared.Rules;

namespace OpenDokoBlazor.Shared.Game
{
    public interface IGame
    {
        public GameType GameType { get; }
        
        public IRules Rules { get; }
        
        public IDeck Deck { get; }
        
        public IPlayerDeck Player1 { get; }
        public IPlayerDeck Player2 { get; }
        public IPlayerDeck Player3 { get; }
        public IPlayerDeck Player4 { get; }

        public IPlayerDeck GetCardsForPlayer(IPlayer player);
        
        public IList<IPlayer> ReSide { get; }
        public IList<IPlayer> KontraSide { get; }
        
        public int TrickNumber { get; }
        
        public IList<IPlayedCard> CurrentTrick { get; }
    }

    public class Game : IGame
    {
        private ILogger<Game> _logger;
        
        public Game(IRules rules, IPlayer player1, IPlayer player2, IPlayer player3, IPlayer player4, ILoggerFactory loggerFactory)
        {
            Rules = rules;
            _logger = loggerFactory.CreateLogger<Game>();
            Deck = OpenDokoBlazor.Shared.Deck.Deck.Create(rules, loggerFactory);
            //Player1 = player1;
            //Player2 = player2;
            //Player3 = player3;
            //Player4 = player4;
            ReSide = new List<IPlayer>();
            KontraSide = new List<IPlayer>();
            TrickNumber = 0;
            CurrentTrick = new List<IPlayedCard>();
            GameType = GameType.Default;
            
        }

        public GameType GameType { get; private set; }
        public IRules Rules { get; }
        public IDeck Deck { get; }
        public IPlayerDeck Player1 { get; }
        public IPlayerDeck Player2 { get; }
        public IPlayerDeck Player3 { get; }
        public IPlayerDeck Player4 { get; }
        public IPlayerDeck GetCardsForPlayer(IPlayer player)
        {
            throw new NotImplementedException();
        }

        public IList<IPlayer> ReSide { get; }
        public IList<IPlayer> KontraSide { get; }
        public int TrickNumber { get; }
        public IList<IPlayedCard> CurrentTrick { get; }
    }

    public enum GameType
    {
        Default,
        Solo,
        Wedding,
        SilentWedding
    }
}
