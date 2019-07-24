using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Protocol.Messages
{
    public class OptionsRequest
    {
        public string Option { get; set; }

        public static readonly string WorldData = "WORLD";
        public static readonly string AssetsComplete = "ASSETSDONE";
    }
}
