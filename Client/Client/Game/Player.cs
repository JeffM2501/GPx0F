using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;

namespace Client.Game
{
    public class Player : Component
    {
        public string Name = string.Empty;
        public Ships.TeamColors Team = Ships.TeamColors.Blue;
        public Ships.ShipNode Ship = null;


        public Player(string name, Ships.TeamColors team, Node root, ResourceCache res)
        {
            Name = name;
            Ship = Ships.GetShipNode(res, root, team, "Mk3");
            Ship.Node.AddComponent(this);
            Team = team;
        }
    }
}
