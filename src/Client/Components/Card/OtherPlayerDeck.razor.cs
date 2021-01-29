using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace OpenDokoBlazor.Client.Components.Card
{
    public partial class OtherPlayerDeck
    {
        [Parameter] public OtherPlayerPosition PlayerPosition { get; set; }
    }
}
