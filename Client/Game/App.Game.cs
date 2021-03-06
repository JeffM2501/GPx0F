﻿using System;
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
        public class StartupArguments
        {
            public string Host = string.Empty;
            public int port = -1;
        }


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

        protected void StartGame(StartupArguments arguments)
        {
            SetupHud();
            MainCamera = CreateCamera(State.RootScene);
            MainCamera.Node.SetWorldPosition(new Vector3(0, 2, 0));

            SetMainViewport();
            float ArenaSize = 800;

            if (arguments.Host != string.Empty && arguments.port < 0)
            {
                Tutorials.TutorialAPI.GameApp = this;
                Tutorials.TutorialAPI.StartTutorial(arguments.Host);

                ArenaSize = Tutorials.TutorialAPI.CurrentTutorial.ArenaSize;
                if (Tutorials.TutorialAPI.CurrentTutorial.UseSimpleArena)
                    State.World = new SimpleArena();
                else
                    State.World = new Arena();  // TODO, get world from server

                arguments.Host = string.Empty; // no server to connect to, the state will be managed by the tutorial
            }
            else
            {
                State.World = new Arena();
            }

            if (arguments.port == int.MaxValue)
            {
                arguments.port = 2501;
            }

            if (arguments.Host != string.Empty)
            {
                Connect(arguments.Host, arguments.port);
            }

            State.World.Setup(ResourceCache, State.RootScene, ArenaSize);
            MainCamera.FarClip = ArenaSize * 2;
            MainCamera.NearClip = 0.1f;

            Tutorials.TutorialAPI.GameApp = this;
            Tutorials.TutorialAPI.CurrentArena = State.World;
            Tutorials.TutorialAPI.CurrentTutorial?.Startup();

            Hud.ChatPanel.AddChatText("Startup", Hud.ChatPanel.SystemSource);

            SetInputMode(true);
            // for now set the state to limboed and wait for a spawn
            if (Me == null)
                SetLimboed();
        }

        protected void StopGame()
        {
            Tutorials.TutorialAPI.StopTutorial();

            State.ClearWorld();
            if (State.RootScene != null)
            {
                State.RootScene.Remove();
                State.RootScene = null;
            }
            State = null;

            GameState = GameStates.Inactive;
            Update -= Game_Update;
        }

        protected void SetLimboed()
        {
            GameState = GameStates.Limboed;
            SetHudMessage(ClientResources.SpawnMessage);
        }

        protected void SetConnecting()
        {
            GameState = GameStates.Connecting;
            SetHudMessage(ClientResources.ConnectMessage + NetServerName);
        }

        protected void SetJoining()
        {
            GameState = GameStates.Joining;
            SetHudMessage(ClientResources.JoinGame);
        }

        private void Game_Update(Urho.UpdateEventArgs obj)
        {
            if (Exiting)
                return;
            PollMessages();

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
            SpawnPlayer(State.World.GetSpawn(), Quaternion.Identity);
        }

        public void SpawnPlayer(Vector3 pos, Quaternion orient)
        {
            GameState = GameStates.Playing;

            if (Me == null)
            {
                var Ship = Ships.GetShipNode(ResourceCache, State.RootScene, Ships.TeamColors.Blue, "Mk3");

                Me = Ship.Node.CreateComponent<LocalPlayer>();
                Me.Setup("Me");
                Me.AttachCamera(MainCamera);
                SetHudPlayer();
                Me.Spawn(pos, orient);
                ThisFrameInput = Me.CurrentInput;
                State.AddPlayer(Me);
            }

            Hud.ChatPanel.AddChatText(ClientResources.SpawnHudMesage + pos.ToString(), Hud.ChatPanel.SystemSource);
        }
    }
}
