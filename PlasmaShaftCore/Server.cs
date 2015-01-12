using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PlasmaShaft.Networking;
using PlasmaShaft.World;

namespace PlasmaShaft
{
    public static class Server {
        public delegate void LogMsg(string message, LogMessage MSG);

        public static bool DebugMode { get; private set; }
        public static bool GUIMode { get; private set; }
        public static List<Level> levels = new List<Level>();
        private static ClientListener listener;
        private static Process thisProcess = Process.GetCurrentProcess();
        public static event LogMsg OnLog = null;
		public static double LastHeartbeatTook { get; set; }
		private static bool Initialized = false;
        
        #region SETUP

        public static void Start(bool DEBUG, bool GUI)
        {
            DebugMode = DEBUG;
            GUIMode = GUI;
            if (!GUI) {
                Init();
                return;
            }
        }

        public static void Init()
        {
            Log("Starting server...", LogMessage.INFO);
            LoadConfig();
            InitialiseListener();
            BlockDB.Init();
            LoadAllDlls.Init();
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
			Salt = GetRandomString(32);
			Thread T2 = new Thread(TimerThread, 1048576);
			T2.Name = "Heartbeat thread";
			T2.Start();
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

		public static void Say(string message, byte id = 0) {
				Player.players.ForEach (p => p.SendMessage (id, message));
		}

        public static void Log(string message, LogMessage type = LogMessage.MESSAGE) {
            if (OnLog == null) {
                if (type == LogMessage.FIRSTCHANCE && !DebugMode)
                    return;
                Console.WriteLine("[{0}]: {1}", type.ToString(), message);
                return;
            }
            OnLog(message, type);
        }

		public static string GetRandomString( int chars ) {
			RandomNumberGenerator prng = RandomNumberGenerator.Create();
			StringBuilder sb = new StringBuilder();
			byte[] oneChar = new byte[1];
			while( sb.Length < chars ) {
				prng.GetBytes( oneChar );
				if( oneChar[0] >= 48 && oneChar[0] <= 57 ||
				   oneChar[0] >= 65 && oneChar[0] <= 90 ||
				   oneChar[0] >= 97 && oneChar[0] <= 122 ) {
					//if( oneChar[0] >= 33 && oneChar[0] <= 126 ) {
					sb.Append( (char)oneChar[0] );
				}
			}
			return sb.ToString();
		}

        #region == PROPERTIES ==

		public static string Name = "PlasmaShaft [Default]";
        public static string MOTD = "+hax";
        public static int MaxClients = 20;
        public static int Port = 25565;
		public static string Salt { get; set; }
		public static bool Public = true;
		public static bool VerifyNames = false;
        public static void LoadConfig()
        {
            Config config = new Config("config");
                config.LoadConfig("config");
                Name = config.GetValue("server-name");
                Port = Convert.ToInt32(config.GetValue("server-port"));
                if (Port == 0)
                    Port = 25565;
                MOTD = config.GetValue("server-motd");
                Public = Convert.ToBoolean(config.GetValue("server-public"));
                MaxClients = Convert.ToInt32(config.GetValue("max-clients"));
                if (MaxClients == 0)
                    MaxClients = 20;
                VerifyNames = Convert.ToBoolean(config.GetValue("verify-names"));
                if (Name == null || MOTD == null || Public == null || VerifyNames == null)
                {
                    if (Name == null)
                        Name = "PlasmaShaft [Default]";
                    if (MOTD == null)
                        MOTD = "+hax";
                    if (Public == false)
                        Public = true;
                    if (VerifyNames == false)
                        VerifyNames = true;
                    config.SetValue("server-name", Name.ToString());
                    config.SetValue("server-port", Port.ToString());
                    config.SetValue("server-motd", MOTD.ToString());
                    config.SetValue("server-public", Public.ToString());
                    config.SetValue("max-clients", MaxClients.ToString());
                    config.SetValue("verify-names", VerifyNames.ToString());
                    config.SaveConfig("config");
                }
        }
        public static Level MainLevel;

        #endregion

		#region == HEARTBEAT ==

		private static void TimerThread()
		{
			Stopwatch clock = new Stopwatch();
			clock.Start();
			double lastHeartbeat = -30;
			while (true) {
				if (clock.Elapsed.TotalSeconds - lastHeartbeat >= 30) {
					double now = clock.Elapsed.TotalSeconds;
					Heartbeat ();
                    foreach (Level lvls in levels)
                        lvls.Save();
					GC.Collect ();
					lastHeartbeat = clock.Elapsed.TotalSeconds;
					LastHeartbeatTook = Math.Round (10 * (clock.Elapsed.TotalSeconds - now)) / 10.0;
				}
			}
		}

		private static void Heartbeat()
		{
			try {

				StringBuilder builder = new StringBuilder();

				builder.Append("port=");
				builder.Append(Port.ToString());

				builder.Append("&users=");
				builder.Append(Player.players.Count);

				builder.Append("&max=");
				builder.Append(MaxClients);

				builder.Append("&name=");
				builder.Append(Name);

				builder.Append("&public=");
				builder.Append(Server.Public.ToString());

				builder.Append("&version=7");

				builder.Append("&salt=");
				builder.Append(Salt);

				builder.Append("&software=PlasmaShaft");
				string postcontent = builder.ToString();
				byte[] post = Encoding.ASCII.GetBytes(postcontent);

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.classicube.net/heartbeat.jsp");
				req.ContentType = "application/x-www-form-urlencoded";
				req.Method = "POST";
				req.ContentLength = post.Length;
				Stream o = req.GetRequestStream();
				o.Write(post, 0, post.Length);
				o.Close();

				WebResponse resp = req.GetResponse();
				StreamReader sr = new StreamReader(resp.GetResponseStream());
				string data = sr.ReadToEnd().Trim();

				if (!Initialized)
				{
					if (!data.Contains("://") && !data.Contains("www")) {
						Log("Heartbeat successful, but no URL returned!", LogMessage.ERROR);
					} else {
						int i = data.IndexOf('=');
						Log("URL found: ");
						Log(data);

						Initialized = true;
					}
				}
			}
			catch(WebException e) {
				Log("Unable to heartbeat " + e.ToString(), LogMessage.ERROR);
			}
		}
		#endregion
	}
}