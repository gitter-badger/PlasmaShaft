namespace PlasmaShaftCore
{
    public class Fire : Block
    {
        public override byte ID
        {
            get { return 54; }
        }

        public override byte Fallback
        {
            get { return 54; }
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
            get { return "fire"; }
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
            get { return true; }
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
