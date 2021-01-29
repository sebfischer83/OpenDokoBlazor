using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Server
{
    public class OpenDokoOptions
    {
        public const string Identifier = "OpenDoko";

        public int NumberOfTables { get; set; }
        public int NumberOfSoloTables { get; set; }
    }
}
