using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlasmaShaft
{
    /// <summary>
    /// Interface for Commands
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// The name of the command
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The author of the command (to add multiple authors just make the string like "Merlin33069, someone else"
        /// </summary>
        string Author { get; }
        /// <summary>
        /// The command version
        /// </summary>
        int Version { get; }
        /// <summary>
        /// The default permission value for the command
        /// </summary>
        /// <remarks></remarks>
        byte Permission { get; }

        /// <summary>
        /// The method that will be called when a player uses this command
        /// </summary>
        /// <param name="p">a Player class</param>
        /// <param name="args">the args of the command the player sent</param>
        void Use(Player p, string[] args);
        /// <summary>
        /// The method to run when a player uses the /help command
        /// </summary>
        /// <param name="p">a Player instance</param>
        void Help(Player p);

        /// <summary>
        /// The initialization of the command, you need to add command references here.
        /// </summary>
        void Initialize();
    }
}