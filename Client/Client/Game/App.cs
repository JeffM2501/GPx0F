using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;
using Urho.Actions;

namespace Client.Game
{
    public class App : Application
    {
        protected Scene World = null;
        protected Camera MainCamera = null;

        protected Arena CurrentArena = null;

        public App(ApplicationOptions options = null) : base(options) { }

        protected override void Start()
        {
            Menus.Stack.Setup(this);

            SetupScene();
            SetupMainMenu();

            Renderer.SetViewport(0, new Viewport(Context, World, MainCamera, null));
            Input.KeyDown += Input_KeyDown;
        }


        private void Input_KeyDown(KeyDownEventArgs args)
        {
            if (args.Key == Key.Esc)
                Exit();
        }

        public void SetupScene()
        {
            World = new Scene();
            World.CreateComponent<Octree>();
        }

        public void SetupMainMenu()
        {
            World.Clear();
            World.CreateComponent<Octree>();
            MainCamera = Geometry.MenuBackground.CreateCamera(World);

            CurrentArena = new Geometry.MenuBackground();
            CurrentArena.Setup(ResourceCache, World, 200);

            Menus.Stack.ClearAll();

             var main = new Menus.Main();
             main.StartGame += Main_StartGame;
             main.Quit += Main_Quit;
             Menus.Stack.Push(main);
        }

        private void Main_Quit(object sender, EventArgs e)
        {
            Config.Current.Save();
            Exit();
        }

        private void Main_StartGame(object sender, EventArgs e)
        {
            Menus.Stack.ClearAll();

            // start the game...
        }
    }
}
