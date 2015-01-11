namespace PlasmaShaft.World.Blocks
{
    public class Mushroom : Block
    {
        public override byte ID
        {
            get { return 39; }
        }

        public override byte Fallback
        {
            get { return 39; }
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
            get { return "mushroom"; }
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
            get { return 20f; }
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
