using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;
using Urho.Resources;

namespace Client.Menus
{
    public static class Stack
    {
        private static Application RootApp = null;

        public static void Setup(Application app)
        {
            RootApp = app;
            XmlFile style = RootApp.ResourceCache.GetXmlFile("UI/DefaultStyle.xml");
            RootApp.UI.Root.SetDefaultStyle(style);
        }

        private static List<MenuScreen> MenuStack = new List<MenuScreen>();

        private static MenuScreen Top()
        {
            if (MenuStack.Count == 0)
                return null;

            return MenuStack[MenuStack.Count - 1];
        }

        public static void Push(MenuScreen screen)
        {
            MenuScreen top = Top();
            if (top != null)
            {
                top.Deactivate();
                top.RootElement.Visible = false;
            }

            screen.Resources = RootApp.ResourceCache;
            screen.RootElement = new Urho.Gui.UIElement();
            screen.RootElement.Size = new IntVector2(RootApp.UI.Root.Size.X, RootApp.UI.Root.Size.Y);
            RootApp.UI.Root.AddChild(screen.RootElement);

            screen.RootElement.EnableAnchor = true;
            screen.RootElement.SetMaxAnchor(1, 1);
            screen.RootElement.SetMinAnchor(0, 0);

            MenuStack.Add(screen);
            screen.Init();
            screen.Activate();
        }

        public static void Pop()
        {
            MenuScreen top = Top();
            if (top != null)
            {
                top.Deactivate();
                top.RootElement.Visible = false;

                top.RootElement.Remove();
                top.RootElement = null;

                MenuStack.RemoveAt(MenuStack.Count - 1);
            }

            top = Top();
            if (top != null)
            {
                top.RootElement.Visible = true;
                top.Activate();
            }
        }

        public static void ClearAll()
        {
            MenuStack.Reverse();
            foreach (var item in MenuStack)
            {
                item.Deactivate();
                item.RootElement.Remove();
            }
            MenuStack.Clear();
        }

        public class MenuScreen
        {
            public Urho.Gui.UIElement RootElement = null;
            public ResourceCache Resources = null;

            public virtual string Name { get; } = string.Empty;

            public virtual void Init()
            {

            }

            public virtual void Activate()
            {

            }

            public virtual void Deactivate()
            {

            }

            protected Text CreateLabel(int x, int y, string text, HorizontalAlignment hAlign = HorizontalAlignment.Left, VerticalAlignment vAlign = VerticalAlignment.Top, int size = 18, UIElement root = null)
            {
                Font font = Resources.GetFont("Fonts/Exo2-Regular.otf");

                if (root == null)
                    root = RootElement;

                Text label = new Text();
                root.AddChild(label);
                label.SetAlignment(hAlign, vAlign);
                label.SetPosition(x, y);
                label.SetFont(font, size);
                label.Value = text;

                return label;
            }

            protected Button CreateButton(int x, int y, int xSize, int ySize, string text, HorizontalAlignment hAlign = HorizontalAlignment.Left, VerticalAlignment vAlign = VerticalAlignment.Top, UIElement root = null)
            {
                Font font = Resources.GetFont("Fonts/Exo2-Medium.otf");
                if (root == null)
                    root = RootElement;

                // Create the button and center the text onto it
                Button button = new Button();
                root.AddChild(button);
                button.SetStyleAuto(null);
                button.SetAlignment(hAlign, vAlign);
                button.SetPosition(x, y);
                button.SetSize(xSize, ySize);

                Text buttonText = new Text();
                button.AddChild(buttonText);
                buttonText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
                buttonText.SetPosition(0, -ySize / 10);
                buttonText.SetFont(font, ySize/2);
                buttonText.Value = text;

                return button;
            }

            protected List<Button> CreateButtonColumnn(int x, int y, int xSize, int ySize, int spacing, string[] texts, HorizontalAlignment hAlign = HorizontalAlignment.Left, VerticalAlignment vAlign = VerticalAlignment.Top, UIElement root = null)
            {
                List<Button> buttons = new List<Button>();
                foreach (var text in texts)
                {
                    var button = CreateButton(x, y, xSize, ySize, text, hAlign, vAlign, root);
                    button.Name = text;
                    buttons.Add(button);
                    y += ySize + spacing;
                }
                return buttons;
            }

            protected Slider CreateSlider(int x, int y, int xSize, int ySize, string text, UIElement root = null)
            {
                if (root == null)
                    root = RootElement;

                Font font = Resources.GetFont("Fonts/Exo2-Medium.otf");
                // Create text and slider below it
                Text sliderText = new Text();
                root.AddChild(sliderText);
                sliderText.SetPosition(x, y);
                sliderText.SetFont(font, ySize - 5 );
                sliderText.Value = text;

                Slider slider = new Slider();
                root.AddChild(slider);
                slider.SetStyleAuto(null);
                slider.SetPosition(x, y + 20);
                slider.SetSize(xSize, ySize/2);
                // Use 0-1 range for controlling sound/music master volume
                slider.Range = 1.0f;

                return slider;
            }

            protected void AddVersionMarker()
            {
                var versionText = new Text();
                versionText.Value = Config.GetVersionString();
                versionText.HorizontalAlignment = HorizontalAlignment.Right;
                versionText.VerticalAlignment = VerticalAlignment.Bottom;
                versionText.SetFont(Resources.GetFont("Fonts/Exo2-Medium.otf"), 14);
                versionText.Position = new IntVector2(-20, -20);
                versionText.SetColor(Color.White);
                versionText.SetMaxAnchor(1, 1);
                versionText.SetMinAnchor(1, 1);
                RootElement.AddChild(versionText);
            }

            protected Text AddHeaderString(string text, int size = -1)
            {
                return AddHeaderString(text, Color.FromHex("485872"), size);
            }

            protected Text AddHeaderString(string text, Color color, int size = -1)
            {
                if (size < 0)
                    size = 120;

                var titleText = new Text();
                titleText.Value = text;
                titleText.HorizontalAlignment = HorizontalAlignment.Left;
                titleText.VerticalAlignment = VerticalAlignment.Top;
                titleText.SetFont(Resources.GetFont("Fonts/Exo2-Black.otf"), size);
                titleText.Position = new IntVector2(20, 20);
                titleText.SetColor(color);
                titleText.SetMaxAnchor(0, 0);
                titleText.SetMinAnchor(0, 0);
                RootElement.AddChild(titleText);

                return titleText;
            }
        }
    }
}
