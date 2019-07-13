using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;

namespace Client.Menus.Settings
{
    public class ControlSettings : SettingsSubMenu
    {
        public override string Name => ClientResources.ControlSettingsName;

        Slider MouseSlider = null;

        public override void Init()
        {
            base.Init();

            int xOffset = 20;

            int yOffset = 5;

            var label = CreateLabel(xOffset, yOffset, " - Mouse Turn Sensitivity +", HorizontalAlignment.Left, VerticalAlignment.Top, 14, RootElement);
            MouseSlider = new Slider ();
            label.AddChild(MouseSlider);
            MouseSlider.SetStyleAuto(null);
            MouseSlider.SetColor(Color.Blue);
            MouseSlider.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            MouseSlider.SetPosition(0, 30);
            MouseSlider.SetSize(300, 20);
            MouseSlider.Range = 500;
            MouseSlider.Value = 500 - Config.Current.MouseXSensitivity;
     
            yOffset += 25;
        }

        public override void Apply()
        {
            Config.Current.MouseXSensitivity = 500 - MouseSlider.Value;
            if (Config.Current.MouseXSensitivity < 1)
                Config.Current.MouseXSensitivity = 1;

            base.Apply();
        }
    }
}
