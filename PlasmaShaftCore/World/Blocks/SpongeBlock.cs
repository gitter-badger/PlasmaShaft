﻿namespace PlasmaShaft.World.Blocks
{
    public class SpongeBlock : Block
    {
        public override byte ID
        {
            get { return 19; }
        }

        public override byte Fallback
        {
            get { return 19; }
        }

        public override bool CPE
        {
            get { return false; }
        }

        public override bool Flammable
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "spongeblock"; }
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
            get { return 0f; }
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
            get { return 5; }
        }
    }
}
