using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Protocol.Messages
{
    public class OptionsResponce
    {
        public string Option { get; set; }
        public byte[] Result { get; set; }
    }
}
