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
        #region Nested
        public class Special
        {
            public string Name { get; set; }
            public Ship.SpecialAttackCreator Creator { get; set; }
        }
        #endregion

        #region Properties
        // TODO: Change Texture2D to Sprite and remove TexturePath.
        public Texture2D Texture { get; private set; }
        public string TexturePath { get; private set; }

        /// <summary>
        /// Text name of the ship.
        /// </summary>
        public string Name { get; private set; }

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
        /// The special attack that the ship will use.
        /// </summary>
        public Special SpecialAttack { get; private set; }

        /// <summary>
        /// How long does the fuel takes to empty, when on full power.
        /// </summary>
        public TimeSpan FuelDuration { get; private set; }
        #endregion

        #region Constructors
        public ShipDescription(ContentManager content, string texturePath, string name, Special specialAttack, float mass, float maxSpeed, TimeSpan fuel)
        {
            Texture = content.Load<Texture2D>(texturePath);
            TexturePath = texturePath;
            Mass = mass;
            Name = name;

            MaxSpeed = maxSpeed;
            RotationStabilizer = 0.5f;

            SpecialAttack = specialAttack;

            FuelDuration = fuel;
        }
        #endregion
    }
}
