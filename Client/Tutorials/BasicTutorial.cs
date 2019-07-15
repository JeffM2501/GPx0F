using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Client.Tutorials
{
    public class BasicTutorial : TutorialAPI.ITutorial
    {
        public string DisplayName => ClientResources.BasicTutorialName;

        public string DisplayText => ClientResources.BasicTutorialDescription;

        public bool UseSimpleArena => true;

        public int ArenaSize => 600;

        public bool Enabled => true;

        public int SortOrder => 1;

        public void Cleanup()
        {
        }

        public void Init(string langauge)
        {
        }

        public void Startup()
        {
           
        }

        public void Update(double timeStep)
        {
        }
    }
}
