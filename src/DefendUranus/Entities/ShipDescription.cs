using DefendUranus.Activities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Entities
{
    /// <summary>
    /// Description of the ship to be used in the GamePlay.
    /// </summary>
    class ShipDescription
    {
        #region Properties
        // TODO: Change Texture2D to Sprite and remove TexturePath.
        public Texture2D Texture { get; private set; }
        public string TexturePath { get; private set; }

        /// <summary>
        /// Text description of the ship.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// How much mass the ship have.
        /// </summary>
        public float Mass { get; private set; }

        /// <summary>
        /// Top speed of the ship.
        /// </summary>
        public float MaxSpeed { get; private set; }

        /// <summary>
        /// Effectiveness of the ship's stabilizer.
        /// When set to 1, the ship can stop rotating immediately.
        /// </summary>
        public float RotationStabilizer { get; private set; }

        /// <summary>
        /// The method that this ship will use as special attack.
        /// </summary>
        public Ship.SpecialAttackCreator SpecialAttack { get; private set; }
        #endregion

        #region Constructors
        public ShipDescription(ContentManager content, string texturePath, float mass, string description, Ship.SpecialAttackCreator specialAttack)
        {
            Texture = content.Load<Texture2D>(texturePath);
            TexturePath = texturePath;
            Mass = mass;
            Description = description;

            MaxSpeed = 10;
            RotationStabilizer = 0.1f;

            SpecialAttack = specialAttack;
        }
        #endregion
    }
}
