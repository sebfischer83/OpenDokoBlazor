using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.ViewModels.Card
{
    public class DeckViewModel
    {
        public List<CardViewModel> Cards { get; set; } = new List<CardViewModel>();
    }
}
