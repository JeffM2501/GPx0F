using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Protocol.Messages;

using LiteNetLib;

namespace Client.Game
{
    public partial class App
    {
        protected void HandleAuthResponce(AuthResponce request, NetPeer peer)
        {
            if (request.Ok)
                SetJoining();
            else
            {
                // handle error state
            }
        }
    }
}
