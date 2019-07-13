using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Desktop;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataPath = string.Empty; // TODO, parse this from optional command line

            if (dataPath == string.Empty || !Directory.Exists(dataPath))
                dataPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "assets");

            if (!Directory.Exists(dataPath))
            {
                Console.WriteLine("Unable to locate data path " + dataPath);
                return;
            }

            if (Directory.Exists(dataPath))
            {
                DesktopUrhoInitializer.AssetsDirectory = dataPath;

                ApplicationOptions options = new ApplicationOptions("Data");
                options.Orientation = ApplicationOptions.OrientationType.Landscape;
                options.ResizableWindow = false;
                options.WindowedMode = true;
                options.AdditionalFlags = "-headless";
              

                int exitCode = 0;
                App app = null;
                try
                {
                    app = new App(options);
                    exitCode = app.Run();
                }
                catch (Exception ex)
                {
                    if (app == null || !app.IsExiting)
                        Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
