using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Network;

namespace Game
{
    public static class NetworkEvents
    {
        public static readonly StringHash PlayerSpawned = new StringHash("PlayerSpawned");
    }
}
