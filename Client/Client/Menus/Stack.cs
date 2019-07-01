using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;

namespace Client.Menus
{
    public static class Stack
    {
        private static Application RootApp = null;

        public static void Setup(Application app)
        {
            RootApp = app;
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
        }
    }
}
