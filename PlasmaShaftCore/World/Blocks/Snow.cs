namespace PlasmaShaft.World.Blocks
{
	public class Snow : Block
	{
		public override byte ID
		{
			get { return 53; }
		}

		public override byte Fallback
		{
			get { return 0; }
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
            get { return "snow"; }
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
			get { return 0; }
		}
	}
}
