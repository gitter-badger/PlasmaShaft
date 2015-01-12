using System;
using System.Collections.Generic;
using System.Threading;

namespace PlasmaShaft
{
    public sealed partial class Player : Entity
    {
        public static List<Player> players = new List<Player>();

        public override string Name { get; set; }
        public override string Skin { get; set; }
        public override string Model { get; set; }
        public override short[] Pos  { get; set; }
        public override byte[] Rot { get; set; }
        public override int EntityID { get { return 0; } }
        public override bool NPC
        {
            get { return false; }
        }

        public override void Attack(Entity target) {
            throw new System.NotImplementedException();
        }

        public override void Teleport(short x, short y, short z) {
            throw new System.NotImplementedException();
        }

        public override void Walk(short x, short y, short z, float speed) {
            throw new System.NotImplementedException();
        }

        public void Say(string message, byte id = 0) {
            Player.players.ForEach(p => p.SendMessage(id, Name + ": " + message));
        }

        public static void UpdatePosition() {
            Player.players.ForEach(p => p.UpdatePos());
        }

        public void SpawnPlayersInLevel(bool self, bool reverse) {
            if (level == null) return;
            level.players.ForEach(p => {
                if (p != this) {
                    if (self) SpawnEntity(p);
                    if (reverse) p.SpawnEntity(this);
                }
            });
        }

        public void DespawnPlayersInLevel(bool self, bool reverse) {
            if (level == null) return;
            level.players.ForEach(p => {
                if (p != this) {
                    if (self) SpawnEntity(p);
                    if (reverse) p.SpawnEntity(this);
                }
            });
        }

        public static void Spawn(Entity e) { 
            
        }
        private void HandleCommand(string[] args)
        {
            string[] sendArgs = new string[0];
            if (args.Length > 1)
            {
                sendArgs = new string[args.Length - 1];
                for (int i = 1; i < args.Length; i++)
                {
                    sendArgs[i - 1] = args[i];
                }
            }

            string name = args[0].ToLower().Trim();
            if (Command.Commands.ContainsKey(name))
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    ICommand cmd = Command.Commands[name];
                    try
                    {
                        cmd.Use(this, sendArgs);
                    }
                    catch (Exception ex)
                    {
                        Server.Log("[Error] An error occured when " + Name + " tried to use " + name + "! " + ex.ToString(), LogMessage.ERROR);
                    }
                });
            }
            else
            {
                SendMessage(0, "Unknown command \"" + name + "\"!");
            }

        }
    }
}