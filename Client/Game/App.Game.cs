using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Physics;

using Game;
using Game.Maps;

namespace Client.Game
{
    public partial class App
    {
        public enum GameStates
        {
            Inactive,
            Connecting,
            Joining,
            Limboed,
            Playing,
        }
        public GameStates GameState = GameStates.Inactive;

        internal List<ClientPlayer> Players = new List<ClientPlayer>();
        internal LocalPlayer Me = null;

        public event EventHandler RequestSpawn = null;

        protected void StartGame()
        {
            SetupHud();
            MainCamera = CreateCamera(World);
            MainCamera.Node.SetWorldPosition(new Vector3(0, 2, 0));

            SetMainViewport();
            float ArenaSize = 800;

            if (Tutorials.TutorialAPI.CurrentTutorial != null)
            {
                Tutorials.TutorialAPI.GameApp = this;

                ArenaSize = Tutorials.TutorialAPI.CurrentTutorial.ArenaSize;
                if (Tutorials.TutorialAPI.CurrentTutorial.UseSimpleArena)
                    CurrentArena = new SimpleArena();
                else
                    CurrentArena = new Arena();
            }
            else
            {
                CurrentArena = new SimpleArena();
            }

            CurrentArena.Setup(ResourceCache, World, ArenaSize);
            MainCamera.FarClip = ArenaSize * 2;
            MainCamera.NearClip = 0.1f;

            Tutorials.TutorialAPI.GameApp = this;
            Tutorials.TutorialAPI.CurrentArena = CurrentArena;
            Tutorials.TutorialAPI.CurrentTutorial?.Startup();

            Hud.ChatPanel.AddChatText("Startup", Hud.ChatPanel.SystemSource);

            SetInputMode(true);
            // for now set the state to limboed and wait for a spawn
            SetLimboed();
        }

        protected void StopGame()
        {
            Tutorials.TutorialAPI.StopTutorial();

            if (World != null)
            {
                World.Clear();
                World.Remove();
                World = null;
            }
            CurrentArena = null;

            GameState = GameStates.Inactive;
            Update -= Game_Update;
        }

        protected void SetLimboed()
        {
            GameState = GameStates.Limboed;
            SetHudMessage(ClientResources.SpawnMessage);
        }

        private void Game_Update(Urho.UpdateEventArgs obj)
        {
            if (Exiting)
                return;

            UpdateFrameInput(obj.TimeStep);

            switch (GameState)
            {
                case GameStates.Inactive:
                    return;

                case GameStates.Connecting:
                    // check for connection
                    return;

                case GameStates.Joining:
                    // finish negotiation
                    return;

                case GameStates.Playing:
                case GameStates.Limboed:
                    HandleGameplay(obj.TimeStep);
                    return;
            }
        }

        private void HandleGameplay(float deltaTime)
        {
            Tutorials.TutorialAPI.UpdateTutorial(deltaTime);

            if (ThisFrameInput.ButtonValues[ButtonFunctions.Menu])
            {
                SetInputMode(false);
                // show in game menu
            }

            if (GameState == GameStates.Limboed)
            {
                if (ThisFrameInput.ButtonValues[ButtonFunctions.Spawn])
                {
                    SetHudMessage(string.Empty);
                    RequestSpawn?.Invoke(this, EventArgs.Empty);
                }
            }

            if (GameState == GameStates.Playing)
            {
               
            }
        }

        public void SpawnPlayer()
        {
            SpawnPlayer(CurrentArena.GetSpawn(), Quaternion.Identity);
        }

        public void SpawnPlayer(Vector3 pos, Quaternion orient)
        {
            GameState = GameStates.Playing;

            if (Me == null)
            {
                var Ship = Ships.GetShipNode(ResourceCache, World, Ships.TeamColors.Blue, "Mk3");

                Me = Ship.Node.CreateComponent<LocalPlayer>();
                Me.Setup("Me");
                Me.AttachCamera(MainCamera);
                SetHudPlayer();
                Me.Spawn(pos, orient);
                ThisFrameInput = Me.CurrentInput;
            }

            Hud.ChatPanel.AddChatText(ClientResources.SpawnHudMesage + pos.ToString(), Hud.ChatPanel.SystemSource);
        }
    }
}
