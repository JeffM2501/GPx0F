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
    public class App : Application
    {
        protected Scene World = null;
        protected Camera MainCamera = null;

        protected Arena CurrentArena = null;

        public App(ApplicationOptions options = null) : base(options) { }

        protected override void Start()
        {
            base.Start();
            Input.SetMouseMode(MouseMode.Absolute);
            Input.SetMouseVisible(true);

            Input.ExitRequested += Input_ExitRequested;

            Menus.Stack.Setup(this);

            SetupScene();
            SetupMainMenu();

            Renderer.SetViewport(0, new Viewport(Context, World, MainCamera, null));
            Input.KeyDown += Input_KeyDown;

            if (Config.Current != null && Config.Current.WinType == Config.WindowTypes.Window)
                this.Graphics.SetWindowPosition(new IntVector2(Config.Current.WindowBounds.X, Config.Current.WindowBounds.Y));

            Graphics.WindowTitle = ClientResources.WindowTitle;
        }

        private void Input_ExitRequested(ExitRequestedEventArgs obj)
        {
            if (Config.Current != null && Config.Current.WinType == Config.WindowTypes.Window)
            {
                Config.Current.WindowBounds = new System.Drawing.Rectangle(Graphics.WindowPosition.X, Graphics.WindowPosition.Y, Graphics.Width, Graphics.Height);
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
            base.OnUpdate(timeStep);
        }


        private void Input_KeyDown(KeyDownEventArgs args)
        {
            if (args.Key == Key.Esc)
                DoExit();
        }

        public void SetupScene()
        {
            World = new Scene();
            World.CreateComponent<Octree>();
        }

        private Node musicNode = null;

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

            var music = ResourceCache.GetSound("Sounds/Ambient/425368__soundholder__ambient-meadow-near-forest-single-bird-and-eurasian-cranes-in-background-stereo-xy-mk012_01.ogg");
            music.Looped = true;
            musicNode = World.CreateChild("Music");
            SoundSource musicSource = musicNode.CreateComponent<SoundSource>();
             musicSource.Gain = 0.25f;
          //  musicSource.Attenuation = 1;
            // Set the sound type to music so that master volume control works correctly
            musicSource.SetSoundType(SoundType.Music.ToString());
            musicSource.Play(music);

        }

        public void DoExit()
        {
            if (Config.Current != null && Config.Current.WinType == Config.WindowTypes.Window)
            {
                Config.Current.WindowBounds = new System.Drawing.Rectangle(Graphics.WindowPosition.X, Graphics.WindowPosition.Y, Graphics.Width, Graphics.Height);
            }
            Exit();
        }

        private void Main_Quit(object sender, EventArgs e)
        {
            DoExit();
        }

        private void Main_StartGame(object sender, EventArgs e)
        {
            Menus.Stack.ClearAll();

            // start the game...
        }
    }
}
