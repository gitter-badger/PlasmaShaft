namespace PlasmaShaft.World.Blocks
{
    public class CobblestoneStair : Block
    {
        public override byte ID
        {
            get { return 50; }
        }

        public override byte Fallback
        {
            get { return 50; }
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
            get { return "cobblestonestair"; }
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
            get { return 250f; }
        }

        public override bool WaterKill
        {
            get { return false; }
        }

        public override bool Walkthrough
        {
            get { return true; } //stairs count
        }

        public override short TouchingRadius
        {
            get { return 1; }
        }
    }
}
