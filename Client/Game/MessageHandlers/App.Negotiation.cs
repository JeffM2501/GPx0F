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
        public event EventHandler WorldLoaded = null;

        protected void HandleAuthResponce(AuthResponce request, NetPeer peer)
        {
            if (request.Ok)
                SetJoining();
            else
            {
                // handle error state
            }
        }

        protected void HandleGameInfo(GameInfo info, NetPeer peer)
        {
            OptionsRequest done = new OptionsRequest();
            done.Option = OptionsRequest.WorldData;
            Send(done);
        }

        protected void HandleOptionsResponce(OptionsResponce info, NetPeer peer)
        {
            bool isDone = false;

            if (info.Option == OptionsRequest.WorldData)
            {
                State.World.Deserialize(info.Result);
                WorldLoaded?.Invoke(this, EventArgs.Empty);
                isDone = true;
            }


            if (isDone)
            {
                OptionsRequest done = new OptionsRequest();
                done.Option = OptionsRequest.AssetsComplete;
                Send(done);
            }
        }
    }
}
