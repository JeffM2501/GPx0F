using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Maps;


using Urho.Resources;

namespace Game
{
    public class GameState
    {
        public Arena World = null;

        public List<PlayerCore> Players = new List<PlayerCore>();

        public delegate void PlayerEventHandler(PlayerCore player);

        public event PlayerEventHandler PlayerAdded;
        public event PlayerEventHandler PlayerRemoved;

        public GameState()
        {

        }

        public void StartSimpleWorld()
        {

        }

        public void AddPlayer(PlayerCore player)
        {
            Players.Add(player);
            PlayerAdded?.Invoke(player);
        }
    }
}
