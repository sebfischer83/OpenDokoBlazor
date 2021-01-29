using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using OpenDokoBlazor.Client.Services;
using OpenDokoBlazor.Shared.Services;
using OpenDokoBlazor.Shared.ViewModels.Tables;
using Stl.Fusion;

#pragma warning disable 8618
namespace OpenDokoBlazor.Client.Components.Table
{
    [Authorize]
    public partial class TableContainer
    {
        [Inject]
        public IGameService GameService { get; set; }
        [Inject]
        public ClientState ClientState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ILogger<TableContainer> Logger { get; set; }

        public class Model
        {
            public IList<TableViewModel> Tables { get; set; } = new List<TableViewModel>();
        }

        private async Task UserGoToTable(Guid tableId)
        {
            Logger.LogInformation($"user go to table {tableId}");
            await GameService.AddPlayerToTable(tableId);
            NavigationManager.NavigateTo($"/table/{tableId}");
        }

        protected override void OnInitialized()
        {
            StateHasChangedTriggers = StateEventKind.All;
            base.OnInitialized();
        }

        protected override async Task<Model> ComputeStateAsync(CancellationToken cancellationToken)
        {
            var tables = await GameService.GetTablesAsync(cancellationToken);
            if (tables.Any())
            {
                return new Model() { Tables = tables.ToList() };
            }

            return new Model() { Tables = new List<TableViewModel>() };
        }
    }
}
