using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;

namespace Client.Menus
{

    public class TutorialMenu : Stack.MenuScreen
    {
        public override string Name => "Tutorials";

        protected int ButtonWidth = 200;
        protected int XOffset = 20;

        Text HeaderText = null;
        public override void Init()
        {
            base.Init();

            HeaderText = AddHeaderString(ClientResources.SettingsTitle, 50);
         /*   AddVersionMarker();*/
            SetupTutorials();
            int yOffset = -60;
            var back = CreateButton(XOffset, yOffset, 250, 50, ClientResources.Back, HorizontalAlignment.Left, VerticalAlignment.Bottom);
            back.Pressed += Back_Pressed;

//             var apply = CreateButton(-XOffset, yOffset, 250, 50, ClientResources.Apply, HorizontalAlignment.Right, VerticalAlignment.Bottom);
//             apply.Pressed += Apply_Pressed;
        }

        protected void SetupTutorials()
        {

            List<string> buttonNames = new List<string>();
            foreach (var tut in Tutorials.TutorialAPI.AvalilbleTutorials.Keys)
            {
                buttonNames.Add(tut);
            }


            var buttons = CreateButtonColumnn(XOffset, 125, ButtonWidth, 36, 15, buttonNames.ToArray(), HorizontalAlignment.Left, VerticalAlignment.Top);

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Pressed += TutorialButtonPressed;
                buttons[i].HoverBegin += TutorialMenu_HoverBegin;
            }

        }

        private void TutorialMenu_HoverBegin(HoverBeginEventArgs obj)
        {
            // load description
        }

        private void TutorialButtonPressed(PressedEventArgs obj)
        {
           SelectTutorial(obj.Element.Name);
        }

        private void SelectTutorial(string current)
        {
            Tutorials.TutorialAPI.StartTutorial(current);
            Main.NewGame_Pressed(new PressedEventArgs());
        }

        private void Back_Pressed(PressedEventArgs obj)
        {
            Stack.Pop();
        }
    }
}
