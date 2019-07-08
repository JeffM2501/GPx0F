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
        protected enum GameStates
        {
            Inactive,
            Connecting,
            Joining,
            Limboed,
            Playing,
        }
        protected GameStates GameState = GameStates.Inactive;

        protected List<Player> Players = new List<Player>();
        protected LocalPlayer Me = null;

        protected void StartGame()
        {
            SetupHud();
            MainCamera = Geometry.MenuBackground.CreateCamera(World);
            SetMainViewport();

            CurrentArena = new Geometry.SimpleArena();
            CurrentArena.Setup(ResourceCache, World, 400);

            Hud.ChatPanel.AddChatText("Startup", Hud.ChatPanel.SystemSource);

            SetInputMode(true);
            // for now set the state to limboed and wait for a spawn
            SetLimboed();
        }

        protected void StopGame()
        {
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
            if (ThisFrameInput.ButtonValues[Config.ButtonFunctions.Menu])
            {
                SetInputMode(false);
                // show in game menu
            }

            if (GameState == GameStates.Limboed)
            {
                if (ThisFrameInput.ButtonValues[Config.ButtonFunctions.Spawn])
                {
                    SetHudMessage(string.Empty);
                    SpawnPlayer();
                }
            }

            if (GameState == GameStates.Playing)
            {
            }
        }

        public void SpawnPlayer()
        {
            GameState = GameStates.Playing;

            Vector3 pos = CurrentArena.GetSpawn();

            if (Me == null)
            {
                var Ship = Ships.GetShipNode(ResourceCache, World, Ships.TeamColors.Blue, "Mk3");
               

                Me = Ship.Node.CreateComponent<LocalPlayer>();
                Me.Setup("Me");
                Me.Node.Position = pos;
                Me.Node.AddChild(MainCamera.Node);
                MainCamera.Node.Position = new Vector3(0, 2, -5);

                ThisFrameInput = Me.CurrentInput;
            }

            Hud.ChatPanel.AddChatText("Spawned at " + pos.ToString(), Hud.ChatPanel.SystemSource);
        }
    }
}
