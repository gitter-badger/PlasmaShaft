// Copyright 2009-2012 Matvei Stefarov <me@matvei.org>
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace PlasmaShaft
{

    /// <summary>
    /// Defines a 3D bounding box, in integer cartesian coordinates
    /// </summary>
    public sealed class BoundingBox : IEquatable<BoundingBox>
    {
        public static readonly BoundingBox Empty = new BoundingBox(0, 0, 0, 0, 0, 0);

        // ReSharper disable FieldCanBeMadeReadOnly.Global
        public int XMin, YMin, ZMin, XMax, YMax, ZMax;
        // ReSharper restore FieldCanBeMadeReadOnly.Global

        /// <summary> Constructs a bounding box using two vectors as opposite corners. </summary>
        public BoundingBox(Vector3s p1, Vector3s p2) :
            this(p1.x, p1.y, p1.z, p2.x, p2.y, p2.z)
        {
        }


        /// <summary> Constructs a bounding box at a given origin, with given dimensions. </summary>
        /// <param name="origin"> Origin point of the bounding box. </param>
        /// <param name="width"> Width (X-axis, horizontal). May be negative. </param>
        /// <param name="length"> Length (Y-axis, horizontal). May be negative. </param>
        /// <param name="height"> Width (Z-axis, vertical). May be negative. </param>
        public BoundingBox(Vector3s origin, int width, int length, int height) :
            this(origin.x, origin.y, origin.z,
                origin.x + width - 1,
                origin.y + length - 1,
                origin.z + height - 1)
        {
        }


        /// <summary> Constructs a bounding box between two given coordinates. </summary>
        public BoundingBox(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            XMin = Math.Min(x1, x2);
            XMax = Math.Max(x1, x2);
            YMin = Math.Min(y1, y2);
            YMax = Math.Max(y1, y2);
            ZMin = Math.Min(z1, z2);
            ZMax = Math.Max(z1, z2);
        }


        #region Collision Detection

        /// <summary> Checks whether this bounding box intersects/touches another one. </summary>
        public bool Insersects(BoundingBox other)
        {
            if (other == null) throw new ArgumentNullException("other");
            return !(XMax < other.XMin || XMin > other.XMax ||
                     YMax < other.YMin || YMin > other.YMax ||
                     ZMax < other.ZMin || ZMin > other.ZMax);
        }


        /// <summary> Checks if another bounding box is wholly contained inside this one. </summary>
        public bool Contains(BoundingBox other)
        {
            if (other == null) throw new ArgumentNullException("other");
            return XMin <= other.XMin && XMax >= other.XMax &&
                   YMin <= other.YMin && YMax >= other.YMax &&
                   ZMin <= other.ZMin && ZMax >= other.ZMax;
        }


        /// <summary> Checks if a given point is inside this bounding box. </summary>
        public bool Contains(int x, int y, int z)
        {
            return x >= XMin && x <= XMax &&
                   y >= YMin && y <= YMax &&
                   z >= ZMin && z <= ZMax;
        }


        /// <summary> Checks if a given point is inside this bounding box. </summary>
        public bool Contains(Vector3s point)
        {
            return point.x >= XMin && point.x <= XMax &&
                   point.y >= YMin && point.y <= YMax &&
                   point.z >= ZMin && point.z <= ZMax;
        }


        /// <summary> Returns a BoundingBox object that describes the space shared between this and another box. </summary>
        /// <returns> Intersecting volume, or BoundingBox.Empty if there is no overlap. </returns>
        public BoundingBox GetIntersection(BoundingBox other)
        {
            if (other == null) throw new ArgumentNullException("other");
            if (Contains(other))
            {
                return other;
            }
            else if (other.Contains(this))
            {
                return this;
            }
            else if (Insersects(other))
            {
                return new BoundingBox(Math.Max(XMin, other.XMin),
                                        Math.Max(YMin, other.YMin),
                                        Math.Max(ZMin, other.ZMin),
                                        Math.Min(XMax, other.XMax),
                                        Math.Min(YMax, other.YMax),
                                        Math.Min(ZMax, other.ZMax));
            }
            else
            {
                return Empty;
            }
        }

        #endregion


        public int Volume
        {
            get { return (XMax - XMin + 1) * (YMax - YMin + 1) * (ZMax - ZMin + 1); }
        }


        public Vector3s Dimensions
        {
            get
            {
                return new Vector3s(XMax - XMin + 1,
                                     YMax - YMin + 1,
                                     ZMax - ZMin + 1);
            }
        }


        public int Width
        {
            get { return (XMax - XMin + 1); }
        }

        public int Length
        {
            get { return (YMax - YMin + 1); }
        }

        public int Height
        {
            get { return (ZMax - ZMin + 1); }
        }


        /// <summary> Returns the vertex closest to the coordinate origin, opposite MaxVertex. </summary>
        public Vector3s MinVertex
        {
            get { return new Vector3s(XMin, YMin, ZMin); }
        }


        /// <summary> Returns the vertex farthest from the origin, opposite MinVertex. </summary>
        public Vector3s MaxVertex
        {
            get { return new Vector3s(XMax, YMax, ZMax); }
        }


        #region Serialization

        public const string XmlRootElementName = "BoundingBox";

        public BoundingBox( XElement root)
        {
            if (root == null) throw new ArgumentNullException("root");
            string[] coords = root.Value.Split(' ');
            int x1 = Int32.Parse(coords[0]);
            int x2 = Int32.Parse(coords[1]);
            int y1 = Int32.Parse(coords[2]);
            int y2 = Int32.Parse(coords[3]);
            int z1 = Int32.Parse(coords[4]);
            int z2 = Int32.Parse(coords[5]);
            XMin = Math.Min(x1, x2);
            XMax = Math.Max(x1, x2);
            YMin = Math.Min(y1, y2);
            YMax = Math.Max(y1, y2);
            ZMin = Math.Min(z1, z2);
            ZMax = Math.Max(z1, z2);
        }

        public XElement Serialize( string tagName)
        {
            if (tagName == null) throw new ArgumentNullException("tagName");
            string data = String.Format("{0} {1} {2} {3} {4} {5}",
                                         XMin, XMax, YMin, YMax, ZMin, ZMax);
            return new XElement(tagName, data);
        }

        public XElement Serialize()
        {
            return Serialize(XmlRootElementName);
        }

        #endregion


        public bool Equals(BoundingBox other)
        {
            return XMin == other.XMin && XMax == other.XMax &&
                   YMin == other.YMin && YMax == other.YMax &&
                   ZMin == other.ZMin && ZMax == other.ZMax;
        }

        public override string ToString()
        {
            return "BoundingBox" + Dimensions;
        }
    }
}
