using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.Player
{
    public interface IPlayer
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class Player : IPlayer
    {
        public Player(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
