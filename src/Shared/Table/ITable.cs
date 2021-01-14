using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenDokoBlazor.Shared.Game;
using OpenDokoBlazor.Shared.Player;

namespace OpenDokoBlazor.Shared.Table
{
    public interface ITable
    {
        Guid Id { get; }

        IGame? CurrentGame { get; }

        int Order { get; set; }

        bool HasFreeSpace { get; }

        IGame StartNewGame();

        void AddPlayer(IPlayer player);

        void RemovePlayer(IPlayer player);

        IEnumerable<IPlayer> GetPlayers();
    }

    public class Table : ITable
    {
        private readonly ILoggerFactory _loggerFactory;

        private List<IPlayer> _players;

        public Guid Id { get; }

        public int Order { get; set; }
        public IGame? CurrentGame { get; }
        public bool HasFreeSpace => GetPlayers().Count() < 4;
        public Table(ILoggerFactory loggerFactory, int order)
        {
            _loggerFactory = loggerFactory;
            Order = order;
            Id = Guid.NewGuid();
            _players = new List<IPlayer>();
        }
        public IGame StartNewGame()
        {
            throw new NotImplementedException();
        }

        public void AddPlayer(IPlayer player)
        {
            if (_players.Count == 4)
                throw new Exception("Can't add more than 4 players to a table");

            _players.Add(player);
        }

        public void RemovePlayer(IPlayer player)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPlayer> GetPlayers()
        {
            return _players;
        }
    }
}
