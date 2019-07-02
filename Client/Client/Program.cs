using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

using Urho;
using Urho.Desktop;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Config.Current = Config.Load();

            string dataPath = Config.Current.AssetFolder;

            if (dataPath == string.Empty || !Directory.Exists(dataPath))
                dataPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "assets");

            if (!Directory.Exists(dataPath))
            {
                MessageBox.Show(ClientResources.NoAssetPathError.Replace("$F", dataPath), ClientResources.NoAssetPathErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Directory.Exists(dataPath))
            {
                DesktopUrhoInitializer.AssetsDirectory = dataPath;

                ApplicationOptions options = new ApplicationOptions("Data");
                options.Orientation = ApplicationOptions.OrientationType.Landscape;
                options.ResizableWindow = false;
                options.WindowedMode = Config.Current.WinType != Config.WindowTypes.FullScreen;
                if (Config.Current.WinType == Config.WindowTypes.Window)
                {
                    options.Width = Config.Current.WindowBounds.Width;
                    options.Height = Config.Current.WindowBounds.Height;
                }
                options.Multisampling = Config.Current.Multisample;
                options.LimitFps = Config.Current.LimitFPS;

                try
                {
                    Game.App app = new Game.App(options);
                    var exitCode = app.Run();
                }
                catch(Exception /*ex*/)
                {
                    
                }
            }
        }


        private static bool PointIsVisible(Point p)
        {
            foreach (var s in Screen.AllScreens)
            {
                if (p.X < s.Bounds.Right && p.X > s.Bounds.Left && p.Y > s.Bounds.Top && p.Y < s.Bounds.Bottom)
                    return true;
            }
            return false;
        }

        private static bool RectIsVisible(Rectangle f)
        {
            return PointIsVisible(new Point(f.Left, f.Top)) && PointIsVisible(new Point(f.Right, f.Top)) && PointIsVisible(new Point(f.Left, f.Bottom)) && PointIsVisible(new Point(f.Right, f.Bottom));
        }
    }
}
