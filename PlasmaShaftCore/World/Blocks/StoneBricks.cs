﻿namespace PlasmaShaftCore
{
    public class StoneBricks : Block
    {
        public override byte ID
        {
            get { return 65; }
        }

        public override byte Fallback
        {
            get { return 65; }
        }

        public override bool CPE
        {
            get { return true; }
        }

        public override bool Flammable
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "stonebricks"; }
        }

        public override bool Opaque
        {
            get { return true; }
        }

        public override int Permission
        {
            get;
            set;
        }

        public override float Resistance
        {
            get { return 600f; }
        }

        public override bool WaterKill
        {
            get { return false; }
        }

        public override bool Walkthrough
        {
            get { return false; }
        }

        public override short TouchingRadius
        {
            get { return 1; }
        }
    }
}
