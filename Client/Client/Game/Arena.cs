using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;

namespace Client.Game
{
    public class Arena
    {
        public virtual bool Setup(ResourceCache resources, Scene world, float size)
        {
            return true; // does nothing, that always works
        }
    }
}
