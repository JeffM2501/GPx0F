using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Client
{
    public class Config
    {
        private static readonly int CurrentVersion = 1;

        public event EventHandler ApplyChanges = null;

        // version info
        public int Version = CurrentVersion;


        // window state
        public Rectangle WindowBounds = new Rectangle(int.MinValue, int.MinValue, 1280, 720);

        public enum WindowTypes
        {
            FullScreen,
            FullScreenWindow,
            Window
        }

        public WindowTypes WinType = WindowTypes.Window;

        // data directory
        public string AssetFolder = string.Empty;


        // rendering
        public int Multisample = 16;
        public bool LimitFPS = false;


        // audio
        public float MasterVolume = 1.0f;
        public float EffectsVolume = 1.0f;
        public float MusicVolume = 1.0f;


        // UI
        public int RadarSize = 300;



        public void Dirty()
        {
            ApplyChanges?.Invoke(this, EventArgs.Empty);
            Save();
        }

        public void Save()
        {
            FileInfo configFile = GetConfigFile();
            try
            {
                if (configFile.Exists)
                    configFile.Delete();

                var fs = configFile.OpenWrite();
                XML.Serialize(fs, this);
                fs.Close();
            }
            catch (Exception /*ex*/)
            {

            }
        }

        private static void Upgrade(Config cfg)
        {
            if (cfg.Version < CurrentVersion)
            {
                // cascade upgrades till we are at the current version
                if (cfg.Version > 0)
                {
                    // upgrade to v1.
                }
                // do other upgrades as needed



                // save at the end, so we don't do this every time.
                cfg.Save();
            }
        }

        public static Config Current = new Config();

        private static XmlSerializer XML = new XmlSerializer(typeof(Config));

        private static string GetConfigDir()
        {
            string thisPath = Assembly.GetExecutingAssembly().Location;
            FileVersionInfo thisInfo = FileVersionInfo.GetVersionInfo(thisPath);

            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Combine(path, thisInfo.CompanyName, thisInfo.ProductName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string GetVersionString()
        {
            string thisPath = Assembly.GetExecutingAssembly().Location;
            FileVersionInfo thisInfo = FileVersionInfo.GetVersionInfo(thisPath);


            return "v." + thisInfo.FileVersion + ":" + File.GetCreationTime(thisPath).ToString();
        }


        private static FileInfo GetConfigFile()
        {
            return new FileInfo(Path.Combine(GetConfigDir(), "client_config.xml"));
        }

        public static Config Load()
        {
            FileInfo configFile = GetConfigFile();
            if (configFile.Exists)
            {
                try
                {

                    var reader = configFile.OpenText();
                    Config cfg = XML.Deserialize(reader) as Config;
                    reader.Close();
                    if (cfg != null)
                    {
                        Upgrade(cfg);
                        return cfg;
                    }
                }
                catch (Exception /*ex*/)
                {

                }
            }
            return new Config();
        }

    }
}
