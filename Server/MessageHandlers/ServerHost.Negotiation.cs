
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LiteNetLib;

using Game.Protocol.Messages;

namespace Server
{
    public partial class ServerHost
    {
        protected void HandleAuthRequest(AuthRequest request, NetPeer peer)
        {
            var player = GetGamePeer(peer);
            if (player == null || player.NegotiationStatus != GamePeer.NegotiationStatuses.Unverified)
                return;

            // fire off a task to check the token
            Services.Link.ValidateToken(request.Token, player, AuthComplete);
        }

        protected void AuthComplete(bool good, string result, ServerHost.GamePeer peer)
        {
            AuthResponce responce = new AuthResponce();
            if (!good)
            {
                responce.Ok = false;
                responce.Result = result;
            }
            else
            {
                responce.Ok = true;
                responce.Result = peer.Callsign;
                responce.ID = peer.GlobalUserID;
            }

            Send(peer, responce);

            if(!responce.Ok)
            {
                Server.Flush();
                peer.Peer.Disconnect();
            }
            else
            {
                Send(peer, GameInfoData);
            }
        }

        public void HandleOptionsRequest(OptionsRequest request, NetPeer peer)
        {
            var player = GetGamePeer(peer);
            if (player == null || player.NegotiationStatus != GamePeer.NegotiationStatuses.Unverified)
                return;

            OptionsResponce responce = new OptionsResponce();
            responce.Option = request.Option;

            if (request.Option == OptionsRequest.WorldData)
                responce.Result = WorldBuffer;
            else if (request.Option == OptionsRequest.AssetsComplete)
            {
                player.NegotiationStatus = GamePeer.NegotiationStatuses.AssetLoaded;
                SendInitalPlayerStates(player);
            }

            if (responce.Result.Length > 0)
                Send(player, responce);
        }

        public void SendInitalPlayerStates(GamePeer peer)
        {
            foreach (GamePeer player in State.Players)
                Send(peer, peer.GetPlayerData(),DeliveryMethod.ReliableOrdered);

          //  foreach(var)
        }
    }
}
