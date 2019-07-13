using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;


namespace Client.Menus.Settings
{
    public class SettingsSubMenu : Stack.MenuScreen
    {
        public event EventHandler Dirty = null;

        public virtual void Apply()
        {

        }
    }

    public class SettingsFrame : Stack.MenuScreen
    {
        public override string Name => "Settings";

        protected List<SettingsSubMenu> SubScreens = new List<SettingsSubMenu>();

        protected int ButtonWidth = 200;
        protected int XOffset = 20;

        Text HeaderText = null;
        public override void Init()
        {
            base.Init();

            HeaderText = AddHeaderString(ClientResources.SettingsTitle, 50);
            AddVersionMarker();
            SetupSubScreens();
            int yOffset = -60;
            var back = CreateButton(XOffset, yOffset, 250, 50, ClientResources.Back, HorizontalAlignment.Left, VerticalAlignment.Bottom);
            back.Pressed += Back_Pressed;

            var apply = CreateButton(-XOffset, yOffset, 250, 50, ClientResources.Apply, HorizontalAlignment.Right, VerticalAlignment.Bottom);
            apply.Pressed += Apply_Pressed;
        }

        protected void SetupSubScreens()
        {
            SubScreens.Add(new RenderSettings());
            SubScreens.Add(new ControlSettings());
            SubScreens.Add(new AudioSettings());
            SubScreens.Add(new AccountSettings());

            List<string> buttonNames = new List<string>();
            foreach (var subScreen in SubScreens)
            {
                buttonNames.Add(subScreen.Name);
                subScreen.RootElement = new Window();
                RootElement.AddChild(subScreen.RootElement);
              
                subScreen.RootElement.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
                subScreen.RootElement.SetColor(new Color(Color.Black, 0.85f));
                subScreen.RootElement.SetSize(RootElement.Width - (XOffset * 3) - ButtonWidth, RootElement.Height - 250);
                subScreen.RootElement.SetPosition(XOffset + ButtonWidth + XOffset, 120);

                subScreen.Resources = Resources;

                // todo ,set anchors?

                subScreen.Init();

                subScreen.RootElement.Visible = false;
            }
   

            var buttons = CreateButtonColumnn(XOffset, 125, ButtonWidth, 36, 15, buttonNames.ToArray(), HorizontalAlignment.Left, VerticalAlignment.Top);

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Pressed += SubSettingsButtonPressed;
            }

            if (SubScreens.Count > 0)
            {
                SelectScreen(SubScreens[0]);
            }
        }

        private void SubSettingsButtonPressed(PressedEventArgs obj)
        {
            var screen = SubScreens.Find((x) => x.Name == obj.Element.Name);
            if (screen != null)
                SelectScreen(screen);
        }

        private void SelectScreen(SettingsSubMenu current)
        {
            foreach (var subScreen in SubScreens)
                subScreen.RootElement.Visible = false;

            current.RootElement.Visible = true;
            HeaderText.Value = current.Name;
        }

        private void Apply_Pressed(PressedEventArgs obj)
        {
            foreach (var subScreen in SubScreens)
                subScreen.Apply();

            Config.Current.Dirty();
            Stack.Pop();
        }

        private void Back_Pressed(PressedEventArgs obj)
        {
            Stack.Pop();
        }
    }
}
