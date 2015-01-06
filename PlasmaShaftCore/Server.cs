using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using PlasmaShaftCore.GUI;
using PlasmaShaftCore.Networking;
using PlasmaShaftCore.World;

namespace PlasmaShaftCore
{
    public static class Server {
        public delegate void LogMsg(string message, LogMessage MSG);

        public static bool DebugMode { get; private set; }
        public static bool GUIMode { get; private set; }
        public static List<Level> levels = new List<Level>();
        private static ClientListener listener;
        private static Process thisProcess = Process.GetCurrentProcess();
        public static event LogMsg OnLog = null;
        
        #region SETUP

        public static void Start(bool DEBUG, bool GUI)
        {
            DebugMode = DEBUG;
            GUIMode = GUI;
            if (!GUI) {
                Init();
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window());
        }

        public static void Init()
        {
            Log("Starting server...", LogMessage.INFO);
            InitialiseListener();
            if (!listener.Run())
            {
                if (!GUIMode)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Server.Log("Server shut down, press any key to continue...", LogMessage.ERROR);
                    Console.ReadKey();
                }
                else Server.Log("Server shut down.");
                return;
            }
            InitialisePositionUpdater();
            CreateDirectories();
            LoadMainLevel();

            if (!GUIMode) Console.ReadKey();
        }

        private static void InitialisePositionUpdater() {
            System.Timers.Timer posTimer = new System.Timers.Timer(100);
            posTimer.Elapsed += delegate { Player.UpdatePosition(); };
            posTimer.Start();
        }

        private static void InitialiseListener() {
            listener = ClientListener.Create(Port);
            listener.OnConnection += AcceptPlayerConnection;
        }

        private static void CreateDirectories() {
            if (!Directory.Exists("properties")) Directory.CreateDirectory("properties");
            if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
            Server.Log("Set up directories!");
        }

        private static void LoadMainLevel() {
            if (!File.Exists("levels/main.cw")) {
                MainLevel = new Level("main", 128, 128, 128);
                MainLevel.Save();
                Server.Log("Main level not found, creating new one!");
            }
            MainLevel = Level.Load("main", LevelFormat.ClassicWorld);
            Server.Log("Loaded main level!");
            levels.Add(MainLevel);
        }

        private static void InitialiseEvents() {

        }

        public static string GetMemoryUsage()  {
            return (int)(thisProcess.WorkingSet64/1024/1024) + " MB";
        }

        private static void AcceptPlayerConnection(TcpClient client) {
            Player player = new Player(client);
        }

        #endregion


        public static void Log(string message, LogMessage type = LogMessage.MESSAGE) {
            if (OnLog == null) {
                if (type == LogMessage.FIRSTCHANCE && !DebugMode)
                    return;
                Console.WriteLine("[{0}]: {1}", type.ToString(), message);
                return;
            }
            OnLog(message, type);
        }

        #region == PROPERTIES ==

        public static string Name = "XCraft 1.0";
        public static string MOTD = "Crafting all the way";
        public static int MaxClients = 20;
        public static int Port = 25566;

        public static Level MainLevel;

        #endregion
    }
}
