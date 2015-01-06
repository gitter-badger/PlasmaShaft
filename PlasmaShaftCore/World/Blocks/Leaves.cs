namespace PlasmaShaftCore
{
    public class Leaves : Block
    {
        public override byte ID
        {
            get { return 18; }
        }

        public override byte Fallback
        {
            get { return 18; }
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
            get { return "leaves"; }
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
            get { return true; }
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
