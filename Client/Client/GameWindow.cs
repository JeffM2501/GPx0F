using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Urho;
using Urho.Desktop;

namespace Client
{
    public partial class GameWindow : Form
    {
        private bool ForceDie = false;

        public GameWindow()
        {
            InitializeComponent();
            Config.Current = Config.Load();


            string dataPath = Config.Current.AssetFolder;

            if (dataPath == string.Empty || !Directory.Exists(dataPath))
                dataPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "assets");

            if (Directory.Exists(dataPath))
            {
                DesktopUrhoInitializer.AssetsDirectory = dataPath;

                GameSurface.Show(typeof(Game.App), new ApplicationOptions("data"));
            }
            else
            {
                MessageBox.Show(ClientResources.NoAssetPathError.Replace("$F", dataPath), ClientResources.NoAssetPathErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ForceDie = true;
            }
        }

        void SetupWindow()
        {
            switch(Config.Current.WinType)
            {
                case Config.WindowTypes.Window:
                    FormBorderStyle = FormBorderStyle.Sizable;
                    Rectangle tempRect = Config.Current.WindowBounds;
                    if (tempRect.X == int.MinValue || tempRect.Y == int.MinValue)
                    {
                        tempRect.X = DesktopBounds.X;
                        tempRect.Y = DesktopBounds.Y;
                    }
                    if (!RectIsVisible(tempRect))
                    {
                        tempRect.X = DesktopBounds.X;
                        tempRect.Y = DesktopBounds.Y;
                        if (!RectIsVisible(tempRect))
                            tempRect = DesktopBounds;
                    }

                    DesktopBounds = tempRect;

                    break;

                case Config.WindowTypes.FullScreen:
                    TopMost = true;
                    WindowState = FormWindowState.Normal;
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                    break;

                case Config.WindowTypes.FullScreenWindow:
                    TopMost = false;
                    FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    WindowState = FormWindowState.Maximized;
                    break;
            }
        }

        private bool PointIsVisible(Point p)
        {
            foreach (var s in Screen.AllScreens)
            {
                if (p.X < s.Bounds.Right && p.X > s.Bounds.Left && p.Y > s.Bounds.Top && p.Y < s.Bounds.Bottom)
                    return true;
            }
            return false;
        }

        private bool RectIsVisible(Rectangle f)
        {
            return PointIsVisible(new Point(f.Left, f.Top)) && PointIsVisible(new Point(f.Right, f.Top)) && PointIsVisible(new Point(f.Left, f.Bottom)) && PointIsVisible(new Point(f.Right, f.Bottom));
        }

        private void GameWindow_Load(object sender, EventArgs e)
        {
            if (ForceDie)
            {
                Close();
                return;
            }
            SetupWindow();
        }

        private void GameWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Config.Current.Save();
        }

        private void GameWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == FormWindowState.Maximized && Config.Current.WinType == Config.WindowTypes.Window)
                Config.Current.WinType = Config.WindowTypes.FullScreenWindow;
            else if (Config.Current.WinType == Config.WindowTypes.Window)
            {
                Config.Current.WindowBounds = Bounds;
            }
        }
    }
}
