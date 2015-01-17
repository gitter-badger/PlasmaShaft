using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlasmaShaft;

namespace PlasmaShaft.Commands
{
    public class CmdStats : ICommand
    {
        public string Name { get { return "Stats"; } }
        public string Author { get { return ""; } }
        public int Version { get { return 1; } }
        public byte Permission { get { return 0; } }
        public void Use(Player p, string[] args)
        {
            if (args.Length != 0)
            {
                p.SendMessage(0, "Usage is /stats.");
                return;
            }

            p.SendMessage("Blocks built: " + p.BlocksBuilt);
            p.SendMessage("Blocks deleted: " + p.BlocksDeleted);
            p.SendMessage("Blocks drawn: " + p.BlocksDrawn);
            p.SendMessage("Messages written: " + p.MessagesWritten);
        }
        public void Help(Player p)
        {
            p.SendMessage(0, "Used to display your stats. Usage is /stats.");
        }
        public void Initialize()
        {
            Command.AddReference(this, "stats");
        }
    }
}