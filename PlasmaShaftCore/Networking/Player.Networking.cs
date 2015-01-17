using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using PlasmaShaft.Networking;
using PlasmaShaft.World;

namespace PlasmaShaft
{
    public sealed partial class Player
    {
        private TcpClient client;
        private byte[] TempData = new byte[0xFF];
        private byte[] PartialData = new byte[0];
        private byte[] OldPos = new byte[3];
        private byte[] OldRot = new byte[3];
        private byte[] SpawnPos = new byte[3];
        private Packet Packet;
        public bool Disconnected { get; private set; }
        public override byte ID { get; set; }

        private System.Timers.Timer PingTimer = new System.Timers.Timer(2000);

        public Level level;

        public bool LoggedIn = false;

        public string IP  {
            get {
                if (client != null)
                    return client.Client.RemoteEndPoint.ToString().Split(':')[0];
                else return string.Empty;
            }
        }

        public NetworkStream NetworkStream {
            get {
                return client.GetStream();
            }
        }

        public Player(TcpClient client) {
            Pos = new short[3];
            Rot = new byte[2];
            Disconnected = false;
            this.client = client;
            Server.Log("&2" + IP + " connected to the server.");

            NetworkStream.BeginRead(TempData, 0, TempData.Length, new AsyncCallback(Read), this);
            PingTimer.Elapsed += delegate { SendPing(); };
            PingTimer.Start();
        }

        private static void Read(IAsyncResult result) {
            Player p = (Player)result.AsyncState;
            try {
                if (p == null) {
                    return;
                }
                else {
                    int read = p.NetworkStream.EndRead(result);
                    if (read == 0) {
                        p.Disconnect();
                        return;
                    }

                    byte[] FullPacket = new byte[p.PartialData.Length + read];
                    Buffer.BlockCopy(p.PartialData, 0, FullPacket, 0, p.PartialData.Length);
                    Buffer.BlockCopy(p.TempData, 0, FullPacket, p.PartialData.Length, read);

                    p.PartialData = p.ProcessData(FullPacket);
                    p.NetworkStream.BeginRead(p.TempData, 0, p.TempData.Length, new AsyncCallback(Read), p);
                }
            }
            catch (IOException) {
                p.Disconnect();
            }
            catch (ObjectDisposedException) {
                p.Disconnected = true;
            }
            catch {
                p.SendKick("An error occurred!");
            }
        }

        private byte[] ProcessData( byte[] data ) {
            int msgID = data[0], length = 0;
            switch (msgID)  {
                case 0x00: length = 130; break;
                case 0x05: length = 8; break;
                case 0x08: length = 9; break;
                case 0x0D: length = 65; break;
                case 0x10: length = 66; break;
                case 0x11: length = 68; break;
                case 0x13: length = 1; break;
                default: break;
            }

            byte[] tmp = new byte[length];
            byte[] tmp2 = new byte[data.Length - length - 1];
            Buffer.BlockCopy(data, 1, tmp, 0, length);
            Buffer.BlockCopy(data, length + 1, tmp2, 0, data.Length - length - 1);

            switch (msgID) {
                    
                case 0x00: ProcessLogin(tmp); break;
                case 0x05: ProcessBlockchange(tmp); break;
                case 0x08: ProcessMovement(tmp); break;
                case 0x0D: ProcessMessage(tmp); break;
                case 0x10: length = 66; break;
                case 0x11: length = 68; break;
                case 0x13: length = 1; break;
            }

            return tmp2;
        }
		private bool VerifyName( string name, string hash, string salt ) 
		{
			if( name == null ) throw new ArgumentNullException( "name" );
			if( hash == null ) throw new ArgumentNullException( "hash" );
			if( salt == null ) throw new ArgumentNullException( "salt" );
			while( hash.Length < 32 ) {
				hash = "0" + hash;
			}
			MD5 hasher = MD5.Create();
			StringBuilder sb = new StringBuilder( 32 );
			foreach( byte b in hasher.ComputeHash( Encoding.ASCII.GetBytes( salt + name ) ) ) 
			{
				sb.AppendFormat( "{0:x2}", b );
			}
			return sb.ToString().TrimStart('0').Equals( hash, StringComparison.OrdinalIgnoreCase );
		}
        private void ProcessLogin(byte[] msg) {
            if (Server.Players.Count >= Server.MaxClients)
                SendKick("The server is full, try again later");
            byte protocolVersion = msg[0];
            string Username = Encoding.ASCII.GetString(msg, 1, 64).Trim();
            string VerificationKey = Encoding.ASCII.GetString(msg, 65, 64).Trim();
            byte clientType = msg[129];
			if (Server.VerifyNames) {
				if (!VerifyName (Username, VerificationKey, Server.Salt))
					SendKick ("Verification failed, try re-logging?");
			}
            this.Name = Username;

            this.ID = Server.MainLevel.FreeID;
            this.level = Server.MainLevel;

            SendID(Server.Name, Server.MOTD, 0x00);
            SendToCurrentLevel();
            if (!Server.PlayersSinceStartUp.Contains(this))
            {
                Server.PlayersSinceStartUp.Add(this);
            }
            Server.Players.Add(this);
            try
            {
                PlayerDB.Load(this);
            }
            catch(Exception ex)
            {
                SendKick("Error loading PlayerDB");
                Server.Log(ex.ToString());
            }
            SpawnPlayersInLevel(true, true);
            Server.Log("&2" + Username + " joined the server.");
			Server.Say("&2" + Username + " joined the server.");
        }

        /// <summary>
        /// Handles when a player changes a block to update the change to all players
        /// </summary>
        private void ProcessBlockchange(byte[] msg) {
            short x = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 0));
            short y = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 2));
            short z = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 4));
            byte mode = msg[6];
            byte type = msg[7];

            if (mode > 1) {
                SendKick("Unknown block action!");
                return;
            }

            if (type > 66) {
                SendKick("Unknown block type!");
                return;
            }

            if (mode == 0)
                BlocksDeleted++;
            else
                BlocksBuilt++;
            level.PlayerBlockchange(this, x, y, z, type, mode);
        }

        private void ProcessMovement(byte[] msg) {
            byte PlayerID = msg[0];
            short x = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 1));
            short y = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 3));
            short z = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(msg, 5));
            byte Yaw = msg[7];
            byte Pitch = msg[8];
            this.Pos[0] = x;
            this.Pos[1] = y;
            this.Pos[2] = z;
            this.Rot[0] = Yaw;
            this.Rot[1] = Pitch;
        }

        private void ProcessMessage(byte[] msg) {
            byte Unused = msg[0];
            string message = Encoding.ASCII.GetString(msg, 1, 64).Trim();
            if (message[0] == '/')
            {
                message = message.Remove(0, 1);

                string[] args = message.Split(' ');
                HandleCommand(args);
                return;
            }
            Say(message);
        }

        private void ProcessExtEntry(byte[] msg) {

        }

        private void ProcessExtInfo(byte[] msg) {

        }

        public void SendID(string ServerName, string MOTD, byte UserType) {
            if (UserType != 0x00 || UserType != 0x64)
                UserType = 0x00;
            Packet = new Packet(131);
            Packet.Write(OpCode.ServerIdentification);
            Packet.Write(0x07);
            Packet.Write(ServerName);
            Packet.Write(MOTD);
            Packet.Write(UserType);
            Send(Packet);
        }

        public void SendPing() {
            Packet = new Packet(1);
            Packet.Write(OpCode.Ping);
            Send(Packet);
        }

        public void SendLevelInitialize() {
            Packet = new Packet(1);
            Packet.Write(OpCode.LevelInitialize);
            Send(Packet);
        }

        public void SendToCurrentLevel() {
            SendLevelInitialize();

            byte[] TempData = new byte[level.BlockData.Length + 4];
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(level.BlockData.Length)).CopyTo(TempData, 0);
            Buffer.BlockCopy(level.BlockData, 0, TempData, 4, level.BlockData.Length);
            TempData = TempData.Compress();
            byte[] tmp;
            byte[] tmp2;
            int loops = (short)(Math.Ceiling(((double)(TempData.Length) / 1024)));

            for (int i = 1; TempData.Length > i; i++)
            {
                short length = (short)Math.Min(TempData.Length, 1024);
                tmp = new byte[length];
                tmp2 = new byte[TempData.Length - length];
                Buffer.BlockCopy(TempData, 0, tmp, 0, length);
                Buffer.BlockCopy(TempData, length, tmp2, 0, TempData.Length - length);
                TempData = tmp2;
                byte percentComplete = (byte)((i * 100 / loops));
                SendLevelDataChunk(length, tmp, percentComplete);
            }
            SendLevelFinalize(level.width, level.depth, level.height);
        }

        public void SendToLevel(Level level) {
            SendToCurrentLevel();
        }

        public void SendLevelDataChunk(short ChunkLength, byte[] ChunkData, byte PercentComplete) {
            Packet = new Packet(4 + 1024);
            Packet.Write(OpCode.LevelDataChunk);
            Packet.Write(ChunkLength);
            Packet.Write(ChunkData);
            Packet.Write(PercentComplete);
            Send(Packet);
        }

        public void SendLevelFinalize(short X, short Y, short Z) {
            Packet = new Packet(7);
            Packet.Write(OpCode.LevelFinalize);
            Packet.Write(X);
            Packet.Write(Y);
            Packet.Write(Z);
            Send(Packet);
        }

        public void SendBlockchange(short x, short y, short z, byte block) {
            Packet = new Packet(8);
            Packet.Write(OpCode.Blockchange);
            Packet.Write(x);
            Packet.Write(y);
            Packet.Write(z);
            Packet.Write(block);
            Send(Packet);
        }

        public void SpawnEntity(Entity e, string name = "", short X = 0, short Y = 0, short Z = 0, byte Yaw = 0, byte Pitch = 0) {
            if (name == "")
                name = e.Name;
            if (X == 0)
                X = e.Pos[0];
            if (Y == 0)
                Y = e.Pos[1];
            if (Z == 0)
                Z = e.Pos[2];
            if (Yaw == 0)
                Yaw = e.Rot[0];
            if (Pitch == 0)
                Pitch = e.Rot[1];
            Packet = new Packet(74);
            Packet.Write(OpCode.SpawnPlayer);
            Packet.Write(e.ID);
            Packet.Write(name);
            Packet.Write(X);
            Packet.Write(Y);
            Packet.Write(Z);
            Packet.Write(Yaw);
            Packet.Write(Pitch);
            Send(Packet);
        }

        public void SendDespawn(Entity e) {
            Packet = new Packet(2);
            Packet.Write(OpCode.DespawnPlayer);
            Packet.Write(e.ID);
            Send(Packet);
        }

        /// <summary>
        /// Sends a message with the desired message type id
        /// </summary>
        public void SendMessage(byte id, string message) {
            MessagesWritten++;
            foreach (string msg in Wordwrap(message))
            {
                Packet = new Packet(66);
                Packet.Write(OpCode.Message);
                Packet.Write(id);
                Packet.Write(msg);
                Send(Packet);
            }
        }

        /// <summary>
        /// Sends a message with an id of 0, normal message type
        /// </summary>
        public void SendMessage(string message)
        {
            MessagesWritten++;
            foreach (string msg in Wordwrap(message))
            {
                Packet = new Packet(66);
                Packet.Write(OpCode.Message);
                Packet.Write((byte)0);
                Packet.Write(msg);
                Send(Packet);
            }
        }

        public void SendKick(string Message) {
            Packet = new Packet(65);
            Packet.Write(OpCode.Disconnect);
            Packet.Write(Message);
            Send(Packet);
        }

        public void Send(byte[] data)  {
            try  {
                if (!Disconnected) this.NetworkStream.BeginWrite(data, 0, data.Length, delegate(IAsyncResult result) { }, null);
            }
            catch {
                Disconnect();
            }
        }

        public void Send(Packet packet) {
            Send(packet.Data);
        }

        public void Disconnect() {
            Quit("Disconnected.", DisconnectReason.Disconnected);
        }

        public void Quit(string reason, DisconnectReason dcreason = DisconnectReason.Quit) {
            if (!Disconnected) {
                client.Close();
                Disconnected = true;
                Server.Log("&4" + Name + " " + dcreason.ToString() + " (" + reason + ")");
                DespawnPlayersInLevel(true, true);
                Server.Players.Remove(this);
            }
        }

        static List<string> Wordwrap(string message)
        {
            List<string> lines = new List<string>();
            message = Regex.Replace(message, @"(&[0-9a-f])+(&[0-9a-f])", "$2");
            message = Regex.Replace(message, @"(&[0-9a-f])+$", "");

            int limit = 64; string color = "";
            while (message.Length > 0)
            {
                //if (Regex.IsMatch(message, "&a")) break;

                if (lines.Count > 0)
                {
                    if (message[0].ToString() == "&")
                        message = "> " + message.Trim();
                    else
                        message = "> " + color + message.Trim();
                }

                if (message.IndexOf("&") == message.IndexOf("&", message.IndexOf("&") + 1) - 2)
                    message = message.Remove(message.IndexOf("&"), 2);

                if (message.Length <= limit) { lines.Add(message); break; }
                for (int i = limit - 1; i > limit - 20; --i)
                    if (message[i] == ' ')
                    {
                        lines.Add(message.Substring(0, i));
                        goto Next;
                    }

            retry:
                if (message.Length == 0 || limit == 0) { return lines; }

                try
                {
                    if (message.Substring(limit - 2, 1) == "&" || message.Substring(limit - 1, 1) == "&")
                    {
                        message = message.Remove(limit - 2, 1);
                        limit -= 2;
                        goto retry;
                    }
                    else if (message[limit - 1] < 32 || message[limit - 1] > 127)
                    {
                        message = message.Remove(limit - 1, 1);
                        limit -= 1;
                        //goto retry;
                    }
                }
                catch { return lines; }
                lines.Add(message.Substring(0, limit));

            Next: message = message.Substring(lines[lines.Count - 1].Length);
                if (lines.Count == 1) limit = 60;

                int index = lines[lines.Count - 1].LastIndexOf('&');
                if (index != -1)
                {
                    if (index < lines[lines.Count - 1].Length - 1)
                    {
                        char next = lines[lines.Count - 1][index + 1];
                        if ("0123456789abcdef".IndexOf(next) != -1) { color = "&" + next; }
                        if (index == lines[lines.Count - 1].Length - 1)
                        {
                            lines[lines.Count - 1] = lines[lines.Count - 1].Substring(0, lines[lines.Count - 1].Length - 2);
                        }
                    }
                    else if (message.Length != 0)
                    {
                        char next = message[0];
                        if ("0123456789abcdef".IndexOf(next) != -1)
                        {
                            color = "&" + next;
                        }
                        lines[lines.Count - 1] = lines[lines.Count - 1].Substring(0, lines[lines.Count - 1].Length - 1);
                        message = message.Substring(1);
                    }
                }
            }
            char[] temp;
            for (int i = 0; i < lines.Count; i++) // Gotta do it the old fashioned way...
            {
                temp = lines[i].ToCharArray();
                if (temp[temp.Length - 2] == '%' || temp[temp.Length - 2] == '&')
                {
                    temp[temp.Length - 1] = ' ';
                    temp[temp.Length - 2] = ' ';
                }
                StringBuilder message1 = new StringBuilder();
                message1.Append(temp);
                lines[i] = message1.ToString();
            }
            return lines;
        }

        private void UpdatePos()
        {
            if (Pos == null || Rot == null)
                return;
            byte changed = 0;

            if (Pos[0] != OldPos[0] || Pos[1] != OldPos[1] || Pos[2] != OldPos[2])
                changed += 1;
            if (Rot[0] != OldRot[0] || Rot[1] != OldRot[1])
                changed += 2;
            if (Math.Abs(OldPos[0] - Pos[0]) > 32 ||
                Math.Abs(OldPos[1] - Pos[1]) > 32 ||
                Math.Abs(OldPos[2] - Pos[2]) > 32)
                changed = 4;
            if (Pos[0] == SpawnPos[0] && Pos[1] == SpawnPos[1] && Pos[2] == SpawnPos[2])
                changed = 4;
            Packet packet = null;
            if (changed == 4) //Speed hacks or teleporting
            {
                packet = new Packet(10);
                packet.Write(0x08);
                packet.Write(ID);
                packet.Write(Pos[0]);
                packet.Write(Pos[1]);
                packet.Write(Pos[2]);
                packet.Write(Rot[0]);
                packet.Write(Rot[1]);
            }
            else if (changed == 3)
            {
                packet = new Packet(7);
                packet.Write(0x09);
                packet.Write(ID);
                packet.Write((sbyte)(Pos[0] - OldPos[0]));
                packet.Write((sbyte)(Pos[1] - OldPos[1]));
                packet.Write((sbyte)(Pos[2] - OldPos[2]));
                packet.Write(Rot[0]);
                packet.Write(Rot[1]);
            }
            else if (changed == 2)
            {
                packet = new Packet(4);
                packet.Write(0x0b);
                packet.Write(ID);
                packet.Write(Rot[0]);
                packet.Write(Rot[1]);
            }
            else if (changed == 1)
            {
                packet = new Packet(5);
                packet.Write(0x0a);
                packet.Write(ID);
                packet.Write((sbyte)(Pos[0] - OldPos[0]));
                packet.Write((sbyte)(Pos[1] - OldPos[1]));
                packet.Write((sbyte)(Pos[2] - OldPos[2]));
            }

            if (changed != 0)
            {
                Server.Players.ForEach(pl =>
                {
                    if (pl != this && pl.level == level)
                        pl.Send(packet);
                });
            }
        }
    }
}