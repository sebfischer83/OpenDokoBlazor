using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenDokoBlazor.Shared.Deck;
using OpenDokoBlazor.Shared.Mechanics;
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

        private IMechanics _mechanics;
        private readonly Dictionary<int, IPlayerDeck> _playerDecks;

        public Game(IRules rules, IPlayer player1, IPlayer player2, IPlayer player3, IPlayer player4, ILoggerFactory loggerFactory)
        {
            Rules = rules;
            _logger = loggerFactory.CreateLogger<Game>();
            Deck = OpenDokoBlazor.Shared.Deck.Deck.Create(rules, loggerFactory);

            ReSide = new List<IPlayer>();
            KontraSide = new List<IPlayer>();
            TrickNumber = 0;
            CurrentTrick = new List<IPlayedCard>();
            GameType = GameType.Default;
            _mechanics = new Mechanics.Mechanics(this);
            var decks = Deck.GetDeckForEachPlayer();

            _playerDecks = new Dictionary<int, IPlayerDeck> { { 1, new PlayerDeck(player1, decks.deck1) }, { 2, new PlayerDeck(player2, decks.deck2) },
                { 3,  new PlayerDeck(player3, decks.deck3)  }, { 4,  new PlayerDeck(player4, decks.deck4)  } };
        }

        public GameType GameType { get; private set; }
        public IRules Rules { get; }
        public IDeck Deck { get; }

        public IPlayerDeck Player1 => _playerDecks[1];

        public IPlayerDeck Player2 => _playerDecks[2];
        public IPlayerDeck Player3 => _playerDecks[3];
        public IPlayerDeck Player4 => _playerDecks[4];
        public IPlayerDeck GetCardsForPlayer(IPlayer player)
        {
            return _playerDecks.FirstOrDefault(pair => pair.Value.Player.Id == player.Id).Value;
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
