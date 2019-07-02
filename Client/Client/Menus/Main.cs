using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;

namespace Client.Menus
{
    public class Main : Menus.Stack.MenuScreen
    {
        public override string Name => "MainMenu";

        public event EventHandler StartGame = null;
        public event EventHandler Quit = null;

        public override void Init()
        {
            base.Init();

            AddHeaderString("GPx0F");

            var copyrightText = new Text();
            copyrightText.Value = ClientResources.CopyrightText;
            copyrightText.HorizontalAlignment = HorizontalAlignment.Left;
            copyrightText.VerticalAlignment = VerticalAlignment.Bottom;
            copyrightText.SetFont(Resources.GetFont("Fonts/Exo2-Medium.otf"), 14);
            copyrightText.Position = new IntVector2(20, -20);
            copyrightText.SetColor(Color.White);
            copyrightText.SetMaxAnchor(0, 1);
            copyrightText.SetMinAnchor(0, 1);
            RootElement.AddChild(copyrightText);

            AddVersionMarker();

            var buttons = CreateButtonColumnn(-150, 100, 400, 75, 25, new string[] { ClientResources.NewGame, ClientResources.JoinGame, ClientResources.Settings, ClientResources.Credits, ClientResources.Exit }, HorizontalAlignment.Right, VerticalAlignment.Top);

            buttons[0].Pressed += NewGame_Pressed;
            buttons[2].Pressed += Settings_Pressed;
            buttons[3].Pressed += CreditsPressed;
            buttons[4].Pressed += Quit_Pressed;
        }

        private void CreditsPressed(PressedEventArgs obj)
        {
            Stack.Push(new Settings.General());
        }

        private void Settings_Pressed(PressedEventArgs obj)
        {
            Stack.Push(new Settings.General());
        }

        private void NewGame_Pressed(PressedEventArgs obj)
        {
            StartGame?.Invoke(this, EventArgs.Empty);
        }

        private void Quit_Pressed(PressedEventArgs obj)
        {
            Quit?.Invoke(this, EventArgs.Empty);
        }
    }
}
