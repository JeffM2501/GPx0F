using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;

namespace Server
{
    public partial class App : Application
    {
        private ServerHost Host = null;
        private Game.GameState State = null;

        public App(ApplicationOptions options = null) : base(options) { }

        protected override void Start()
        {
            base.Start();
            Host = new ServerHost();
            State = new Game.GameState();

            Update += App_Update;
            Host.Startup(State);
        }

        private void App_Update(UpdateEventArgs obj)
        {
            Host.Update(obj);
        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);
        }
    }
}
