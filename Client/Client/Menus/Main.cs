using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;

namespace Client.Menus
{
    public class Main : Menus.Stack.MenuScreen
    {
        public event EventHandler StartGame = null;
        public event EventHandler Quit = null;

        public override void Init()
        {
            base.Init();

            var LogoText = new Text();
 			LogoText.Value = "GPx0F";
            LogoText.HorizontalAlignment = HorizontalAlignment.Left;
 			LogoText.VerticalAlignment = VerticalAlignment.Top;
            LogoText.SetFont(Resources.GetFont("Fonts/Exo2-Black.otf"), 120);
 			LogoText.Position = new IntVector2(20, 20);
 			LogoText.SetColor(Color.FromHex("485872"));
            LogoText.SetMaxAnchor(0, 0);
            LogoText.SetMinAnchor(0, 0);
            RootElement.AddChild(LogoText);


            var copyrightText = new Text();
            copyrightText.Value = ClientResources.CopyrightText;
            copyrightText.HorizontalAlignment = HorizontalAlignment.Left;
            copyrightText.VerticalAlignment = VerticalAlignment.Bottom;
            copyrightText.SetFont(Resources.GetFont("Fonts/Exo2-ExtraLight.otf"), 16);
            copyrightText.Position = new IntVector2(20, -20);
            copyrightText.SetColor(Color.White);
            copyrightText.SetMaxAnchor(0, 1);
            copyrightText.SetMinAnchor(0, 1);
            RootElement.AddChild(copyrightText);
        }
    }
}
