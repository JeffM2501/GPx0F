using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Maps;
using Urho;

namespace Client.Tutorials
{
    public class DebugTutorial : TutorialAPI.ITutorial
    {
        public string DisplayName => ClientResources.DebugTutorialName;

        public string DisplayText => ClientResources.DebugTutorialDescription;

        public bool UseSimpleArena => true;

        public int ArenaSize => 400;

#if DEBUG
        public bool Enabled => true;

#else
        public bool Enabled => false;
#endif

        public void Cleanup()
        {
            TutorialAPI.GameApp.RequestSpawn -= GameApp_RequestSpawn;
        }

        public void Init(string langauge)
        {

        }

        public void Startup()
        {
            // throw a box into the map
            SimpleArena sa = TutorialAPI.CurrentArena as SimpleArena;
            if (sa != null)
            {
                sa.MakeBox("TestBox", new Vector3(10, 2.5f, 10), new Vector3(5, 5, 5), "Legacy/zone/Materials/BoxWall.xml", new Vector2(5 * sa.WallRepeat, 5 * sa.WallRepeat));
            }

            TutorialAPI.GameApp.RequestSpawn += GameApp_RequestSpawn;
        }

        private void GameApp_RequestSpawn(object sender, EventArgs e)
        {
            TutorialAPI.GameApp.SpawnPlayer(TutorialAPI.CurrentArena.GetSpawn(), Quaternion.Identity);
        }

        public void Update(double timeStep)
        {
        }
    }
}
