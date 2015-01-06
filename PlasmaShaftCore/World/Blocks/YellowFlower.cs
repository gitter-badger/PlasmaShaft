namespace PlasmaShaftCore
{
    public class YellowFlower : Block
    {
        public override byte ID
        {
            get { return 37; }
        }

        public override byte Fallback
        {
            get { return 37; }
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
            get { return "yellowflower"; }
        }

        public override bool Opaque
        {
            get { return false; }
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
            get { return true; }
        }

        public override short TouchingRadius
        {
            get { return 2; }
        }
    }
}
