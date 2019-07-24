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
        protected StatusPanel StatusWindow = null;

        protected Text HudCenterMessage = null;
        protected Window HudCenterMessageFrame = null;
        protected float HudCenterMessageLife = -1;

        protected Sprite Crosshairs = null;

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
            StatusWindow = new StatusPanel();
            HudRoot.AddChild(StatusWindow);
            StatusWindow.SetStyleAuto(null);
            StatusWindow.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            size = Math.Min((int)(Config.Current.RadarSize * 0.5f), UI.Root.Size.Y / 8);
            StatusWindow.SetSize(300, size);

            StatusWindow.SetColor(new Color(Color.Green, 0.5f));
            StatusWindow.SetPosition(0, 0);
            StatusWindow.Setup(Me, ResourceCache);


            Font font = ResourceCache.GetFont("Fonts/Exo2-Black.otf");

            HudCenterMessageFrame = new Window();
            HudRoot.AddChild(HudCenterMessage);
            HudCenterMessageFrame.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            HudCenterMessageFrame.SetPosition(0, 100);
            HudCenterMessageFrame.SetSize(HudRoot.Width -100, HudRoot.Height/3);
            HudCenterMessageFrame.SetColor(new Color(Color.Green, 0.5f));

            HudCenterMessage = new Text();
            HudCenterMessageFrame.AddChild(HudCenterMessage);
            HudCenterMessage.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
          //  HudCenterMessage.SetPosition(0, 100);
            HudCenterMessage.SetFont(font, 36);
            HudCenterMessage.SetColor(Color.FromHex("0F0F0F"));
            HudCenterMessage.Value = string.Empty;
            HudCenterMessage.Visible = true;

            HudCenterMessageFrame.Visible = false;

            Crosshairs = new Sprite();
            HudRoot.AddChild(Crosshairs);
            Crosshairs.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            Crosshairs.Texture = ResourceCache.GetTexture2D("UI/RoundCrosshair1.png");
            Crosshairs.SetSize(HudRoot.Height / 6, HudRoot.Height / 6);
            Crosshairs.SetHotSpot(Crosshairs.Width / 2, Crosshairs.Height / 2);
            Crosshairs.SetColor(new Color(Color.Gray, 0.5f));
            Crosshairs.BlendMode = BlendMode.Addalpha;

            Update += HudUpdate;
            ApplicationExiting += App_ApplicationExiting;
        }

        protected void SetHudPlayer()
        {
            if (StatusWindow != null)
                StatusWindow.Setup(Me, ResourceCache);
        }

        private void App_ApplicationExiting(object sender, EventArgs e)
        {
            Update -= HudUpdate;
        }

        private void HudUpdate(UpdateEventArgs obj)
        {
            if (Exiting)
                return;

            RadarWindow?.DoUpdate(obj.TimeStep, Time.ElapsedTime);
            ChatWindow?.DoUpdate(obj.TimeStep, Time.ElapsedTime);
            StatusWindow?.DoUpdate(obj.TimeStep, Time.ElapsedTime);

            if (HudCenterMessageFrame != null && HudCenterMessageFrame.Visible && HudCenterMessageLife > 0)
            {
                HudCenterMessageLife -= obj.TimeStep;
                if (HudCenterMessageLife < 0)
                    HudCenterMessageFrame.Visible = false;
            }
        }

        public void SetHudMessage(string text, float lifetime = -1)
        {
            HudCenterMessage.Value = text;
            HudCenterMessageFrame.Visible = text != string.Empty;
            if (lifetime > 0)
                HudCenterMessageLife = lifetime;
            else
                HudCenterMessageLife = -1;
        }
    }
}
