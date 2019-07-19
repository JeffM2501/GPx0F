using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game;
using Urho;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Server
{
    public class ServerHost : EventBasedNetListener
    {
        protected GameState Game = null;
        protected NetManager Server = null;

        protected NetPacketProcessor Processor = new NetPacketProcessor();


        public class GamePeer
        {
            public NetPeer Peer = null;
            public PlayerCore Player = null;

            public enum NegotiationStatuses
            {
                Unverified,
                Verified,
                AssetLoading,
                Accepted,
            }

            public NegotiationStatuses NegotiationStatus = NegotiationStatuses.Unverified;

            public GamePeer(NetPeer peer)
            {
                Peer = peer;
            }
        }

        public List<GamePeer> ConnectedPeers = new List<GamePeer>();

        public void Startup(GameState game, Application hostingApp)
        {
            NetworkErrorEvent += ServerHost_NetworkErrorEvent;
            ConnectionRequestEvent += ServerHost_ConnectionRequestEvent;
            PeerConnectedEvent += ServerHost_PeerConnectedEvent;
            PeerDisconnectedEvent += ServerHost_PeerDisconnectedEvent;
            NetworkLatencyUpdateEvent += ServerHost_NetworkLatencyUpdateEvent;
            NetworkReceiveEvent += ServerHost_NetworkReceiveEvent;

            Server = new NetManager(this);

            Server.UnconnectedMessagesEnabled = false;

            Processor.SubscribeReusable<Game.Messages.AuthRequest, NetPeer>(HandleAuthRequest);

            Server.Start(2501);
        }


        protected void HandleAuthRequest(Game.Messages.AuthRequest request, NetPeer peer)
        {

        }

        protected GamePeer GetGamePeer(NetPeer peer)
        {
            return ConnectedPeers.Find((x) => x.Peer == peer);
        }

        private void ServerHost_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            GamePeer p = GetGamePeer(peer);
            if (p == null)
                return;
        }

        private void ServerHost_NetworkLatencyUpdateEvent(NetPeer peer, int latency)
        {
        }

        private void ServerHost_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {

        }

        private void ServerHost_PeerConnectedEvent(NetPeer peer)
        {
            GamePeer p = new GamePeer(peer);
            ConnectedPeers.Add(p);
            StartPeerNegotiation(p);
        }

        private void ServerHost_ConnectionRequestEvent(ConnectionRequest request)
        {
            if (request.Data.GetString(8) != "GPx0F_001")
                return;

            // check ban list
            // request.RemoteEndPoint

            request.Accept();
        }

        private void ServerHost_NetworkErrorEvent(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
        {
            // log it
        }


        protected virtual void StartPeerNegotiation(GamePeer peer)
        {
            // ask them for the shit!
        }

    }
}
