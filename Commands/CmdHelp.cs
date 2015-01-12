using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlasmaShaft;

namespace PlasmaShaft.Commands
{
    public class CmdHelp : ICommand
    {
        public string Name { get { return "Help"; } }
        public string Author { get { return ""; } }
        public int Version { get { return 1; } }
        public byte Permission { get { return 0; } }
        public void Use(Player p, string[] args)
        {
            if (args.Length == 0)
            {
                p.SendMessage(0, "&8Use &b/help [command] &8to view more info.");
                return;
            }
            try
            {
                ICommand cmd = Command.Find(args[0]);
                cmd.Help(p);
                return;
            }
            catch (Exception) { }
            p.SendMessage(0, "Could not find command or block specified");
            return;
        }
        public void Help(Player p)
        {
            p.SendMessage(0, "...really? Wow. Just... wow.");
        }
        public void Initialize()
        {
            Command.AddReference(this, "help");
        }
    }
}