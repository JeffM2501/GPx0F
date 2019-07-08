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

            World.GetComponent<PhysicsWorld>().PhysicsPreStep += PhysicsPreStep;
            
            Update += Game_Update;

            MainCamera = Geometry.MenuBackground.CreateCamera(World);
            SetMainViewport();

            CurrentArena = new Geometry.SimpleArena();
            CurrentArena.Setup(ResourceCache, World, 400);

            Hud.ChatPanel.AddChatText("Startup", Hud.ChatPanel.SystemSource);

          //  SetInputMode(true);
            // for now set the state to limboed and wait for a spawn
            SetLimboed();
        }

        private void PhysicsPreStep(PhysicsPreStepEventArgs obj)
        {
            UpdateFrameInput(obj.TimeStep);
        }

        protected void StopGame()
        {
            Update -= Game_Update;
        }

        protected void SetLimboed()
        {
            GameState = GameStates.Limboed;
            SetHudMessage(ClientResources.SpawnMessage);
        }

        private void Game_Update(Urho.UpdateEventArgs obj)
        {
            switch(GameState)
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

        protected void SetMaxInputs()
        {
            ThisFrameInput.SetMaxAxisVal(Config.AxisFunctions.Acceleration, 50);
            ThisFrameInput.SetMaxAxisVal(Config.AxisFunctions.Turning, 360);
            ThisFrameInput.SetMaxAxisVal(Config.AxisFunctions.SideSlide, 10);
            ThisFrameInput.SetMaxAxisVal(Config.AxisFunctions.Aiming, 180);
        }

        public void SpawnPlayer()
        {
            GameState = GameStates.Playing;
            SetMaxInputs();

            Vector3 pos = CurrentArena.GetSpawn();

            if (Me == null)
            {
                Me = new LocalPlayer("Me", Ships.TeamColors.Blue, World, ResourceCache);
                Me.Node.Position = pos;
                Me.Node.AddChild(MainCamera.Node);
                MainCamera.Node.Position = new Vector3(0, 1, 0);

                ThisFrameInput = Me.CurrentInput;
            }

            Hud.ChatPanel.AddChatText("Spawned at " + pos.ToString(), Hud.ChatPanel.SystemSource);
        }
    }
}
