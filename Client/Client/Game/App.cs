using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;
using Urho.Actions;

using Urho.Audio;

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

           
            Input.KeyDown += Input_KeyDown;

            if (Config.Current != null && Config.Current.WinType == Config.WindowTypes.Window)
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
            base.OnUpdate(timeStep);
        }


        private void Input_KeyDown(KeyDownEventArgs args)
        {
            if (args.Key == Key.Esc)
                DoExit();
        }

        protected void SetMainViewport()
        {
            Renderer.SetViewport(0, new Viewport(Context, World, MainCamera, null));
        }

        public void SetupScene()
        {
            World = new Scene();
            World.CreateComponent<Octree>();
        }
    }
}
