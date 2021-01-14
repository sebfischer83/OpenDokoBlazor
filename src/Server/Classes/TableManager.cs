using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        public void Init()
        {
            for (int i = 0; i < _openDokoOptions.Value.NumberOfTables; i++)
            {
                var table = new Table(_loggerFactory, i + 1);
                _tables.Add(table.Id, table);
            }
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
