﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefendUranus.Helpers;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Extensions;

namespace DefendUranus.Entities
{
    public class PhysicsEntity : Entity
    {
        #region Attributes
        float _angularForces;
        #endregion

        #region Properties
        /// <summary>
        /// Body momentum.
        /// </summary>
        public Vector2 Momentum { get; set; }

        /// <summary>
        /// Body angular momentum.
        /// </summary>
        public float AngularMomentum { get; set; }

        /// <summary>
        /// Movement Direction.
        /// </summary>
        public Vector2 Direction
        {
            get
            {
                return Vector2.Normalize(Momentum);
            }
        }

        public float Speed
        {
            get
            {
                return Momentum.Length();
            }
        }

        /// <summary>
        /// Limit the entity maximum speed.
        /// </summary>
        /// <value>The max speed.</value>
        public float MaxSpeed { get; set; }

        /// <summary>
        /// Limit the entity maximum rotation speed.
        /// </summary>
        public float MaxRotationSpeed { get; set; }

        /// <summary>
        /// Constant forces applied on the body.
        /// </summary>
        public Dictionary<string, Vector2> ConstantForces { get; set; }

        /// <summary>
        /// Forces applied once on the body.
        /// </summary>
        public Stack<Vector2> Forces { get; set; }

        /// <summary>
        /// Body Mass.
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Does the entity rotate with the force?
        /// </summary>
        public bool RotateToMomentum { get; set; }

        /// <summary>
        /// Constant friction being applied on the body.
        /// </summary>
        public Vector2 Friction { get; set; }

        /// <summary>
        /// Constant friction being applied on the body's rotation.
        /// </summary>
        public float RotationFriction { get; set; }
        #endregion Properties

        #region Constructor
        public PhysicsEntity() : base()
        {
            Forces = new Stack<Vector2>();
            ConstantForces = new Dictionary<string,Vector2>();

            Friction = Vector2.Zero;
            Momentum = Vector2.Zero;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Updates the Physics of the entity.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            Vector2 forces = Vector2.Zero;
            Vector2 instantForces = Vector2.Zero;
            float secs = (float)gameTime.ElapsedGameTime.TotalSeconds;

            #region Calculate Forces
            //Constant forces
            foreach (Vector2 force in ConstantForces.Values)
            {
                forces += force;
            }

            //Forces applied once
            while (Forces.Count > 0)
            {
                instantForces += (Forces.Pop() / Mass);
            }
            #endregion Calculate Forces

            Vector2 acceleration = (forces / Mass);
            Vector2 accelSecs = acceleration * secs;

            Momentum *= (Vector2.One - Friction);
            Position += WorldHelper.MetersToPixels((Momentum + accelSecs / 2) * secs);
            Momentum += (accelSecs + instantForces);

            if (!RotateToMomentum)
            {
                var angularAccelSecs = _angularForces * secs;
                var sum = (AngularMomentum + angularAccelSecs / 2);

                AngularMomentum *= 1 - RotationFriction;
                var toRotate = sum * secs;
                Rotation += toRotate;
                AngularMomentum += angularAccelSecs;

                _angularForces = 0;
            }

            if (RotateToMomentum)
                Rotation = Momentum.GetAngle();

            AngularMomentum = MathHelper.Clamp(AngularMomentum, -MaxRotationSpeed, MaxRotationSpeed);
            if (Speed > MaxSpeed)
                Momentum *= MaxSpeed / Speed;

            base.Update(gameTime);
        }

        public void ApplyRotation(float force)
        {
            _angularForces += force / Mass;
        }
        #endregion Methods
    }
}
