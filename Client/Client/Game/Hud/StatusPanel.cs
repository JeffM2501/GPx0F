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

        public void Setup(Player linkedPlayer, ResourceCache res)
        {
            ImageRect = new IntRect(48, 0, 64, 16);
            Border = new IntRect(4, 4, 4, 4);

            PlayerObject = linkedPlayer;

            this.RemoveAllChildren();

            if (linkedPlayer == null)
                return;

            var TextFont = res.GetFont("Fonts/Exo2-Regular.otf");

            Sprite s = new Sprite();
            AddChild(s);
            s.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            s.SetSize(Height, Height);
            s.Position = new IntVector2(0, 0);
            s.SetHotSpot(s.Width, 0);
            s.Texture = res.GetTexture2D("UI/HudMarker1.png");
            s.SetColor(linkedPlayer.GetIconColor());

            StatusText = new Text();
            AddChild(StatusText);
            StatusText.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            StatusText.SetSize(Width, height: 16);
            StatusText.SetFont(TextFont, 12);
            StatusText.Value = string.Empty;

            StatusText2 = new Text();
            AddChild(StatusText2);
            StatusText2.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
            StatusText2.Position = new IntVector2(0, -20);
            StatusText2.SetSize(Width, height: 16);
            StatusText2.SetFont(TextFont, 12);
            StatusText2.Value = string.Empty;
        }

        public void DoUpdate(float deltaTime, float elapsedTime)
        {
            if (PlayerObject != null)
            {
                StatusText.Value = "F" + PlayerObject.PhysicsBody.AnisotropicFriction.ToString() + " : L" + PlayerObject.PhysicsBody.RollingFriction;
                StatusText2.Value = (PlayerObject.OnGround ? "ground": "falling") + " : " + (PlayerObject.DidMove? "thrusting" : "coasting");
            }
        }
    }
}
