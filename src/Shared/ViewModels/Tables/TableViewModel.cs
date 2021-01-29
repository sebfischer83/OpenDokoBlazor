using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.ViewModels.Player;

namespace OpenDokoBlazor.Shared.ViewModels.Tables
{
    public class TableViewModel
    {
        public TableViewModel(Guid id, int order, IList<PlayerViewModel> playerList, bool soloTable)
        {
            Id = id;
            Order = order;
            PlayerList = playerList;
            SoloTable = soloTable;
        }

        public Guid Id { get; set; }

        public int Order { get; set; }

        public IList<PlayerViewModel> PlayerList { get; set; }
        public bool SoloTable { get; }
    }
}
