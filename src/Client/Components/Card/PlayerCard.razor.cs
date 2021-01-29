using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using OpenDokoBlazor.Shared.ViewModels.Card;
#pragma warning disable 8618

namespace OpenDokoBlazor.Client.Components.Card
{
    public partial class PlayerCard
    {
        [Parameter]
        public CardViewModel CardViewModel { get; set; }

        [Parameter]
        public bool Placeable { get; set; }

        [Inject]
        public ICardStyle Style { get; set; }
    }
}
