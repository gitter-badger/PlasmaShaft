using PlasmaShaftCore;

namespace PlasmaShaftCore.World
{
    public static class BlockQueue
    {
        public static int Chunks = 200;
        private static Blockchange bc = new Blockchange();
        private static System.Timers.Timer Updates = new System.Timers.Timer(100);

        public static void Begin()
        {
            Updates.Elapsed += delegate
            {
                Server.levels.ForEach((l) =>
                {
                    try
                    {
                        int c = Chunks;
                        if (l.BlockQueue_.Count <= 0) return;

                        if (l.BlockQueue_.Count < Chunks) c = l.BlockQueue_.Count;
                        if (l.players.Count == 0) c = l.BlockQueue_.Count;
                        for (int i = 0; i < c; i++)
                            l.PlayerBlockchange(l.BlockQueue_[i].p, l.BlockQueue_[i].x, l.BlockQueue_[i].y, l.BlockQueue_[i].z, l.BlockQueue_[i].type);
                        l.BlockQueue_.RemoveRange(0, c);
                    }
                    catch
                    {
                        l.BlockQueue_.Clear();
                    }
                });
            };

            Updates.Start();
        }

        public static void Enqueue(Player p, short x, short y, short z, byte type)
        {
            bc.x = x; bc.y = y; bc.z = z; bc.p = p; bc.type = type;
            try
            {
                p.level.BlockQueue_.Add(bc);
            }
            catch (System.OutOfMemoryException)
            {
                p.level.BlockQueue_.Clear();
            }
        }
    }
}
