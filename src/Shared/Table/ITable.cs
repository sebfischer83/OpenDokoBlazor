using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.Game;
using OpenDokoBlazor.Shared.Player;
using OpenDokoBlazor.Shared.ViewModels.Player;

namespace OpenDokoBlazor.Shared.Table
{
    public interface ITable
    {
        Guid Id { get; }

        bool IsGameRunning { get; }

        IGame? CurrentGame { get; }

        int Order { get; set; }

        bool HasFreeSpace { get; }
        bool IsSoloTable { get; }

        IGame StartNewGame();
        
        void RemovePlayer(Guid playerId);

        IEnumerable<IPlayer> GetPlayers();

        void AddPlayer(Guid playerId, string userName);
    }
}
