using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;

namespace Client.Game
{
    public class LocalPlayer : Player
    {
        public LocalPlayer(string name, Ships.TeamColors team, Node root, ResourceCache res) : base(name, team, root, res)
        {

        }
    }
}
