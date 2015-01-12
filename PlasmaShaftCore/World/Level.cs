using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using PlasmaShaft.Networking;

namespace PlasmaShaft {
    public class Level : IDisposable  {
        public List<Player> players {
            get {
                return Server.Players.FindAll(p => p.level.Name == this.Name);
            }
        }
        public BlockDB BlockDB { get; private set; }

        public Vector3s PlayerSpawn {
            get {
                return new Vector3s((short)(Spawn[0] * 32), (short)(Spawn[1] * 32), (short)(Spawn[2] * 32));
            }
        }

        public byte FreeID {
            get {
                for (byte i = 0; i < 64; i++)
                    if (!players.Any(p => p.ID == i))
                        return i;
                    else continue;
                unchecked { return (byte)-1; }
            }
        }

        public byte[] BlockData;
        public short width, depth, height;
        public short[] Spawn = new short[3];
        public byte[] SpawnRot = new byte[2];
        public string Name;
        public string Author = "PlasmaShaft";
        public byte DefaultClickDistance = 160;
        public long TimeCreated = 0;
        public long LastAccessed = 0;
        public long LastModified = 0;
        public byte FormatVersion { get; private set; }
        public byte[] UUID { get; private set; }
        public List<Blockchange> BlockQueue_ = new List<Blockchange>();
        public int PerBuild = 50; // -51 and lower means nobody can build
        public int PerVisit = -50;

        public void Dispose() {
            BlockData = null;
            Spawn = null;
            SpawnRot = null;
            UUID = null;
            BlockQueue_.Clear();
        }

        public Level(string name, short x, short y, short z, byte generation = 0, bool Save = false) {
            Name = name;
            width = x;
            depth = y;
            height = z;
            BlockData = new byte[width * depth * height];
            Spawn[0] = (short)(x / 2);
            Spawn[1] = (short)(y * 0.75);
            Spawn[2] = (short)(z / 2);
            SpawnRot = new byte[2] { 0, 0 };
            TimeCreated = GetCurrentUnixTime();
            LastAccessed = GetCurrentUnixTime();
            LastModified = GetCurrentUnixTime();
            BlockDB = new BlockDB(this);
            UUID = new byte[16];
            var random = RandomNumberGenerator.Create();
            random.GetBytes(UUID);

            for (int xx = 0; xx < width; xx++)
                for (int zz = 0; zz < height; zz++)
                        SetTile((short)xx, (short)(depth/2-1), (short)zz, (byte)2);
			for (int xx = 0; xx < width; xx++)
				for (int zz = 0; zz < height; zz++)
					for (int yy = 0; yy < ((depth / 2) - 1); yy++)
						SetTile((short)xx, (short)yy, (short)zz, (byte)3);
        }

        public bool InBounds(Vector3s vec)
        {
            return vec.x < width && vec.y < depth && vec.z < height && vec.x >= 0 && vec.y >= 0 && vec.z >= 0;
        }

        public void SetTile(short x, short y, short z, byte type) {
            if (x < 0 || y < 0 || z < 0 || x >= width || y >= height || z >= depth)
                return;
            BlockData[x + (z * width) + (y * width * height)] = type;
        }

        public byte GetTile(short x, short y, short z) {
            if (x < 0 || y < 0 || z < 0 || x >= width || y >= height || z >= depth)
                return 0xff;
            return BlockData[x + (z * width) + (y * width * height)];
        }

        public int Index(int x, int y, int z)
        {
            return x + (z * width) + (y * width * height);
        }


        public void PlayerBlockchange(Player p, short x, short y, short z, byte type, short mode = 0) {
            byte oldType = GetTile(x, y, z);
            byte tmpType = type;
            if (mode == 0)
                type = 0;
            BlockDBEntry newEntry = new BlockDBEntry((int)DateTime.UtcNow.ToUnixTime(),
                              p.ID,
                              new Vector3s(p.Pos[0], p.Pos[1], p.Pos[2]),
                              GetTile(x, y, z),
                              type,
                              BlockChangeContext.Manual);
            SetTile(x, y, z, type);
            p.level.BlockDB.AddEntry(newEntry);
            Server.Players.ForEach(pl => { if (pl.level == this) pl.SendBlockchange(x, y, z, type); });
        }

        public void Save() {
            if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
            var compound = new NbtCompound("ClassicWorld") {
                new NbtByte("FormatVersion", 1),
                new NbtString("Name", Name),
                new NbtByteArray("UUID", UUID),
                new NbtShort("X", width),
                new NbtShort("Y", depth),
                new NbtShort("Z", height),
                new NbtCompound("CreatedBy") {
                    new NbtString("Service", "ClassiCube"),
                    new NbtString("Username", Author)
                },
                new NbtCompound("MapGenerator") {
                    new NbtString("Software", "PlasmaShaft"),
                    new NbtString("MapGeneratorName", "PlasmaShaft")
                },
                new NbtLong("TimeCreated", TimeCreated),
                new NbtLong("LastAccessed", LastAccessed),
                new NbtLong("LastModified", LastModified),
                new NbtCompound("Spawn") {
                    new NbtShort("X", Spawn[0]),
                    new NbtShort("Y", Spawn[1]),
                    new NbtShort("Z", Spawn[2]),
                    new NbtByte("H", SpawnRot[0]),
                    new NbtByte("P", SpawnRot[1])
                },
                new NbtByteArray("BlockArray", BlockData),
                new NbtCompound("Metadata") {
                    /*new NbtCompound("ClickDistance") {
                        new NbtInt("ExtensionVersion", 1),
                        new NbtShort("Distance", DefaultClickDistance)
                    }*/
                    new NbtCompound("Permission") {
                        new NbtInt("PerBuild", PerBuild),
                        new NbtInt("PerVisit", PerVisit)
                    }
                }
            };

            if (File.Exists(string.Format("levels/{0}.cw", Name))) File.Delete(string.Format("levels/{0}.cw", Name));
            var MyFile = new NbtFile(compound);
            MyFile.SaveToFile("levels/" + Name + ".cw", NbtCompression.GZip);
            BlockDB.Flush(false, true);
        }

        public void Export(LevelFormat format) {

        }

        public static Level LoadMap(string FileName, LevelFormat format = LevelFormat.ClassicWorld)
        {
            return Load(FileName, format);
        }

        public Level LoadMap(LevelFormat format = LevelFormat.ClassicWorld)
        {
            return Load(this.Name, format);
        }

        public static Level Load(string FileName, LevelFormat format) {
            Level level = null;

            if (FileName == null) throw new ArgumentNullException(" 'FileName' ");

            switch (format) {
                case LevelFormat.ClassicWorld:
                    if (!File.Exists("levels/" + FileName + ".cw"))
                        return null;
                    var NbtLvl = new NbtFile("levels/" + FileName + ".cw");
                    NbtCompound _basetag = NbtLvl.RootTag;
                    if (_basetag.Name != "ClassicWorld")
                        throw new FormatException(FileName + " is not a ClassicWorld file!");
                    var createdBy = _basetag.Get<NbtCompound>("CreatedBy");
                    //var MapGen = _basetag.Get<NbtCompound>("MapGenerator");
                    
                    level = new Level(String.Empty, 0, 0, 0) {
                        FormatVersion = _basetag["FormatVersion"].ByteValue,
                        Name = _basetag["Name"].StringValue,
                        UUID = _basetag["UUID"].ByteArrayValue,
                        width = _basetag["X"].ShortValue,
                        depth = _basetag["Y"].ShortValue,
                        height = _basetag["Z"].ShortValue,
                    };

                    if (createdBy != null)
                        level.Author = createdBy["Username"].StringValue;
                    var spawnpoint = _basetag.Get<NbtCompound>("Spawn");
                    if (spawnpoint == null)
                        throw new FormatException("Spawn not found.");
                    level.Spawn[0] = spawnpoint["X"].ShortValue;
                    level.Spawn[1] = spawnpoint["Y"].ShortValue;
                    level.Spawn[2] = spawnpoint["Z"].ShortValue;
                    level.SpawnRot[0] = spawnpoint["H"].ByteValue;
                    level.SpawnRot[1] = spawnpoint["P"].ByteValue;

                    var Permission = _basetag.Get<NbtCompound>("Permission");
                    if(Permission != null) {
                        level.PerBuild = Permission["PerBuild"].IntValue;
                        level.PerVisit = Permission["PerVisit"].IntValue;
                    }
                    level.BlockData = _basetag["BlockArray"].ByteArrayValue;

                    if (level.BlockData == null)
                        throw new FormatException("BlockArray not found.");

                    if (level.width == 0 || level.depth == 0 || level.height == 0)
                        throw new FormatException("Map size not found.");

                    if (level.FormatVersion == 0 || level.Name == null || level.UUID == null)
                        throw new FormatException("Map header information not found.");

                    if (level.LastAccessed != 0)
                        level.LastAccessed = GetCurrentUnixTime();
                    _basetag = null;
                    break;
                case LevelFormat.Dat:
                    if (!File.Exists("levels/" + FileName + ".dat"))
                        throw new Exception(FileName + ".dat does not exist!");
                    using (FileStream mapStream = File.OpenRead(FileName + ".dat")) {
                        byte[] temp = new byte[8];
                        mapStream.Seek(-4, SeekOrigin.End);
                        mapStream.Read(temp, 0, 4);
                        mapStream.Seek(0, SeekOrigin.Begin);
                        int uncompressedLength = BitConverter.ToInt32(temp, 0);
                        byte[] data = new byte[uncompressedLength];
                        using (GZipStream reader = new GZipStream(mapStream, CompressionMode.Decompress, true))  {
                            reader.Read(data, 0, uncompressedLength);
                        }

                        for (int i = 0; i < uncompressedLength - 1; i++) {
                            if (data[i] != 0xAC || data[i + 1] != 0xED) continue;
                            int pointer = i + 6;
                            Array.Copy(data, pointer, temp, 0, 2);
                            pointer += IPAddress.HostToNetworkOrder(BitConverter.ToInt16(temp, 0));
                            pointer += 13;
                            int headerEnd;
                            for (headerEnd = pointer; headerEnd < data.Length - 1; headerEnd++) {
                                if (data[headerEnd] == 0x78 && data[headerEnd + 1] == 0x70)  {
                                    headerEnd += 2;
                                    break;
                                }
                            }


                            int offset = 0;
                            short width = 0, length = 0, height = 0;
                            short[] spawn = new short[3];
                            while (pointer < headerEnd) {
                                switch ((char)data[pointer]) {
                                    case 'Z':
                                        offset++;
                                        break;
                                    case 'F':
                                    case 'I':
                                        offset += 4;
                                        break;
                                    case 'J':
                                        offset += 8;
                                        break;
                                }

                                pointer += 1;
                                Array.Copy(data, pointer, temp, 0, 2);
                                short skip = IPAddress.HostToNetworkOrder(BitConverter.ToInt16(temp, 0));
                                Array.Copy(data, headerEnd + offset - 4, temp, 0, 4);
                                Array.Copy(data, headerEnd + offset - 4, temp, 0, 4);
                                if (MemCmp(data, pointer, "width")) {
                                    width = (short)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(temp, 0));
                                }
                                else if (MemCmp(data, pointer, "depth")) {
                                    height = (short)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(temp, 0));
                                }
                                else if (MemCmp(data, pointer, "height")) {
                                    length = (short)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(temp, 0));
                                }
                                else if (MemCmp(data, pointer, "xSpawn")) {
                                    spawn[0] = (short)(IPAddress.HostToNetworkOrder(BitConverter.ToInt32(temp, 0)) * 32 + 16);
                                }
                                else if (MemCmp(data, pointer, "ySpawn")) {
                                    spawn[1] = (short)(IPAddress.HostToNetworkOrder(BitConverter.ToInt32(temp, 0)) * 32 + 16);
                                }
                                else if (MemCmp(data, pointer, "zSpawn")) {
                                    spawn[2] = (short)(IPAddress.HostToNetworkOrder(BitConverter.ToInt32(temp, 0)) * 32 + 16);
                                }
                                pointer += skip;
                            }

                            level = new Level(FileName, width, height, length);
                            level.SpawnRot[0] = 0;
                            level.SpawnRot[1] = 0;
                            level.Spawn[0] = spawn[0];
                            level.Spawn[1] = spawn[2];
                            level.Spawn[2] = spawn[1];
                            

                            bool foundBlockArray = false;
                            offset = Array.IndexOf<byte>(data, 0x00, headerEnd);
                            while (offset != -1 && data[offset + 1] == 0x78 && data[offset + 2] == 0x70) {
                                foundBlockArray = true;
                                pointer += offset + 7;
                            }
                            offset = Array.IndexOf<byte>(data, 0x00, offset + 1);

                            if (foundBlockArray) {
                                Array.Copy(data, pointer, level.BlockData, 0, level.BlockData.Length);
                            }
                            else {
                                throw new Exception("DatLoader: Could not find block array.");
                            }

                            break;
                        }

                        if (level == null) {
                            throw new Exception("DatLoader: Failed to create the map, since no data was found.");
                        }
                        else {
                            Server.Log(String.Format("Level '{0}' was loaded.", level.Name));
                        }
                    }
                    break;
                case LevelFormat.MCLawlLvl:
                case LevelFormat.MCForgeLvl:
                    string ext = (format == LevelFormat.MCForgeLvl ? ".mcf" : ".lvl");
                    if (!File.Exists("levels/" + FileName + ext))
                        throw new Exception(FileName + ext + " does not exist!");
                    using (FileStream fs = File.OpenRead("levels/" + FileName + ext)) {
                        using (GZipStream gs = new GZipStream(fs, CompressionMode.Decompress)) {
                            byte[] ver = new byte[2];
                            gs.Read(ver, 0, ver.Length);
                            short version = BitConverter.ToInt16(ver, 0);
                            if (version == 1874) {
                                byte[] header = new byte[16]; gs.Read(header, 0, header.Length);
                                short width = BitConverter.ToInt16(header, 0);
                                short height = BitConverter.ToInt16(header, 4);
                                short depth = BitConverter.ToInt16(header, 2);
                                level = new Level(FileName, width, height, depth);
                                level.Spawn[0] = BitConverter.ToInt16(header, 6);
                                level.Spawn[1] = BitConverter.ToInt16(header, 10);
                                level.Spawn[2] = BitConverter.ToInt16(header, 8);
                                level.SpawnRot[0] = header[13]; level.SpawnRot[1] = header[13];
                            }
                            else {
                                byte[] header = new byte[12]; gs.Read(header, 0, header.Length);
                                short width = version;
                                short height = BitConverter.ToInt16(header, 2);
                                short depth = BitConverter.ToInt16(header, 0);
                                level = new Level(FileName, width, height, depth);
                                level.Spawn[0] = BitConverter.ToInt16(header, 4);
                                level.Spawn[1] = BitConverter.ToInt16(header, 6);
                                level.Spawn[2] = BitConverter.ToInt16(header, 8);
                                level.SpawnRot[0] = header[10]; level.SpawnRot[1] = header[11];
                            }
                            byte[] blocks;
                            if (format == LevelFormat.MCLawlLvl) {
                                blocks = new byte[level.width * level.height * level.depth];
                                gs.Read(blocks, 0, blocks.Length);
                                for (int i = 0; i < blocks.Length; i++)
                                    if (blocks[i] >= 66)
                                        blocks[i] = 0;
                                level.BlockData = blocks;
                                gs.Close();
                            }
                            else {
                                blocks = new byte[2 * level.width * level.depth * level.height];
                                gs.Read(blocks, 0, blocks.Length);
                                for (int i = 0; i < (blocks.Length / 2); ++i)
                                    level.BlockData[i] = (byte)BitConverter.ToInt16(new byte[] { blocks[i * 2], blocks[(i * 2) + 1] }, 0);
                                for (int i = 0; i < level.BlockData.Length; i++)
                                    if (level.BlockData[i] >= 66)
                                        level.BlockData[i] = 0;
                                gs.Close();
                            }
                            if (level == null) {
                                throw new Exception("MCLoader: Failed to create the map, since no data was found.");
                            }
                            else {
                                Server.Log(String.Format("Level '{0}' was loaded.", FileName));
                            }
                            blocks = null;
                        }
                    }
                    break;
            }
            level.BlockDB = new BlockDB(level);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return level;
        }

        private static readonly DateTime UnixEpoch =
            new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );

        private static long GetCurrentUnixTime() {
            var timeSinceEpoch = (DateTime.UtcNow - UnixEpoch);
            return (long)timeSinceEpoch.TotalSeconds;
        }

        public static bool MemCmp(byte[] data, int offset, string value) {
            if (data == null) throw new ArgumentNullException("data");
            if (value == null) throw new ArgumentNullException("value");
            if (offset < 0 || offset > data.Length) throw new ArgumentOutOfRangeException("offset");
            for (int i = 0; i < value.Length; i++)  {
                if (offset + i >= data.Length || data[offset + i] != value[i]) return false;
            }
            return true;
        }

        public static Level Find(string name) {
            List<Level> tempList = new List<Level>();
            tempList.AddRange(Server.levels);
            Level tempLevel = null; bool returnNull = false;

            foreach (Level l in tempList) {
                if (l.Name.ToLower() == name.ToLower()) return l;
                if (l.Name.ToLower().IndexOf(name.ToLower()) != -1) {
                    if (tempLevel == null) tempLevel = l;
                    else returnNull = true;
                }
            }

            if (returnNull == true) return null;
            if (tempLevel != null) return tempLevel;

            return null;
        }

        public static Level FindExact(string name) {
            List<Level> templist = new List<Level>(Server.levels);
            foreach (Level l in templist) {
                if (l.Name.ToLower() == name.ToLower())
                    return l;
            }
            return null;
        }

        public void Unload() {
            try {
                lock (Server.levels)
                    Server.levels.Remove(this);
            }
            catch { }
            Save();
            Server.Log("Level '" + Name + "' was unloaded.");
            Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}