using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using OpenDokoBlazor.Shared.Game;
using OpenDokoBlazor.Shared.Services;
using OpenDokoBlazor.Shared.ViewModels.Card;
using Stl.Fusion;
using Stl.Fusion.Authentication;

namespace OpenDokoBlazor.Client.Pages.Table
{
    [Authorize]
    public partial class TablePage
    {
        [Parameter]
        public string? TableId { get; set; }

        [Inject]
        public IGameService GameService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ILogger<TablePage> Logger { get; set; }

        public class Model
        {
            public Model(DeckViewModel playerDeck, List<Guid> placeableCards, bool isGameStarted)
            {
                PlayerDeck = playerDeck;
                PlaceableCards = placeableCards;
                IsGameStarted = isGameStarted;
            }

            public bool IsGameStarted { get; set; }
            public DeckViewModel PlayerDeck { get; set; }
            public List<Guid> PlaceableCards { get; set; }
        }

        protected override void OnInitialized()
        {
            StateHasChangedTriggers = StateEventKind.All;
            base.OnInitialized();
        }

        public override void Dispose()
        {
            base.Dispose();
            Logger.LogInformation($"Table {TableId} closed");
            if (!string.IsNullOrWhiteSpace(TableId))
                GameService.RemovePlayerFromTable(new Guid(TableId));
        }

        protected override async Task<Model> ComputeStateAsync(CancellationToken cancellationToken)
        {
            var started = await GameService.GetGameStartedState(new Guid(TableId!), cancellationToken);
            if (!started)
            {
                return new Model(new DeckViewModel(), new List<Guid>(), false);
            }
            var deck = await GameService.GetUserDeck(new Guid(TableId!),cancellationToken);
            var placableCards = await GameService.GetPlacableCards(new Guid(TableId!), cancellationToken);

            return new Model(deck, placableCards, true);
        }
    }
}
