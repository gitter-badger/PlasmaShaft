
namespace PlasmaShaft
{
    public class CmdReloadCmds : ICommand
    {
        public string Name { get { return "Reload Commands"; } }
        public string Author { get { return ""; } }
        public int Version { get { return 1; } }
        public byte Permission { get { return 120; } }

        public void Use(Player p, string[] args)
        {
            Server.Say("Reloading the Command system, please wait.");
            Command.Commands.Clear();
            LoadAllDlls.InitCommands();
            Initialize();
        }

        public void Help(Player p)
        {
            p.SendMessage(0, "/reloadcommands - Reloads the command system");
            p.SendMessage(0, "Shortcuts: /reloadcmds, /rc");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[3] { "reloadcmds", "reloadcommands", "rc" });
        }
    }
}