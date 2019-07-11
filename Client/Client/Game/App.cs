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

            SetupRenderer();

            Menus.Stack.Setup(this);
            World = new Scene();
            SetupScene();
            SetupMainMenu();

            Update += Game_Update;

            if (Config.Current != null && Config.Current.WinType == Config.WindowTypes.Window && Program.RectIsVisible(Config.Current.WindowBounds))
                this.Graphics.SetWindowPosition(new IntVector2(Config.Current.WindowBounds.X, Config.Current.WindowBounds.Y));

            Graphics.WindowTitle = ClientResources.WindowTitle;
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

        public void SetupRenderer()
        {
            if (Config.Current == null)
                return;

            int shadowMapBase = 1;
            switch (Config.Current.ShadowQuality)
            {
                case Config.ShadowQualities.Low:
                    shadowMapBase = 2;
                    Renderer.ShadowQuality = ShadowQuality.SimpleN16Bit;
                    break;

                case Config.ShadowQualities.Medium:
                    Renderer.ShadowQuality = ShadowQuality.PcfN16Bit;
                    shadowMapBase = 8;
                    break;

                case Config.ShadowQualities.High:
                    shadowMapBase = 16;
                    Renderer.ShadowQuality = ShadowQuality.PcfN24Bit;
      
                    break;
            }
 
             Renderer.ShadowMapSize = (shadowMapBase) * 1024;
            SetupDebug();
        }

        public void SetupScene()
        {
            // start the game...
            World.Clear();
            World.CreateComponent<Octree>();
            World.CreateComponent<DebugRenderer>();

            var physics = World.CreateComponent<PhysicsWorld>();

            physics.SetGravity(new Vector3(0, -10, 0));
            physics.Interpolation = false;
            physics.Fps = 120;
            physics.SplitImpulse = true;
        }
    }
}
