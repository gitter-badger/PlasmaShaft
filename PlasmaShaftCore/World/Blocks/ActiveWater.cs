namespace PlasmaShaftCore
{
    public class ActiveWater : Block
    {
        public override byte ID
        {
            get { return 8; }
        }

        public override byte Fallback
        {
            get { return 8; }
        }

        public override bool CPE
        {
            get { return false; }
        }

        public override bool Flammable
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "active_water"; }
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
            get { return 660f; }
        }

        public override bool WaterKill
        {
            get { return false; }
        }

        public override bool Walkthrough
        {
            get { return true; }
        }

        public override short TouchingRadius
        {
            get { return 1; }
        }
    }
}