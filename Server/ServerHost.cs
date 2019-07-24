using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game;
using Game.Protocol;
using Game.Protocol.Messages;
using Urho;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Server
{
    public partial class ServerHost : EventBasedNetListener
    {
        protected GameState State = null;
        protected NetManager Server = null;

        public Game.Protocol.Messages.GameInfo GameInfoData = null;
        public byte[] WorldBuffer = new byte[0];

        public class GamePeer : PlayerCore
        {
            public NetPeer Peer = null;

            public string Callsign = string.Empty;
            public int GlobalUserID = int.MinValue;
            public bool VerifiedUser = false;
            public string ValidationToken = string.Empty;

            public enum NegotiationStatuses
            {
                Unverified,
                Validated,
                AssetLoaded,
                Accepted,
            }

            public NegotiationStatuses NegotiationStatus = NegotiationStatuses.Unverified;

            public GamePeer(NetPeer peer)
            {
                Peer = peer;
            }

            public SetPlayerData GetPlayerData()
            {
                SetPlayerData data = new SetPlayerData();
                data.Callsign = Callsign;
                data.Team = this.Team;
                data.PlayerID = ID;
                data.Score = 0;

                return data;
            }
        }

        public List<GamePeer> ConnectedPeers = new List<GamePeer>();

        public void Startup(GameState game)
        {
            SetGameInfo();

            NetworkErrorEvent += ServerHost_NetworkErrorEvent;
            ConnectionRequestEvent += ServerHost_ConnectionRequestEvent;
            PeerConnectedEvent += ServerHost_PeerConnectedEvent;
            PeerDisconnectedEvent += ServerHost_PeerDisconnectedEvent;
            NetworkLatencyUpdateEvent += ServerHost_NetworkLatencyUpdateEvent;
            NetworkReceiveEvent += ServerHost_NetworkReceiveEvent;

            Server = new NetManager(this);

            Server.UnconnectedMessagesEnabled = false;
            RegisterMessageHandlers();

            Server.Start(2501);
        }

        protected void SetGameInfo()
        {
            GameInfoData.MapHash = string.Empty;
            GameInfoData.MapName = "random";
            GameInfoData.MapSize = 500;

            WorldBuffer = State.World.Serialize();
        }

        private void HostingApp_Update(UpdateEventArgs obj)        {
            Server.PollEvents();
        }

        protected GamePeer GetGamePeer(NetPeer peer)
        {
            return ConnectedPeers.Find((x) => x.Peer == peer);
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
            if (request.Data.GetString(8) != Game.Protocol.Constants.HeaderProto)
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
