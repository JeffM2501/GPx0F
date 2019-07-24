using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LiteNetLib;
using LiteNetLib.Utils;

using Game.Protocol.Messages;

namespace Server
{
    public partial class ServerHost
    {
        protected NetPacketProcessor Processor = new NetPacketProcessor();

        public void RegisterMessageHandlers()
        {
            Processor.SubscribeReusable<AuthRequest, NetPeer>(HandleAuthRequest);
            Processor.SubscribeReusable<OptionsRequest, NetPeer>(HandleOptionsRequest);
        }

        protected virtual void Send<T>(GamePeer peer, T message, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            Processor.Send(peer.Peer, message, method);
        }
    }
}
