namespace PlasmaShaft.World.Blocks
{
    public class StationaryLava : Block
    {
        public override byte ID
        {
            get { return 11; }
        }

        public override byte Fallback
        {
            get { return 11; }
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
            get { return "stilllava"; }
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
            get { return 550f; }
        }

        public override bool WaterKill
        {
            get { return true; }
        }

        public override bool Walkthrough
        {
            get { return true; }
        }

        public override short TouchingRadius
        {
            get { return 3; }
        }
    }
}
