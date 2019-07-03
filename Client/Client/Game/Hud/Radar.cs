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
    public class Radar : Window
    {
        Sprite SelfMarker = null;

        Texture OtherMarker = null;
        Sprite Ring = null;

        public void Setup(ResourceCache res)
        {
            SelfMarker = new Sprite();
            AddChild(SelfMarker);
            SelfMarker.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            SelfMarker.SetPivot(0.5f, 0.5f);
            SelfMarker.SetSize(Width / 10, Width / 10);
            SelfMarker.Position = new IntVector2(-SelfMarker.Size.X / 2, -SelfMarker.Size.X / 2);
            SelfMarker.Texture = res.GetTexture2D("UI/HudMarker1.png");
            SelfMarker.SetColor(new Color(Color.White, 0.75f));

            Ring = new Sprite();
            AddChild(Ring);
            Ring.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            Ring.SetSize(Width, Width);          
            Ring.SetPosition(0, 0);
         //   Ring.Position = new IntVector2(-Ring.Size.X/2, -Ring.Size.X/2);
          
            Ring.Texture = res.GetTexture2D("UI/Ring.png");
          //  

        }

        public void DoUpdate(float deltaTime, float elapsedTime)
        {
            Ring.Rotation += deltaTime * 12;
        }
    }
}
