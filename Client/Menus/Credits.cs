using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Gui;

namespace Client.Menus
{
    public class Credits : Stack.MenuScreen
    {
        public override void Init()
        {
            base.Init();

            AddHeaderString(ClientResources.Credits, 64);
            AddVersionMarker();

            int xOffset = 20;
            int yOffset = -60;

            Window frame = new Window();
            RootElement.AddChild(frame);
            frame.SetStyleAuto(null);
            frame.SetColor(new Color(Color.Black, 0.75f));
            frame.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            frame.SetPosition(0, -20);
            frame.SetSize(RootElement.Width - (xOffset* 2), RootElement.Height - (275));

            int border = 5;
            var credits = new Text();
            frame.AddChild(credits);
            credits.Value = ClientResources.CreditText;
            credits.HorizontalAlignment = HorizontalAlignment.Left;
            credits.VerticalAlignment = VerticalAlignment.Top;
            credits.SetStyleAuto(null);
            credits.SetFont(Resources.GetFont("Fonts/Exo2-RegularCondensed.otf"), 14);
            credits.SetPosition(border, border);
            credits.SetSize(frame.Width - (border *2), frame.Height - (border * 2));
            credits.SetColor(Color.White);
            credits.SetMaxAnchor(1, 1);
            credits.SetMinAnchor(0, 0);

            var back = CreateButton(xOffset, yOffset, 250, 50, ClientResources.Back, HorizontalAlignment.Left, VerticalAlignment.Bottom);
            back.Pressed += Back_Pressed;

        }

        private void Back_Pressed(PressedEventArgs obj)
        {
            Stack.Pop();
        }
    }
}
