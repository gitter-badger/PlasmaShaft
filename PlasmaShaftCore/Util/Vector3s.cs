using System;

namespace PlasmaShaftCore
{
    public class Vector3s
    {
        public short x, y, z;

        public Vector3s(short x, short y, short z)
        {
            this.x = x; this.y = y; this.z = z;
        }

        public Vector3s()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public static implicit operator short[](Vector3s b) {
            if (b == null) throw new NullReferenceException();
            return new short[3] { b.x, b.y, b.z };
        }
    }
}
