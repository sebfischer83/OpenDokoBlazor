using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.ViewModels.Card;
using OpenDokoBlazor.Shared.ViewModels.Chat;
using OpenDokoBlazor.Shared.ViewModels.Tables;
using Stl.Fusion;
using Stl.Fusion.Authentication;

namespace OpenDokoBlazor.Shared.Services
{
    public interface IGameService
    {
        Task AddPlayerToTable(Guid tableId, CancellationToken cancellationToken = default);

        Task RemovePlayerFromTable(Guid tableId, CancellationToken cancellationToken = default);

        [ComputeMethod(KeepAliveTime = 1)]
        Task<TableViewModel[]> GetTablesAsync(CancellationToken cancellationToken = default);

        [ComputeMethod(KeepAliveTime = 1)]
        Task<DeckViewModel> GetUserDeck(Guid tableId, CancellationToken cancellationToken = default);

        [ComputeMethod(KeepAliveTime = 1)]
        Task<List<Guid>> GetPlacableCards(Guid tableId, CancellationToken cancellationToken = default);

        [ComputeMethod(KeepAliveTime = 1)]
        Task<bool> GetGameStartedState(Guid tableId, CancellationToken cancellationToken = default);
    }
}
