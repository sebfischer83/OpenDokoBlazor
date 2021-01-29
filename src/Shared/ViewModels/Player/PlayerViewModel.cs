using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.ViewModels.Player
{
    public class PlayerViewModel
    {
        public PlayerViewModel(Guid id, int order, string name)
        {
            Id = id;
            Order = order;
            Name = name;
        }

        public Guid Id { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }
    }
}
