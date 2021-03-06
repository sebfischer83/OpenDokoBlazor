﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.Rules
{
    /// <summary>
    /// The rules for a specific game or table.
    /// The default table rules going to be enriched by the game while playing.
    /// </summary>
    public interface IRules
    {
        public int CardsPerPlayer { get; }

        public bool DulleBeatsDulle { get; set; }
    }

    public class Rules : IRules
    {
        public int CardsPerPlayer
        {
            get
            {
                return 12;
            }
        }

        public bool DulleBeatsDulle { get; set; }

        public Rules()
        {
            DulleBeatsDulle = false;
        }
    }
}
