using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;
using Urho.Actions;

using Urho.Audio;
using Urho.Physics;

namespace Client.Game
{
    public partial class App : Application
    {
        protected Scene World = null;
        protected Camera MainCamera = null;

        protected Arena CurrentArena = null;

        public App(ApplicationOptions options = null) : base(options) { }

        protected override void Start()
        {
            base.Start();

            if (Config.Current != null)
                Config.Current.ApplyChanges += Config_ApplyChanges;

            Input.SetMouseMode(MouseMode.Absolute);
            Input.SetMouseVisible(true);

            Input.ExitRequested += Input_ExitRequested;

            Menus.Stack.Setup(this);

            SetupScene();
            SetupMainMenu();

            Update += Game_Update;

            if (Config.Current != null && Config.Current.WinType == Config.WindowTypes.Window && Program.RectIsVisible(Config.Current.WindowBounds))
                this.Graphics.SetWindowPosition(new IntVector2(Config.Current.WindowBounds.X, Config.Current.WindowBounds.Y));

            Graphics.WindowTitle = ClientResources.WindowTitle;

            Engine.PostRenderUpdate += Engine_PostRenderUpdate;
        }

        private void Engine_PostRenderUpdate(PostRenderUpdateEventArgs obj)
        {
            if (World != null && World.GetComponent<PhysicsWorld>() != null)
            {
                World.GetComponent<PhysicsWorld>().DrawDebugGeometry(false);
            }
        }

        protected override void Stop()
        {
            if (Config.Current != null)
                Config.Current.Save();

             base.Stop();
        }

        protected override void OnUpdate(float timeStep)
        {
            if (!Exiting)
                base.OnUpdate(timeStep);

          
        }

        protected void SetMainViewport()
        {
            Renderer.SetViewport(0, new Viewport(Context, World, MainCamera, null));
        }

        public void SetupScene()
        {
            World = new Scene();
            World.CreateComponent<Octree>();
            World.CreateComponent<DebugRenderer>();
        }
    }
}
