using System;
using System.Collections.Generic;
using System.Linq;

namespace PlasmaShaft
{
    /// <summary>
    /// The command class, used to store commands for players to use
    /// </summary>
    public class Command
    {
        public static Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();

        /// <summary>
        /// Returns the dictionary of all commands.
        /// </summary>
        public static Dictionary<string, ICommand> All { get { return Commands; } }

        /// <summary>
        /// Add an array of referances to your command here
        /// </summary>
        /// <param name="command">the command that this referance... referances, you should most likely use 'this'</param>
        /// <param name="reference">the array of strings you want players to type to use your command</param>
        public static void AddReference(ICommand command, params string[] reference)
        {
            foreach (string s in reference)
            {
                AddReference(command, s.ToLower());
            }
        }
        /// <summary>
        /// Add a referance to your command here
        /// </summary>
        /// <param name="command">the command that this referance... referances, you should most likely use 'this'</param>
        /// <param name="reference">the string you want player to type to use your command, you can use this method more than once :)</param>
        public static void AddReference(ICommand command, string reference)
        {
            if (Commands.ContainsKey(reference.ToLower()))
            {
                Server.Log("Command " + command.Name + " replaces " + Commands[reference].Name + " for /" + reference, LogMessage.INFO);
                Commands[reference] = command;
                return;
            }
            Commands.Add(reference.ToLower(), command);
        }

        public static ICommand Find(string p)
        {
            try
            {
                KeyValuePair<string, ICommand> firstCmd = Commands.First((entry) => entry.Key == p);
                return firstCmd.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}