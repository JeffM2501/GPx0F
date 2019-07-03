using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Update += Game_Update;

            MainCamera = Geometry.MenuBackground.CreateCamera(World);
            SetMainViewport();

            CurrentArena = new Geometry.SimpleArena();
            CurrentArena.Setup(ResourceCache, World, 400);

            // for now set the state to limboed and wait for a spawn
            GameState = GameStates.Limboed;
            SetupHud();
        }

        protected void StopGame()
        {
            Update -= Game_Update;
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

                case GameStates.Limboed:
                    // wait for spawn from user
                    return;

                case GameStates.Playing:
                    // play the game
                    return;
            }
        }

    }
}
