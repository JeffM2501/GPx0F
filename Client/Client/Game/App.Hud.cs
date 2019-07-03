using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;

using Client.Game.Hud;

namespace Client.Game
{
	public partial class App
    {
        protected UIElement HudRoot = null;

        protected Radar RadarWindow = null;

		protected void SetupHud()
        {
            if (HudRoot != null)
                HudRoot.Remove();

			HudRoot = new Urho.Gui.UIElement();
			UI.Root.AddChild(HudRoot);
			HudRoot.Size = new IntVector2(UI.Root.Size.X, UI.Root.Size.Y);
			HudRoot.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);

            // add the radar
            RadarWindow = new Radar();
            HudRoot.AddChild(RadarWindow);
            RadarWindow.SetStyleAuto(null);
            RadarWindow.SetColor(new Color(Color.Green, 0.75f));
            RadarWindow.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            RadarWindow.SetPosition(0, 0);

            int size = Math.Min(Config.Current.RadarSize, UI.Root.Size.Y / 3);
            RadarWindow.SetSize(size, size);
            RadarWindow.Setup(ResourceCache);

            this.Update += HudUpdate;
        }

        private void HudUpdate(UpdateEventArgs obj)
        {
            RadarWindow?.DoUpdate(obj.TimeStep, Time.ElapsedTime);
        }
    }
}
