﻿using DefendUranus.SteeringBehaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Entities.SpecialAttacks
{
    class MeteoroidAlienform : SpecialAttack
    {
        #region Attributes
        Pursuit _pursuit;
        #endregion

        #region Constructors
        public MeteoroidAlienform(Ship owner, float lifetime)
            : base(owner.Level, "Sprites/Meteoroid-Alienform.png", Color.Green, owner, lifetime)
        {
            _owner = owner;
            RotateToMomentum = true;
            Momentum = Vector2.One;
            MaxSpeed = 9f;

            _pursuit = new Pursuit(this);
            SteeringBehaviors.Add(_pursuit);
        }
        #endregion

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            #region Selects target
             var target = _owner.Level.Entities.OfType<SpecialAttack>()
                .Where(s => s != this && s.Visible)
                .OrderBy(s => (s.Position - Position).LengthSquared())
                .FirstOrDefault();

            // If you got a new target...
            if (target != null && _pursuit.Target != target)
            {
                //target.SteeringBehaviors.Clear();
                target.SteeringBehaviors.Insert(0, new Flee(target) { PanicDistance = 10000f, Target = this});
                _pursuit.Target = target;
            }

            #endregion Selects target

            base.Update(gameTime);
        }
        #endregion
    }
}
