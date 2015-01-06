
namespace PlasmaShaftCore
{
    public abstract class Block
    {
        public static Block[] BlockList = new Block[] {
            new AirBlock(),
            new StoneBlock(),
            new GrassBlock(),
            new DirtBlock(),
            new CobblestoneBlock(),
            new WoodBlock(),
            new Plant(),
            new AdmincreteBlock(),
            new ActiveWater(),
            new StationaryWater(),
            new ActiveLava(),
            new StationaryLava(),
            new SandBlock(),
            new GravelBlock(),
            new GoldOre(),
            new IronOre(),
            new CoalOre(),
            new TreeBlock(),
            new Leaves(),
            new SpongeBlock(),
            new GlassBlock(),
            new RedWool(),
            new OrangeWool(),
            new YellowWool(),
            new LimeWool(),
            new GreenWool(),
            new TealWool(),
            new AquaWool(),
            new CyanWool(),
            new BlueWool(),
            new IndigoWool(),
            new VioletWool(),
            new MagentaWool(),
            new PinkWool(),
            new BlackWool(),
            new GrayWool(),
            new WhiteWool(),
            new YellowFlower(),
            new RedFlower(),
            new Mushroom(),
            new RedMushroom(),
            new GoldBlock(),
            new IronBlock(),
            new DoubleStair(),
            new Stair(),
            new Bricks(),
            new TNT(),
            new Bookshelf(),
            new MossyCobblestone(),
            new ObsidianBlock(),
            new CobblestoneStair(),
            new Rope(),
            new Sandstone(),
            new Snow(),
            new Fire(),
            new LightPinkWool(),
            new ForestGreenWool(),
            new BrownWool(),
            new DeepBlueWool(),
            new TurquoiseWool(),
            new IceBlock(),
            new CeramicTile(),
            new MagmaBlock(),
            new PillarBlock(),
            new Crate(),
            new StoneBricks()
        };

        public abstract byte ID { get; }
        public abstract byte Fallback { get; }
        public abstract string Name { get; }
        public abstract int Permission { get; set; }
        public abstract bool Walkthrough { get; }
        public abstract bool Opaque { get; }
        public abstract bool CPE { get; }
        public abstract bool Flammable { get; }
        public abstract bool WaterKill { get; }
        public abstract float Resistance { get; }
        public abstract short TouchingRadius { get; }

        public virtual void OnWalkthrough(Entity e) { }
        public virtual void OnDestroy(Entity e = null) { }
        public virtual void OnCreate(Entity e = null) { }
        public virtual void OnStanding(Entity e = null) { }
        public virtual void OnContact(byte b, short x, short y, short z) { }
        public virtual void OnContactEntity(Entity e = null) { }
        public virtual void Tick(Entity e = null) { }
    }
}
