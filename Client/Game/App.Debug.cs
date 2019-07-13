using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Physics;

namespace Client.Game
{
    public partial class App
    {
        public bool DrawDebug = false;
        protected void SetupDebug()
        {
            Engine.PostRenderUpdate += Engine_PostRenderUpdate;
        }

        private void Engine_PostRenderUpdate(PostRenderUpdateEventArgs obj)
        {
            if (!DrawDebug || Exiting)
                return;

            if (World != null && World.GetComponent<PhysicsWorld>() != null)
            {
                World.GetComponent<PhysicsWorld>().DrawDebugGeometry(false);
            }
        }
    }
}
