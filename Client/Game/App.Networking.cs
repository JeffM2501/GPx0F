using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Game.Protocol.Messages;
using LiteNetLib;
using LiteNetLib.Utils;

using Game.Protocol;
using Game.Protocol.Messages;

namespace Client.Game
{
    public partial class App
    {
        NetManager NetCient = null;
        NetPacketProcessor Processor = new NetPacketProcessor();
        EventBasedNetListener Listener = null;

        protected NetPeer ServerPeer = null;

        protected string NetServerName = string.Empty;


        public void Connect(string name, int port)
        {
            Listener = new EventBasedNetListener();
            Listener.PeerConnectedEvent += OnPeerConnected;
            Listener.PeerDisconnectedEvent += OnPeerDisconnected;
            Listener.NetworkReceiveEvent += OnNetworkReceive;

            NetCient = new NetManager(Listener);
            var data = new NetDataWriter();
            NetCient.Start();
            NetCient.Connect(name, port, Constants.HeaderProto);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            ServerPeer = peer;

            AuthRequest request = new AuthRequest();
            request.Token = "Random Token";
            request.Version = Config.GetVersionString();
            Send(request);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            ServerPeer = null;
            NetCient = null;
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Processor.ReadAllPackets(reader, peer);
        }

        private void PollMessages()
        {
            NetCient?.PollEvents();
        }

        public void RegisterMessageHandlers()
        {
            Processor.SubscribeReusable<AuthResponce, NetPeer>(HandleAuthResponce);
            Processor.SubscribeReusable<GameInfo, NetPeer>(HandleGameInfo);
        }

        protected virtual void Send<T>(T message, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            if (ServerPeer == null)
                return;

            Processor.Send(ServerPeer, message, method);
        }
    }
}
