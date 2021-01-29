using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.ViewModels.Player;

namespace OpenDokoBlazor.Shared.Player
{
    public interface IPlayer
    {
        public Guid Id { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }

        PlayerViewModel ToViewModel();
    }

    public class DummyPlayer : Player
    {
        public DummyPlayer(Guid id, string name, int order) : base(id, name, order)
        {
        }
    }

    public class Player : IPlayer
    {
        public Player(Guid id, string name, int order)
        {
            Id = id;
            Name = name;
            Order = order;
        }

        public static implicit operator PlayerViewModel(Player p)
        {
            return new(p.Id, p.Order, p.Name);
        }

        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public PlayerViewModel ToViewModel()
        {
            return this;
        }
    }
}
