using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Protocol.Messages
{
    public class AuthRequest
    {
        public string Token { get; set; }
        public string Version { get; set; }
    }
}
