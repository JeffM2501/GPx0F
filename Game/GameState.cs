using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Maps;

using Urho;
using Urho.Physics;
using Urho.Resources;

namespace Game
{
    public class GameState
    {
        public Arena World = null;
        public Scene RootScene = null;

        public List<PlayerCore> Players = new List<PlayerCore>();

        public delegate void PlayerEventHandler(PlayerCore player);

        public event PlayerEventHandler PlayerAdded;
        public event PlayerEventHandler PlayerRemoved;

        public GameState()
        {
            RootScene = new Scene();
            RootScene.CreateComponent<Octree>();
            RootScene.CreateComponent<PhysicsWorld>();
        }

        public void ClearWorld()
        {
            if (RootScene != null)
            {
                foreach (var child in RootScene.Children.ToArray())
                    child.Remove();
            }
        }

        public void AddPlayer(PlayerCore player)
        {
            Players.Add(player);
            PlayerAdded?.Invoke(player);
        }

        public void RemovePlayer(PlayerCore player)
        {
            Players.Remove(player);
            PlayerRemoved?.Invoke(player);
        }
    }
}
