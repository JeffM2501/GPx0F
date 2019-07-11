using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;

namespace Client.Menus.Settings
{
    public class RenderSettings : SettingsSubMenu
    {
        CheckBox FullScreen = null;
        CheckBox VSync = null;
        CheckBox MultiSample = null;

        public override string Name => ClientResources.RenderSettingsName;

        public override void Init()
        {
            base.Init();
            int xOffset = 20;

            int yOffset = 5;

            var label = CreateLabel(xOffset, yOffset, "Fullscreen", HorizontalAlignment.Left, VerticalAlignment.Top, 14, RootElement);
            FullScreen = new CheckBox();
            label.AddChild(FullScreen);
            FullScreen.SetStyleAuto(null);
            FullScreen.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            FullScreen.SetPosition(25, 6);
            FullScreen.SetSize(20, 20);
            FullScreen.Checked = Config.Current.WinType != Config.WindowTypes.Window;

            label = CreateLabel(xOffset + 150, yOffset, "VSync", HorizontalAlignment.Left, VerticalAlignment.Top, 14, RootElement);
            VSync = new CheckBox();
            label.AddChild(VSync);
            VSync.SetStyleAuto(null);
            VSync.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            VSync.SetPosition(25, 6);
            VSync.SetSize(20, 20);
            VSync.Checked = Config.Current.LimitFPS;

            label = CreateLabel(xOffset + 300, yOffset, "Multisampling", HorizontalAlignment.Left, VerticalAlignment.Top, 14, RootElement);
            MultiSample = new CheckBox();
            label.AddChild(MultiSample);
            MultiSample.SetStyleAuto(null);
            MultiSample.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            MultiSample.SetPosition(25, 6);
            MultiSample.SetSize(20, 20);
            MultiSample.Checked = Config.Current.Multisample > 0;

            yOffset += 25;
            // second row

        }

        public override void Apply()
        {
            base.Apply();

            Config.Current.WinType = FullScreen.Checked ? Config.WindowTypes.FullScreen : Config.WindowTypes.Window;
            Config.Current.Multisample = MultiSample.Checked ? 16 : 0;
            Config.Current.LimitFPS = VSync.Checked;
        }
    }
}
