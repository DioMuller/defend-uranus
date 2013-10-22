﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefendUranus.Helpers;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Extensions;
using MonoGameLib.Core.Sprites;

namespace DefendUranus.Entities
{
    public class PhysicsEntity : Entity
    {
        #region Attributes
        /// <summary>
        /// Angular forces applied once on the body.
        /// </summary>
        float _angularForce;
        /// <summary>
        /// Forces/Accelerations that are constantly being applied to the body.
        /// </summary>
        Vector2 _acceleration;
        /// <summary>
        /// Forces that will be applied only once to the body.
        /// These forces are not affected by game time.
        /// </summary>
        Vector2 _instantaneousForce;
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

        /// <summary>
        /// Computes the actual speed of the entity.
        /// </summary>
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

        /// <summary>
        /// How much this entity can bounce on collision.
        /// </summary>
        public float Restitution { get; set; }
        #endregion Properties

        #region Constructors
        public PhysicsEntity()
        {
        }

        public PhysicsEntity(string texturePath)
        {
            Sprite = new Sprite(texturePath, default(Point), 0);
            Sprite.Animations.Add(new Animation("default", 0, 0, 0));
            Sprite.Origin = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) / 2;
            Sprite.ChangeAnimation(0);
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Updates the Physics of the entity.
        /// </summary>
        /// <param name="gameTime">How much time have passed since the last update.</param>
        public override void Update(GameTime gameTime)
        {
            float secs = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 accelSecs = _acceleration * secs;

            Momentum += _instantaneousForce;
            Momentum *= Vector2.One - Friction;
            Position += WorldHelper.MetersToPixels((Momentum + accelSecs / 2) * secs);
            Momentum += accelSecs;

            if(RotateToMomentum)
                Rotation = Momentum.GetAngle();
            else
            {
                var angularAccelSecs = _angularForce * secs;
                var sum = (AngularMomentum + angularAccelSecs / 2);

                AngularMomentum *= 1 - RotationFriction;
                var toRotate = sum * secs;
                Rotation += toRotate;
                AngularMomentum += angularAccelSecs;

                _angularForce = 0;
            }

            _instantaneousForce = _acceleration = Vector2.Zero;

            AngularMomentum = MathHelper.Clamp(AngularMomentum, -MaxRotationSpeed, MaxRotationSpeed);
            if (Speed > MaxSpeed)
                Momentum *= MaxSpeed / Speed;

            base.Update(gameTime);
        }

        /// <summary>
        /// Apply a rotation force to the body.
        /// </summary>
        /// <param name="force">How much force is being applied.</param>
        public void ApplyRotation(float force)
        {
#if DEBUG
            if (RotateToMomentum)
                throw new InvalidOperationException("This entity is set to RotateToMomentum and manual rotation is disabled.");
#endif

            _angularForce += force / Mass;
        }

        /// <summary>
        /// Apply a force to the body.
        /// </summary>
        /// <param name="force">How much force to be applied.</param>
        /// <param name="instantaneous">
        /// Indicates if this force will be applied only once to this body.
        /// Leave it to False if this force is being applied on every game loop.
        /// </param>
        public void ApplyForce(Vector2 force, bool instantaneous = false)
        {
            if (instantaneous)
                _instantaneousForce += force / Mass;
            else
                _acceleration += force / Mass;
        }

        /// <summary>
        /// Apply an acceleration to this body.
        /// The acceleration is applied regardless of the body's mass.
        /// </summary>
        /// <param name="acceleration"></param>
        public void ApplyAcceleration(Vector2 acceleration)
        {
            _acceleration += acceleration;
        }
        #endregion Methods
    }
}
