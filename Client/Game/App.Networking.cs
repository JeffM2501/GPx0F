using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using LiteNetLib;

namespace Client.Game
{
    public partial class App
    {
        NetManager NetCient = null;

        public void Connect(string name, int port)
        {
            NetCient = new NetManager(this);
        }

        public void OnPeerConnected(NetPeer peer)
        {

        }
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {

        }
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {

        }
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {

        }
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {

        }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }
        public void OnConnectionRequest(ConnectionRequest request)
        {
        }
    }
}
