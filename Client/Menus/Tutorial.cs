using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;
using Urho.Urho2D;

namespace Client.Menus
{

    public class TutorialMenu : Stack.MenuScreen
    {
        public override string Name => "Tutorials";

        protected int ButtonWidth = 200;
        protected int XOffset = 20;

        protected string Current = string.Empty;

        Text HeaderText = null;
        Text DescriptionText = null;
        public override void Init()
        {
            base.Init();

            HeaderText = AddHeaderString(ClientResources.Tutorials, 50);

            Window frame = new Window();
            RootElement.AddChild(frame);
            frame.SetStyleAuto(null);
            frame.SetColor(new Color(Color.Black, 0.75f));
            frame.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top );
            frame.SetPosition(300, 100);
            frame.SetSize(RootElement.Width - 350, RootElement.Height - (275));


            DescriptionText = CreateLabel(25,25, string.Empty,HorizontalAlignment.Left,VerticalAlignment.Top,18,frame);
            DescriptionText.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            DescriptionText.SetPosition(25, 25);
            DescriptionText.SetSize(frame.Width - 50, frame.Height - 25);

            Texture text = new Texture();


         /*   AddVersionMarker();*/
            SetupTutorials();
            int yOffset = -60;
            var back = CreateButton(XOffset, yOffset, 250, 50, ClientResources.Back, HorizontalAlignment.Left, VerticalAlignment.Bottom);
            back.Pressed += Back_Pressed;

            var start = CreateButton(-XOffset, yOffset, 250, 50, ClientResources.Start, HorizontalAlignment.Right, VerticalAlignment.Bottom);
            start.Pressed += Start_Pressed;
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

            if (buttonNames.Count > 0)
                SelectTutorial(buttonNames[0]);
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
            Current = current;

            var tut = Tutorials.TutorialAPI.AvalilbleTutorials[current];

            DescriptionText.Value = tut.DisplayText;

        }

        private void Start_Pressed(PressedEventArgs obj)
        {
            Client.Game.App.StartupArguments args = new Game.App.StartupArguments();
            args.Host = Current;
            Main.CallStartGame(args);
        }

        private void Back_Pressed(PressedEventArgs obj)
        {
            Stack.Pop();
        }
    }
}
