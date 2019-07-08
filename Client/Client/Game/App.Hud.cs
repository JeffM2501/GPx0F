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
        protected ChatPanel ChatWindow = null;

        protected Text HudCenterMessage = null;
        protected float HudCenterMessageLife = -1;

		protected void SetupHud()
        {
            if (HudRoot != null)
                HudRoot.Remove();

			HudRoot = new Urho.Gui.UIElement();
			UI.Root.AddChild(HudRoot);
			HudRoot.Size = new IntVector2(UI.Root.Size.X, UI.Root.Size.Y);
			HudRoot.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);

            // add the radar
            RadarWindow = new Radar();
            HudRoot.AddChild(RadarWindow);
            RadarWindow.SetStyleAuto(null);
            RadarWindow.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Bottom);
            int size = Math.Min(Config.Current.RadarSize, UI.Root.Size.Y / 3);
            RadarWindow.SetSize(size, size);
            RadarWindow.SetColor(new Color(Color.Green, 0.75f));
            RadarWindow.SetPosition(0, 0);
            RadarWindow.Setup(ResourceCache);

            // add the chat window
            ChatWindow = new ChatPanel();
            HudRoot.AddChild(ChatWindow);
            ChatWindow.SetStyleAuto(null);
            ChatWindow.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            size = Math.Min((int)(Config.Current.RadarSize * 0.75f), UI.Root.Size.Y / 4);
            ChatWindow.SetSize(HudRoot.Width - RadarWindow.Width - 25, size);
            ChatWindow.SetColor(new Color(Color.Green, 0.5f));
            ChatWindow.SetPosition(0, 0);
            ChatWindow.Setup(ResourceCache);

            // add the player window
            var playerList = new Window();
            HudRoot.AddChild(playerList);
            playerList.SetStyleAuto(null);
            playerList.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            size = Math.Min((int)(Config.Current.RadarSize * 0.5f), UI.Root.Size.Y / 8);
            playerList.SetSize(size, 300);

            playerList.SetColor(new Color(Color.Green, 0.5f));
            playerList.SetPosition(0, 0);

            // add the status window
            var statusWindow = new Window();
            HudRoot.AddChild(statusWindow);
            statusWindow.SetStyleAuto(null);
            statusWindow.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            size = Math.Min((int)(Config.Current.RadarSize * 0.5f), UI.Root.Size.Y / 8);
            statusWindow.SetSize(300, size);

            statusWindow.SetColor(new Color(Color.Green, 0.5f));
            statusWindow.SetPosition(0, 0);

            Font font = ResourceCache.GetFont("Fonts/Exo2-Black.otf");

            HudCenterMessage = new Text();
            HudRoot.AddChild(HudCenterMessage);
            HudCenterMessage.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            HudCenterMessage.SetPosition(0, 0);
            HudCenterMessage.SetFont(font, 36);
            HudCenterMessage.SetColor(Color.White);
            HudCenterMessage.Value = string.Empty;
            HudCenterMessage.Visible = false;

            this.Update += HudUpdate;
        }

        private void HudUpdate(UpdateEventArgs obj)
        {
            RadarWindow?.DoUpdate(obj.TimeStep, Time.ElapsedTime);
            ChatWindow?.DoUpdate(obj.TimeStep, Time.ElapsedTime);

            if (HudCenterMessage != null && HudCenterMessage.Visible && HudCenterMessageLife > 0)
            {
                HudCenterMessageLife -= obj.TimeStep;
                if (HudCenterMessageLife < 0)
                    HudCenterMessage.Visible = false;
            }
        }

        public void SetHudMessage(string text, float lifetime = -1)
        {
            HudCenterMessage.Value = text;
            HudCenterMessage.Visible = text != string.Empty;
            if (lifetime > 0)
                HudCenterMessageLife = lifetime;
            else
                HudCenterMessageLife = -1;
        }
    }
}
