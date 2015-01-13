using System;
using System.Collections.Generic;
using System.Threading;

namespace PlasmaShaft
{
    /// <summary>
    /// Represents a player object
    /// </summary>
    public sealed partial class Player : Entity
    {
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
            Server.Players.ForEach(p => p.SendMessage(id, Name + ": " + message));
        }

        public static void UpdatePosition() {
            Server.Players.ForEach(p => p.UpdatePos());
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

        #region PlayerInfo
        //Inspired (but mostly stolen) from fCraft

        /// <summary> If set, will be used instead of Name in chat. </summary>
        public string DisplayedName;

        /// <summary> First time the player ever logged in, UTC.</summary>
        public DateTime FirstLoginDate;

        /// <summary> Most recent time the player logged in, UTC. </summary>
        public DateTime LastLoginDate;

        /// <summary> Last time the player has been seen online (last logout), UTC. </summary>
        public DateTime LastSeen;

        //add current rank 

        //add previous rank

        /// <summary> Reason given for the most recent promotion/demotion. May be empty. </summary>
        public string RankChangeReason;

        /// <summary>
        /// Returns whether or not the player is banned
        /// </summary>
        public bool IsBanned;

        /// <summary> Date of most recent ban, UTC. May be DateTime.MinValue if player was never banned. </summary>
        public DateTime BanDate;

        /// <summary> Name of the player responsible for ban, may be empty. </summary>
        public string BannedBy;

        /// <summary> Reason for ban, may be empty. </summary>
        public string BanReason;

        /// <summary> Date of most recent unban, UTC. May be DateTime.MinValue if player was never unbanned. </summary>
        public DateTime UnbanDate;

        /// <summary> Name of the player responsible for most recent unban, may be empty. </summary>
        public string UnbannedBy;

        /// <summary> Reason given for the most recent unban, may be empty. </summary>
        public string UnbanReason;

        /// <summary> Number of bans issued by this player. </summary>
        public int TimesBannedOthers;

        /// <summary> Total amount of time the player spent on this server. </summary>
        public TimeSpan TotalTime;

        /// <summary> Total number of blocks manually built or painted by the player. </summary>
        public int BlocksBuilt;

        /// <summary> Total number of blocks manually deleted by the player. </summary>
        public int BlocksDeleted;

        /// <summary> Total number of blocks modified using draw and copy/paste commands. </summary>
        public long BlocksDrawn;

        /// <summary> Number of sessions/logins. </summary>
        public int TimesVisited;

        /// <summary> Total number of messages written. </summary>
        public int MessagesWritten;

        /// <summary> Number of kicks issues by this player. </summary>
        public int TimesKickedOthers;

        /// <summary> Number of times that this player has been manually kicked. </summary>
        public int TimesKicked;

        /// <summary> Date of the most recent kick.
        /// May be DateTime.MinValue if the player has never been kicked. </summary>
        public DateTime LastKickDate;

        /// <summary> Name of the entity that most recently kicked this player. May be empty. </summary>
        public string LastKickBy;

        /// <summary> Reason given for the most recent kick. May be empty. </summary>
        public string LastKickReason;

        /// <summary> Whether this player is currently frozen. </summary>
        public bool IsFrozen;

        /// <summary> Date of the most recent freezing.
        /// May be DateTime.MinValue of the player has never been frozen. </summary>
        public DateTime FrozenOn;

        /// <summary> Name of the entity that most recently froze this player. May be empty. </summary>
        public string FrozenBy;

         /// <summary> Whether this player is currently muted. </summary>
        public bool IsMuted;

        /// <summary> Date until which the player is muted. If the date is in the past, player is NOT muted. </summary>
        public DateTime MutedUntil;

        /// <summary> Name of the entity that most recently muted this player. May be empty. </summary>
        public string MutedBy;


        #endregion
    }
}