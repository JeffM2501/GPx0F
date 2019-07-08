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
    public class Radar : BorderImage
    {
        Sprite SelfMarker = null;

        Texture OtherMarker = null;
        Sprite[] Rings = null;

        public void Setup(ResourceCache res)
        {
            ImageRect = new IntRect(48, 0, 64, 16);
            Border = new IntRect(4, 4, 4, 4);

            SelfMarker = new Sprite();
            SelfMarker.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            AddChild(SelfMarker);
            SelfMarker.SetSize(Width / 16, Width / 16);
            SelfMarker.SetHotSpot(SelfMarker.Width / 2, SelfMarker.Height / 2);
            SelfMarker.Texture = res.GetTexture2D("UI/HudMarker1.png");
            SelfMarker.BlendMode = BlendMode.Addalpha;
            SelfMarker.SetColor(new Color(Color.White, 0.5f));

            Rings = new Sprite[3];
            Rings[0] = new Sprite();
            Rings[0].SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            Rings[0].SetSize(Width - 4, Width - 4);
            Rings[0].SetHotSpot(Rings[0].Width / 2, Rings[0].Height / 2);
            Rings[0].SetPosition(0, 0);
            Rings[0].Texture = res.GetTexture2D("UI/Ring.png");
            Rings[0].SetColor(new Color(Color.White, 0.25f));
            Rings[0].BlendMode = BlendMode.Addalpha;
            AddChild(Rings[0]);

            Rings[1] = new Sprite();
            Rings[1].SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            Rings[1].SetSize(Width + 4, Width + 4);
            Rings[1].SetHotSpot(Rings[1].Width / 2, Rings[1].Height / 2);
            Rings[1].SetPosition(0, 0);
            Rings[1].Texture = res.GetTexture2D("UI/Ring.png");
            Rings[1].SetColor(new Color(Color.White, 0.25f));
            Rings[1].BlendMode = BlendMode.Addalpha;
            AddChild(Rings[1]);

            Rings[2] = new Sprite();
            Rings[2].SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            AddChild(Rings[2]);
            int inset = 0;
            Rings[2].SetSize(Width - inset, Width - inset);
            Rings[2].SetHotSpot(Rings[2].Width / 2, Rings[2].Height / 2);
            Rings[2].Texture = res.GetTexture2D("UI/Ring_Concentric.png");
            Rings[2].BlendMode = BlendMode.Addalpha;
            Rings[2].SetColor(new Color(Color.White, 0.5f));

            SelfMarker.BringToFront();

        }

        public void DoUpdate(float deltaTime, float elapsedTime)
        {
            float speed = 90;
            Rings[0].Rotation += deltaTime * speed;
            Rings[1].Rotation -= deltaTime * speed;

          //  Rings[2].Rotation -= deltaTime * 3;
        }
    }
}
