using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;

namespace Server.Services
{
    public static class Link
    {
        public delegate void PlayerLinkTaskCallback(bool good, string result, ServerHost.GamePeer peer);

        private static string ServicesURL = "https://www.voidperilous.com/API/api/";

        public static void ValidateToken(string token, ServerHost.GamePeer peer, PlayerLinkTaskCallback callback)
        {
            Random rng = new Random();
            peer.Callsign = "Player" + rng.Next(99, 9999).ToString();
            peer.GlobalUserID = rng.Next(1, int.MaxValue);
            peer.ValidationToken = rng.Next().ToString() + rng.Next().ToString();

            callback?.Invoke(true, peer.ValidationToken, peer);
            return;

            WebClient validateClient = new WebClient();
            byte[] data = new byte[0];
            validateClient.UploadDataAsync(new Uri(ServicesURL), "POST", data, new Tuple<ServerHost.GamePeer, PlayerLinkTaskCallback>(peer,callback));
            validateClient.UploadDataCompleted += ValidateClient_UploadDataCompleted;
        }

        private static void ValidateClient_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            ServerHost.GamePeer peer = (e.UserState as Tuple<ServerHost.GamePeer, PlayerLinkTaskCallback>).Item1;
            PlayerLinkTaskCallback cb = (e.UserState as Tuple<ServerHost.GamePeer, PlayerLinkTaskCallback>).Item2;

            if (e.Cancelled || e.Error == null)
            {
                cb?.Invoke(false, "failed", peer);
            }
            else
            {
                // read the data and set the user
            }
        }
    }
}
