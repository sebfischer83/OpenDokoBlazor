using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.Rules
{
    /// <summary>
    /// The rules for a specific game or table
    /// </summary>
    public interface IRules
    {
        public int CardsPerPlayer { get; }
    }
}
