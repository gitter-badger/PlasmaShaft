﻿namespace PlasmaShaftCore
{
    public class ForestGreenWool : Block
    {
        public override byte ID
        {
            get { return 56; }
        }

        public override byte Fallback
        {
            get { return 56; }
        }

        public override bool CPE
        {
            get { return true; }
        }

        public override bool Flammable
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "forestgreenwool"; }
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
            get { return 1; }
        }
    }
}
