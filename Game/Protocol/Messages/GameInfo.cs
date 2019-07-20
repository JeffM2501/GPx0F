using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Protocol.Messages
{
    public class GameInfo
    {
        public string MapName { get; set; }
        public string MapHash { get; set; }
        public int MapSize { get; set; }

        public string[] Assets { get; set; }
    }
}
