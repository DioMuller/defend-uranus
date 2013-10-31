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
        // TODO: Change Texture to Sprite.
        public Texture2D Texture { get; private set; }
        public string TexturePath { get; set; }

        /// <summary>
        /// Text description of the ship.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// How much mass the ship have.
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Top speed of the ship.
        /// </summary>
        public float MaxSpeed { get; set; }

        /// <summary>
        /// Effectiveness of the ship's stabilizer.
        /// When set to 1, the ship can stop rotating immediately.
        /// </summary>
        public float RotationStabilizer { get; set; }

        /// <summary>
        /// The method that this ship will use as special attack.
        /// </summary>
        public Ship.SpecialAttackMethod SpecialAttack { get; set; }
        #endregion

        #region Constructors
        public ShipDescription(ContentManager content, string texturePath, float mass, string description, Ship.SpecialAttackMethod specialAttack)
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

        #region Public Methods
        /// <summary>
        /// Build the ship based on this description.
        /// </summary>
        /// <returns></returns>
        public Ship BuildShip(GamePlay gamePlay)
        {
            return new Ship(gamePlay, TexturePath)
            {
                Mass = Mass,
                MaxSpeed = MaxSpeed,
                RotationStabilizer = RotationStabilizer,
                SpecialAttack = SpecialAttack
            };
        }
        #endregion
    }
}
