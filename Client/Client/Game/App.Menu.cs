﻿using System;
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
            World.Clear();
            World.CreateComponent<Octree>();
            MainCamera = Geometry.MenuBackground.CreateCamera(World);
            SetMainViewport();

            CurrentArena = new Geometry.MenuBackground();
            CurrentArena.Setup(ResourceCache, World, 200);

            Menus.Stack.ClearAll();

            var main = new Menus.Main();
            main.StartGame += Main_StartGame;
            main.Quit += Main_Quit;
            Menus.Stack.Push(main);

            var music = ResourceCache.GetSound("Sounds/Ambient/425368__soundholder__ambient-meadow-near-forest-single-bird-and-eurasian-cranes-in-background-stereo-xy-mk012_01.ogg");
            music.Looped = true;
            var musicNode = World.CreateChild("Music");
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

        private void Main_StartGame(object sender, EventArgs e)
        {
            Menus.Stack.ClearAll();

            // start the game...
            World.Clear();
            World.CreateComponent<Octree>();
            World.CreateComponent<DebugRenderer>();

            var physics = World.CreateComponent<PhysicsWorld>();
           
            physics.SetGravity(new Vector3(0, -10, 0));
            physics.Interpolation = false;
            StartGame();
        }
    }
}
