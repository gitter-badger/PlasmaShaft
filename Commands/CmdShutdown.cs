using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlasmaShaft;

namespace PlasmaShaft.Commands
{
    public class CmdShutdown : ICommand
    {
        public string Name { get { return "Shutdown"; } }
        public string Author { get { return ""; } }
        public int Version { get { return 1; } }
        public byte Permission { get { return 0; } }
        public void Use(Player p, string[] args)
        {
            if (args.Length != 0)
            {
                p.SendMessage(0, "Usage is /shutdown.");
                return;
            }

            Server.Shutdown();    
        }
        public void Help(Player p)
        {
            p.SendMessage(0, "Used to shutdown the server. Usage is /shutdown.");
        }
        public void Initialize()
        {
            Command.AddReference(this, "shutdown");
        }
    }
}