using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;

using Client.Game;

namespace Client.Menus
{
    public class Main : Menus.Stack.MenuScreen
    {
        public override string Name => "MainMenu";

        public delegate void StartupCallback(App.StartupArguments args);

        public static event StartupCallback StartGame = null;
        public static event EventHandler Quit = null;

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

            var buttons = CreateButtonColumnn(-150, 100, 400, 50, 25, new string[] { ClientResources.NewGame, ClientResources.JoinGame, ClientResources.Tutorials, ClientResources.Settings, ClientResources.Credits, ClientResources.Exit }, HorizontalAlignment.Right, VerticalAlignment.Top);

            buttons[0].Pressed += QuickStart_Pressed;
            buttons[1].Pressed += JoinGame_Pressed;
            buttons[2].Pressed += Tutorials_Pressed;
            buttons[3].Pressed += Settings_Pressed;
            buttons[4].Pressed += CreditsPressed;
            buttons[5].Pressed += Quit_Pressed;
        }

        private void Tutorials_Pressed(PressedEventArgs obj)
        {
            Stack.Push(new TutorialMenu());
        }

        private void CreditsPressed(PressedEventArgs obj)
        {
            Stack.Push(new Credits());
        }

        private void Settings_Pressed(PressedEventArgs obj)
        {
            Stack.Push(new Settings.SettingsFrame());
        }

        private void JoinGame_Pressed(PressedEventArgs obj)
        {
            App.StartupArguments args = new App.StartupArguments();
            args.Host = "localhost";
            args.port = int.MaxValue;
            CallStartGame(args);
        }

        private void QuickStart_Pressed(PressedEventArgs obj)
        {
            App.StartupArguments args = new App.StartupArguments();
            // quick start
#if DEBUG
            if (Tutorials.TutorialAPI.CurrentTutorial == null)
                args.Host = Tutorials.TutorialAPI.AvalilbleTutorials.Values.ToArray()[0].DisplayName;
#else
            // TODO find network game, or start basic tutorial

#endif
            CallStartGame(args);
        }


        public static void CallStartGame(App.StartupArguments args)
        {
            StartGame?.Invoke(args);
        }

        private void Quit_Pressed(PressedEventArgs obj)
        {
            Quit?.Invoke(this, EventArgs.Empty);
        }
    }
}
