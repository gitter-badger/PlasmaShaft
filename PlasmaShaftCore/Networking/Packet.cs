using System;
using System.Net;
using System.Text;

namespace PlasmaShaftCore.Networking
{
    public class Packet
    {
        private byte[] _data;
        private int index;
        public byte[] Data { get { return _data; } }

        public Packet(int length)
        {
            _data = new byte[length];
            index = 0;
        }

        public void Write(string data)
        {
            if (index + 64 <= _data.Length && data.Length <= 64)
            {
                Buffer.BlockCopy(Encoding.ASCII.GetBytes(data.PadRight(64)), 0, _data, index, 64);
                index += 64;
                return;
            }
            throw new Exception("Packet too small to add string data");
        }

        public void Write(byte data)
        {
            if (index + 1 <= _data.Length)
            {
                _data[index] = data;
                index++;
                return;
            }
            throw new Exception("Packet full.");
        }

        public void Write(sbyte data)
        {
            if (index + 1 <= _data.Length)
            {
                unchecked { _data[index] = (byte)data; }
                index++;
                return;
            }
            throw new Exception("Packet full.");
        }

        public void Write(short data)
        {
            if (index + 2 <= _data.Length)
            {
                byte[] data_ = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
                Buffer.BlockCopy(data_, 0, _data, index, 2);
                index += 2;
                return;
            }
            throw new Exception("Packet too small to add short data");
        }

        public void Write(byte[] data)
        {
            if (index + 1024 <= _data.Length)
            {
                Buffer.BlockCopy(data, 0, _data, index, data.Length);
                index += 1024;
                return;
            }
            throw new Exception(string.Format("Packet too small to add byte[{0}] data", data.Length));
        }

        public void Write(OpCode OpCode)
        {
            Write((byte)OpCode);
        }

        public void WriteInt(int data)
        {
            if (index + 4 <= _data.Length)
            {
                byte[] data_ = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
                Buffer.BlockCopy(data_, 0, _data, index, 4);
                index += 4;
                return;
            }
            throw new Exception("Packet too small to add int data");
        }
    }
}
