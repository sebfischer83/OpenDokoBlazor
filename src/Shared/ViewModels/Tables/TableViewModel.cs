using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.ViewModels.Tables
{
    public class TableViewModel
    {
        public TableViewModel(Guid id, int order, IList<string> playerList)
        {
            Id = id;
            Order = order;
            PlayerList = playerList;
        }

        public Guid Id { get; set; }

        public int Order { get; set; }

        public IList<string> PlayerList { get; set; }
    }
}
