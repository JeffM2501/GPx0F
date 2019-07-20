using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Protocol.Messages
{
    public class AuthResponce
    {
        public bool Ok { get; set; }
        public string Result { get; set; }
        public int ID { get; set; }
    }
}
