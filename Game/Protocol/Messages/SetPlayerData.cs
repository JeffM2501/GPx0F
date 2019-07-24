using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Protocol.Messages
{
    public class SetPlayerData
    {
        public uint PlayerID { get; set; }
        public string Callsign { get; set; }
        public int Score { get; set; }
        public Ships.TeamColors Team { get; set; }
    }
}
