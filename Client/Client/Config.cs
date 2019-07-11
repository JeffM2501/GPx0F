using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using Urho;

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

        public enum ShadowQualities
        {
            Low,
            Medium,
            High,
        }

        public ShadowQualities ShadowQuality = ShadowQualities.Medium;


        // audio
        public float MasterVolume = 1.0f;
        public float EffectsVolume = 1.0f;
        public float MusicVolume = 1.0f;


        // UI
        public int RadarSize = 300;


        // input

        public enum AxisFunctions
        {
            None,
            Turning,
            Acceleration,
            Aiming,
            SideSlide,
        }

        public enum ButtonFunctions
        {
            None,
            Jump,
            PrimaryFire,
            SecondaryFire,
            Drop,
            Accessory,
            Spawn,
            StartChat,
            SendChat,
            Menu,
            MenuBack,
            MenuAccept,
            Boost,
            ToggleReverseThrottle,
            HoldReverseThrottle,
        }

        public AxisFunctions MouseXAxisFunciton = AxisFunctions.Turning;
        public AxisFunctions MouseYAxisFunciton = AxisFunctions.Aiming;
        public AxisFunctions MouseZAxisFunciton = AxisFunctions.None;

        public float MouseXSensitivity = 100.0f;
        public float MouseYSensitivity = 1.0f;
        public float MouseZSensitivity = 1.0f;

        public class MouseButtonMapItem
        {
            public ButtonFunctions Function = ButtonFunctions.None;
            public MouseButton Button = MouseButton.Left;
        }

        public MouseButtonMapItem[] MouseButtonFunctions = new MouseButtonMapItem[]
        {
            new MouseButtonMapItem() { Function = ButtonFunctions.PrimaryFire, Button = MouseButton.Left },
            new MouseButtonMapItem() { Function = ButtonFunctions.SecondaryFire, Button = MouseButton.Right },
            new MouseButtonMapItem() { Function = ButtonFunctions.Drop, Button = MouseButton.Middle },
            new MouseButtonMapItem() { Function = ButtonFunctions.Spawn, Button = MouseButton.Right },
        };

        public class AxisKeyset
        {
            public Key PositiveKey = Key.End;
            public Key NegativeKey = Key.End;
            public Key HalfSpeedKey = Key.Application;

            public static readonly AxisKeyset Empty = new AxisKeyset();
        }

        public class KeyboardAxisMapItem
        {
            public AxisFunctions Function = AxisFunctions.None;
            public AxisKeyset Keys = null;
        }

        public KeyboardAxisMapItem[] KeyboardAxisFunctions = new KeyboardAxisMapItem[]
        {
            new KeyboardAxisMapItem(){Function = AxisFunctions.Acceleration, Keys = new AxisKeyset(){PositiveKey = Key.W, NegativeKey = Key.S }},
            new KeyboardAxisMapItem(){Function = AxisFunctions.SideSlide, Keys = new AxisKeyset(){PositiveKey = Key.D, NegativeKey = Key.A }},
            new KeyboardAxisMapItem(){Function = AxisFunctions.Aiming, Keys = new AxisKeyset(){PositiveKey = Key.Up, NegativeKey = Key.Down }},
            new KeyboardAxisMapItem(){Function = AxisFunctions.Turning, Keys = new AxisKeyset(){PositiveKey = Key.Right, NegativeKey = Key.Left }}
        };

        public class KeyboardButtonMapItem
        {
            public ButtonFunctions Function = ButtonFunctions.None;
            public Key ButtonKey = Key.End;
        }

        public KeyboardButtonMapItem[] KeyboardButtonFunctions = new KeyboardButtonMapItem[]
        {
            new KeyboardButtonMapItem(){Function = ButtonFunctions.Jump, ButtonKey = Key.Tab },
            new KeyboardButtonMapItem(){Function = ButtonFunctions.PrimaryFire, ButtonKey = Key.Space},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.SecondaryFire,ButtonKey = Key.Ctrl},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.Accessory, ButtonKey = Key.Shift},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.Drop, ButtonKey = Key.Q},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.Spawn, ButtonKey = Key.Space},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.Menu, ButtonKey = Key.Esc},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.MenuBack, ButtonKey = Key.Esc},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.MenuBack, ButtonKey = Key.Delete},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.MenuAccept, ButtonKey = Key.Return},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.MenuAccept, ButtonKey = Key.Return2},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.MenuAccept, ButtonKey = Key.Kp_ENTER},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.StartChat, ButtonKey = Key.Return},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.SendChat, ButtonKey = Key.Return},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.Boost, ButtonKey = Key.Shift},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.Boost, ButtonKey = Key.LeftShift},
            new KeyboardButtonMapItem(){Function = ButtonFunctions.Boost, ButtonKey = Key.RightShift}
        };


        public class JoystickControlInfo
        {
            public AxisFunctions Function = AxisFunctions.None;

            public string DeviceName = string.Empty;
            public int ControlIndex = -1;

            public static readonly JoystickControlInfo Empty = new JoystickControlInfo();
        }

        public class JoystickButtonInfo
        {
            public ButtonFunctions Function = ButtonFunctions.None;

            public string DeviceName = string.Empty;
            public bool IsHat = false;
            public int ControlIndex = -1;
            public byte ControlFactor = 1;

            public static readonly JoystickButtonInfo Empty = new JoystickButtonInfo();
        }

        public JoystickControlInfo[] JoystickAxisFunctions = new JoystickControlInfo[0];
        public JoystickButtonInfo[] JoystickButtonFunctions = new JoystickButtonInfo[0];


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
            catch (Exception ex)
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
                var reader = configFile.OpenText();
                try
                {
                    Config cfg = XML.Deserialize(reader) as Config;
                   
                    if (cfg != null)
                    {
                        Upgrade(cfg);
                        return cfg;
                    }
                }
                catch (Exception ex)
                {

                }

                reader.Close();
            }
            return new Config();
        }

    }
}
