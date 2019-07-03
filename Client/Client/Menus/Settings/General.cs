using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;

namespace Client.Menus.Settings
{
    public class General : Stack.MenuScreen
    {
        public override string Name => "General Settings";

        CheckBox FullScreen = null;
        CheckBox VSync = null;
        CheckBox MultiSample = null;

        public override void Init()
        {
            base.Init();

            AddHeaderString(ClientResources.SettingsTitle, 64);
            AddVersionMarker();
            int xOffset = 20;

            Window frame = new Window();
            RootElement.AddChild(frame);
            frame.SetStyleAuto(null);
            frame.SetColor(new Color(Color.Black, 0.75f));
            frame.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            frame.SetPosition(0, 0);
            frame.SetSize(1280,500);
            CreateLabel(xOffset, xOffset, "Rendering", HorizontalAlignment.Left, VerticalAlignment.Top,18,frame);


            var label = CreateLabel(xOffset, xOffset*3, "Fullscreen", HorizontalAlignment.Left, VerticalAlignment.Top, 14, frame);
            FullScreen = new CheckBox();
            label.AddChild(FullScreen);
            FullScreen.SetStyleAuto(null);
            FullScreen.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            FullScreen.SetPosition(25, 6);
            FullScreen.SetSize(20, 20);
            FullScreen.Checked = Config.Current.WinType != Config.WindowTypes.Window;

            label = CreateLabel(xOffset + 150, xOffset * 3, "VSync", HorizontalAlignment.Left, VerticalAlignment.Top, 14, frame);
            VSync = new CheckBox();
            label.AddChild(VSync);
            VSync.SetStyleAuto(null);
            VSync.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            VSync.SetPosition(25, 6);
            VSync.SetSize(20, 20);
            VSync.Checked = Config.Current.LimitFPS;

            label = CreateLabel(xOffset + 300, xOffset * 3, "Multisampling", HorizontalAlignment.Left, VerticalAlignment.Top, 14, frame);
            MultiSample = new CheckBox();
            label.AddChild(MultiSample);
            MultiSample.SetStyleAuto(null);
            MultiSample.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            MultiSample.SetPosition(25, 6);
            MultiSample.SetSize(20, 20);
            MultiSample.Checked = Config.Current.Multisample > 0;

            int yOffset = -60;
            var back = CreateButton(xOffset, yOffset, 250, 50, ClientResources.Back, HorizontalAlignment.Left, VerticalAlignment.Bottom);
            back.Pressed += Back_Pressed;

            var apply = CreateButton(-xOffset, yOffset, 250, 50, ClientResources.Apply, HorizontalAlignment.Right, VerticalAlignment.Bottom);
            apply.Pressed += Apply_Pressed;
        }

        private void Apply_Pressed(PressedEventArgs obj)
        {
            // apply all the temp settings from the GUI

            Config.Current.WinType = FullScreen.Checked ? Config.WindowTypes.FullScreen : Config.WindowTypes.Window;
            Config.Current.Multisample = MultiSample.Checked ? 16 : 0;
            Config.Current.LimitFPS = VSync.Checked;



            Config.Current.Dirty();
            Stack.Pop();
        }

        private void Back_Pressed(PressedEventArgs obj)
        {
            Stack.Pop();
        }

    }
}
