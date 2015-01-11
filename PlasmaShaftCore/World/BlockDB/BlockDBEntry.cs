// Copyright 2009-2013 Matvei Stefarov <me@matvei.org>
using System.IO;
using System.Runtime.InteropServices;

namespace PlasmaShaft
{
    /// <summary> Struct representing a single block change.
    /// You may safely cast byte* pointers directly to BlockDBEntry* and vice versa. </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BlockDBEntry
    {
        /// <summary> UTC Unix timestamp of the change. </summary>
        public readonly int Timestamp;

        /// <summary> Numeric PlayerDB id of the player who made the change. </summary>
        public readonly int PlayerID;

        /// <summary> X coordinate (horizontal), in terms of blocks. </summary>
        public readonly short X;

        /// <summary> Y coordinate (horizontal), in terms of blocks. </summary>
        public readonly short Y;

        /// <summary> Z coordinate (vertical), in terms of blocks. </summary>
        public readonly short Z;

        /// <summary> Block that previously occupied this coordinate </summary>
        public readonly byte OldBlock;

        /// <summary> Block that now occupies this coordinate </summary>
        public readonly byte NewBlock;

        /// <summary> Context for this block change. </summary>
        public readonly BlockChangeContext Context;


        public BlockDBEntry(int timestamp, int playerID, short x, short y, short z,
                             byte oldBlock, byte newBlock, BlockChangeContext flags)
        {
            Timestamp = timestamp;
            PlayerID = playerID;
            X = x;
            Y = y;
            Z = z;
            OldBlock = oldBlock;
            NewBlock = newBlock;
            Context = flags;
        }

        public BlockDBEntry(int timestamp, int playerID, Vector3s coords,
                             byte oldBlock, byte newBlock, BlockChangeContext flags)
        {
            Timestamp = timestamp;
            PlayerID = playerID;
            X = (short)coords.x;
            Y = (short)coords.y;
            Z = (short)coords.z;
            OldBlock = oldBlock;
            NewBlock = newBlock;
            Context = flags;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Timestamp);
            writer.Write(PlayerID);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
            writer.Write(OldBlock);
            writer.Write(NewBlock);
            writer.Write((int)Context);
        }
    }
}