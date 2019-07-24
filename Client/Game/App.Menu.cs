using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Audio;
using Urho.Physics;

namespace Client.Game
{
    public partial class App
    {
        private bool Exiting = false;
        public event EventHandler ApplicationExiting = null;

        private void Config_ApplyChanges(object sender, EventArgs e)
        {
            if (Config.Current.WinType == Config.WindowTypes.FullScreen != Graphics.Fullscreen)
                Graphics.ToggleFullscreen();
        }

        private void Input_ExitRequested(ExitRequestedEventArgs obj)
        {
            PreExit();
        }

        protected SoundSource BGMSource = null;

        private void SetupMainMenu()
        {
            State.ClearWorld();
            MainCamera = CreateCamera(State.RootScene);
            MainCamera.Node.SetWorldPosition(new Vector3(0, 1, -5));
            SetMainViewport();

            State.World = new Menus.Geometry.MenuBackground();
            State.World.Setup(ResourceCache, State.RootScene, 200);

            Menus.Stack.ClearAll();

            var main = new Menus.Main();
            Menus.Main.StartGame += Main_StartGame;
            Menus.Main.Quit += Main_Quit;
            Menus.Stack.Push(main);

            var music = ResourceCache.GetSound("Sounds/Ambient/425368__soundholder__ambient-meadow-near-forest-single-bird-and-eurasian-cranes-in-background-stereo-xy-mk012_01.ogg");
            music.Looped = true;
            var musicNode = State.RootScene.CreateChild("Music");
            BGMSource = musicNode.CreateComponent<SoundSource>();
            BGMSource.Gain = Config.Current.MusicVolume;
            //  musicSource.Attenuation = 1;
            // Set the sound type to music so that master volume control works correctly
            BGMSource.SetSoundType(SoundType.Music.ToString());
            BGMSource.Play(music);

        }

        protected void PreExit()
        {
            Exiting = true;
            ApplicationExiting?.Invoke(this, EventArgs.Empty);
            StopGame();

            if (Config.Current != null && Config.Current.WinType == Config.WindowTypes.Window)
            {
                Config.Current.WindowBounds = new System.Drawing.Rectangle(Graphics.WindowPosition.X, Graphics.WindowPosition.Y, Graphics.Width, Graphics.Height);
            }
        }

        protected void DoExit()
        {
            PreExit();
            Exit();
        }

        private void Main_Quit(object sender, EventArgs e)
        {
            DoExit();
        }

        private void Main_StartGame(StartupArguments args)
        {
            Menus.Stack.ClearAll();
            SetupScene();
            StartGame(args);
        }
    }
}
