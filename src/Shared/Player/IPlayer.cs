using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.Cards;

namespace OpenDokoBlazor.Shared.Player
{
    public interface IPlayer
    {
    }

    public interface IPlayedCard
    {
        public IPlayer Player { get; }

        public ICard Card { get; }
        
        public int Order { get; }
    }
}
