using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using OpenDokoBlazor.Shared.ViewModels.Card;

namespace OpenDokoBlazor.Client.Components.Card
{
    public partial class PlayerCardDeck
    {
        [Parameter]
        public DeckViewModel Model { get; set; }

        [Parameter]
        public List<Guid> Placeable { get; set; }

        private bool IsCardPlaceable(CardViewModel model)
        {
            if (Placeable == null || Placeable.Count == 0)
            {
                return false;
            }

            return Placeable.Contains(model.Id);
        }
    }
}
