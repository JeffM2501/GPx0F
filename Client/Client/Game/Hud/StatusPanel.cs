using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;
using Urho.Resources;

namespace Client.Game.Hud
{
    public class StatusPanel : BorderImage
    {
        public Player PlayerObject = null;

        private Text StatusText = null;
        private Text StatusText2 = null;

        protected FillBar ShieldBar = null;
        protected FillBar PowerBar = null;

        protected Sprite WeaponIcon = null;

        public void Setup(Player linkedPlayer, ResourceCache res)
        {
            ImageRect = new IntRect(48, 0, 64, 16);
            Border = new IntRect(4, 4, 4, 4);

            PlayerObject = linkedPlayer;

            this.RemoveAllChildren();

            if (linkedPlayer == null)
                return;

            Color DarkYellow = Color.FromHex("535d00");
            Color BrightYellow = Color.FromHex("e6f84f");

            Color DarkBlue = Color.FromHex("062165");
            Color BrightBlue = Color.FromHex("5ee7f7");

            var TextFont = res.GetFont("Fonts/Exo2-Regular.otf");

            int teamIconSize = Height / 2;
            WeaponIcon = new Sprite();
            AddChild(WeaponIcon);
            WeaponIcon.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            WeaponIcon.SetSize(teamIconSize, teamIconSize);
            WeaponIcon.Position = new IntVector2(0, 0);
            WeaponIcon.SetHotSpot(teamIconSize, 0);
            WeaponIcon.Texture = res.GetTexture2D("UI/GameIcons.Net/blaster.png");
        //    WeaponIcon.SetColor(linkedPlayer.GetIconColor());

            // shields
            int iconSize = Height / 3;
            var s = new Sprite();
            AddChild(s);
            s.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            s.SetSize(iconSize, iconSize);
            s.Position = new IntVector2(0, 0);
            s.SetHotSpot(0, 0);
            s.Texture = res.GetTexture2D("UI/GameIcons.Net/bubble-field.png");
            s.SetColor(BrightBlue);

            ShieldBar = new FillBar();
            AddChild(ShieldBar);
            ShieldBar.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            ShieldBar.SetSize(Width- teamIconSize-iconSize, iconSize / 2);
            ShieldBar.Position = new IntVector2(iconSize, iconSize / 4);
            ShieldBar.SetColor(new Color (DarkBlue, 0.75f));
            ShieldBar.SetFillColor(BrightBlue);
            ShieldBar.SetFillFactor(1);
            ShieldBar.SetInset(1);


            s = new Sprite();
            AddChild(s);
            s.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            s.SetSize(Height / 4, Height / 4);
            s.SetColor(BrightYellow);
            s.Position = new IntVector2(0, Height / 3);
            s.SetHotSpot(0, 0);
            s.Texture = res.GetTexture2D("UI/GameIcons.Net/battery-pack-alt.png");

            PowerBar = new FillBar();
            AddChild(PowerBar);
            PowerBar.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            PowerBar.SetSize(Width - teamIconSize - iconSize, iconSize / 2);
            PowerBar.Position = new IntVector2(iconSize, s.Position.Y + iconSize / 4);
            PowerBar.SetColor(new Color(DarkYellow, 0.75f));
            PowerBar.SetFillColor(BrightYellow);
            PowerBar.SetFillFactor(1);
            PowerBar.SetInset(1);

            s = new Sprite();
            AddChild(s);
            s.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            s.SetSize(Height / 4, Height / 4);
            s.SetColor(Color.White);
            s.Position = new IntVector2(0, 0);
            s.SetHotSpot(0, s.Height);
            s.Texture = res.GetTexture2D("UI/GameIcons.Net/speedometer.png");

            StatusText = new Text();
            AddChild(StatusText);
            StatusText.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            StatusText.SetSize(Width - iconSize, 16);
            StatusText.Position = new IntVector2(iconSize, 0);
            StatusText.SetFont(TextFont, 12);
            StatusText.Value = string.Empty;

            StatusText2 = new Text();
            AddChild(StatusText2);
            StatusText2.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            StatusText2.Position = new IntVector2(iconSize, -20);
            StatusText2.SetSize(Width - iconSize, 16);
            StatusText2.SetFont(TextFont, 12);
            StatusText2.Value = string.Empty;

        }

        public void DoUpdate(float deltaTime, float elapsedTime)
        {
            if (PlayerObject != null)
            {
                StatusText.Value = PlayerObject.PhysicsBody.LinearVelocity.LengthFast.ToString("F0");
                StatusText2.Value = (PlayerObject.OnGround ? "ground": "falling") + " : " + (PlayerObject.DidMove? "thrusting" : "coasting");

                PowerBar.SetFillFactor(PlayerObject.CurrentPower / PlayerObject.MaxPower);
                ShieldBar.SetFillFactor(PlayerObject.CurrentShields / PlayerObject.MaxShields);

                ShieldBar.Update(deltaTime);
                PowerBar.Update(deltaTime);
            }
        }
    }
}
