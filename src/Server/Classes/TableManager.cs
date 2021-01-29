using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenDokoBlazor.Shared.Game;
using OpenDokoBlazor.Shared.Table;

namespace OpenDokoBlazor.Server.Classes
{
    public class TableManager : IEnumerable<ITable>
    { 
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<OpenDokoOptions> _openDokoOptions;
        private readonly Dictionary<Guid, ITable> _tables;

        public TableManager(ILoggerFactory loggerFactory, IOptions<OpenDokoOptions> openDokoOptions)
        {
            _loggerFactory = loggerFactory;
            _openDokoOptions = openDokoOptions;
            _tables = new();
        }
        
        public ITable this[Guid id] => _tables[id];

        public ITable this[int id] => _tables.First(pair => pair.Value.Order == id).Value;

        public bool IsPlayerAlreadyInGame(Guid playerId)
        {
            return _tables.Any(pair => pair.Value.GetPlayers().Any(player => player.Id == playerId));
        }

        public void Init()
        {
            for (int i = 0; i < _openDokoOptions.Value.NumberOfTables; i++)
            {
                var table = new Table(_loggerFactory, i + 1);
                _tables.Add(table.Id, table);
            }
            for (int i = 0; i < _openDokoOptions.Value.NumberOfSoloTables; i++)
            {
                var table = new Table(_loggerFactory, i + 1, true);
                _tables.Add(table.Id, table);
            }
        }

        public IGame GetGameForPlayer(Guid playerId)
        {
            var table = _tables.Select(pair => pair.Value).FirstOrDefault(pair => pair.GetPlayers().Any(player => player.Id == playerId));
            return table?.CurrentGame;
        }

        public IEnumerator<ITable> GetEnumerator()
        {
            return _tables.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
