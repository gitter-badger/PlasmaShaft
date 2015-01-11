using System;
using System.Diagnostics;
using PlasmaShaft;
using System.Windows.Forms;
using PlasmaShaft.GUI;

namespace PlasmaShaft
{
    public class Program
    {
        public static void Main(string[] args)
        {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Window());
            if (args.Length > 0)
            {
                bool debug = false;
                bool gui = false;
                try
                {
                    debug = bool.Parse(args[0]);
                    gui = bool.Parse(args[1]);
                }
                catch
                {
                    Console.WriteLine("Could not parse the arguments.");
                    Console.WriteLine("Make sure it is as followed: \"ServerGUI.exe [bool:debug] [bool:gui]\"");
                    Environment.Exit(0);
                }
                Server.Start(debug, gui);
            }
            else
            {
#if DEBUG
                Server.Start(true, true);
#else
                Server.Start(false, true);
#endif
            }
        }
    }
}
