using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenDokoBlazor.Shared.Game;
using OpenDokoBlazor.Shared.Player;

namespace OpenDokoBlazor.Shared.Table
{
    public class Table : ITable
    {
        private readonly ILoggerFactory _loggerFactory;

        private readonly List<IPlayer> _players;

        public Guid Id { get; }
        public bool IsGameRunning { get; private set; }
        public bool IsSoloTable { get; private set; }

        public int Order { get; set; }
        public IGame? CurrentGame { get; private set; }
        public bool HasFreeSpace => GetPlayers().Count() < 4;
        public Table(ILoggerFactory loggerFactory, int order, bool soloTable = false)
        {
            _loggerFactory = loggerFactory;
            Order = order;
            Id = Guid.NewGuid();
            _players = new List<IPlayer>();
            IsGameRunning = false;
            IsSoloTable = soloTable;
        }
        public IGame StartNewGame()
        {
            if (IsSoloTable)
            {
                CurrentGame = new Game.Game(new Rules.Rules(),
                    _players[0], 
                    new DummyPlayer(Guid.NewGuid(), "Bot 1", 2),
                    new DummyPlayer(Guid.NewGuid(), "Bot 2", 3),
                    new DummyPlayer(Guid.NewGuid(), "Bot 3", 4), 
                        _loggerFactory);
            }
            else
                CurrentGame = new Game.Game(new Rules.Rules(), _players[0], _players[1], _players[2],
                    _players[3], _loggerFactory);
            IsGameRunning = true;
            return CurrentGame;
        }
        
        public void AddPlayer(Guid playerId, string userName)
        {
            if (_players.Count == 4)
                throw new Exception("Can't add more than 4 players to a table");
            if (_players.Any(player1 => player1.Id == playerId))
                throw new Exception("Can't add player more than once to a table");

            int firstAvailable = Enumerable.Range(0, 3)
                .Except(_players.Select(p => p.Order))
                .First();
            var player = new Player.Player(playerId, userName, firstAvailable);
            _players.Add(player);
            if (IsSoloTable || _players.Count == 4)
            {
                StartNewGame();
            }
        }

        public void RemovePlayer(Guid playerId)
        {
            var player = _players.Find(player => player.Id == playerId);
            if (player != null)
                _players.Remove(player);
        }

        public IEnumerable<IPlayer> GetPlayers()
        {
            return _players;
        }
    }
}