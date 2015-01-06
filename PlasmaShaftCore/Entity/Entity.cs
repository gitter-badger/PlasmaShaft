using System;
using PlasmaShaftCore.World;

namespace PlasmaShaftCore
{
    public abstract class Entity
    {
        /// <summary>
        /// Walks to a specified point with a given speed
        /// </summary>
        public abstract void Walk(short x, short y, short z, float speed);

        /// <summary>
        /// Teleports an entity to a point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public abstract void Teleport(short x, short y, short z);

        /// <summary>
        /// The attack AI of the entity
        /// </summary>
        public abstract void Attack(Entity target);

        /// <summary>
        /// The unique ID of the entity
        /// </summary>
        public abstract byte ID { get; set; }

        /// <summary>
        /// The entity ID of the entity
        /// 0 = Player
        /// </summary>
        public abstract int EntityID { get; }

        /// <summary>
        /// The name of the entity
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// The skin of the entity
        /// </summary>
        public abstract string Skin { get; set; }

        /// <summary>
        /// The model of the entity;
        /// </summary>
        public abstract string Model { get; set; }

        /// <summary>
        /// Determines whether the entity is an npc or not
        /// </summary>
        public abstract bool NPC { get; }

        /// <summary>
        /// The position of the entity
        /// </summary>
        public abstract short[] Pos { get; set; }

        /// <summary>
        /// The rotation of the player
        /// </summary>
        public abstract byte[] Rot { get; set; }

        /// <summary>
        /// The level the entity is in
        /// </summary>
        public virtual Level level { get; set; }
    }
}
