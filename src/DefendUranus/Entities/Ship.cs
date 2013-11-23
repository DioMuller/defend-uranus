using System.Collections.Generic;
using DefendUranus.Activities;
using DefendUranus.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonoGameLib.Core;
using MonoGameLib.Core.Particles;

namespace DefendUranus.Entities
{
    class Ship : GamePlayEntity
    {
        #region Nested
        public delegate Task SpecialAttackMethod(Ship owner);
        #endregion

        #region Constants
        /// <summary>
        /// Time between main weapon shots.
        /// </summary>
        static readonly TimeSpan MainWeaponDelay = TimeSpan.FromMilliseconds(100);
        /// <summary>
        /// Time between special weapon shots.
        /// </summary>
        static readonly TimeSpan SpecialWeaponDelay = TimeSpan.FromSeconds(1);
        /// <summary>
        /// For how long the power can be used before emptying.
        /// </summary>
        static readonly TimeSpan FuelDuration = TimeSpan.FromSeconds(4);
        /// <summary>
        /// How long does it take for the power to refill.
        /// Refill will only occur if the power is not in use.
        /// </summary>
        static readonly TimeSpan FuelRegenTime = TimeSpan.FromSeconds(2);
        /// <summary>
        /// How much fuel is needed for automatic operations.
        /// RotationStabilizer is only applied when the fuel is not on reserve.
        /// </summary>
        static readonly TimeSpan FuelReserve = TimeSpan.FromSeconds(0.30f);
        #endregion

        #region Attributes
        bool _useReserve;
        float _rotating, _accelerating;
        public readonly AutoRegenContainer Fuel = new AutoRegenContainer((int)FuelDuration.TotalMilliseconds, FuelRegenTime)
        {
            Reserve = (int)FuelReserve.TotalMilliseconds
        };
        public readonly AutoRegenContainer MainWeaponAmmo = new AutoRegenContainer(20, TimeSpan.FromSeconds(5));
        public readonly AutoRegenContainer SpecialWeaponAmmo = new AutoRegenContainer(3, TimeSpan.FromSeconds(60));

        private ParticleEmiter _thrustParticleEmiter, _rotateParticleEmiter;
        private Color _particleColor;
        #endregion

        #region Properties
        /// <summary>
        /// How much force is applied to the ship's engine.
        /// </summary>
        public float ThrotleForce { get; set; }

        /// <summary>
        /// How much force is applied to the ship's rotation.
        /// </summary>
        public float RotationForce { get; set; }

        /// <summary>
        /// Effectiveness of the ship's stabilizer.
        /// When set to 1, the ship can stop rotating immediately.
        /// </summary>
        public float RotationStabilizer { get; set; }

        /// <summary>
        /// Controls the MainWeapon usage.
        /// </summary>
        public AsyncOperation MainWeapon { get; set; }

        /// <summary>
        /// Controls the SpecialWeapon usage.
        /// </summary>
        public AsyncOperation SpecialWeapon { get; set; }

        /// <summary>
        /// The method that this ship will use as special attack.
        /// </summary>
        public SpecialAttackMethod SpecialAttack { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a ship with default parameters.
        /// </summary>
        /// <param name="level">The level that will contain this Ship.</param>
        /// <param name="texturePath">The location of the ship's draw image.</param>
        public Ship(GamePlay level, string texturePath, Color particleColor, float particleDuration, int normalAmmo, int specialAmmo)
            : base(level, texturePath)
        {
            Health = new Container(20);
            RotationForce = 10;
            MaxRotationSpeed = 3;
            MaxSpeed = 10;
            ThrotleForce = 40;
            Restitution = 0.5f;

            MainWeaponAmmo.Maximum = normalAmmo;
            SpecialWeaponAmmo.Maximum = specialAmmo;

            MainWeapon = new AsyncOperation(c => FireWeapon(MainWeaponAmmo, FireLaser, c));
            SpecialWeapon = new AsyncOperation(c => FireWeapon(SpecialWeaponAmmo, FireSpecialWeapon, c));

            #region Particle
            _particleColor = particleColor;

            List<ParticleState> particleStates = new List<ParticleState> {
                new ParticleState { Color = _particleColor, Scale = 1f, Duration = particleDuration },
                new ParticleState { Color = new Color(_particleColor * 0.2f, 0), Scale = 3f }
            };

            _thrustParticleEmiter = new ParticleEmiter(Level, "Particles/spark.png", particleStates) { MillisecondsToEmit = 8f, OpeningAngle = 20f, ParticleSpeed = 1f };
            _rotateParticleEmiter = new ParticleEmiter(Level, "Particles/spark.png", particleStates) { MillisecondsToEmit = 8f, OpeningAngle = 10f, ParticleSpeed = 2f };
            #endregion Particle
        }

        /// <summary>
        /// Create a ship based on this description.
        /// </summary>
        /// <param name="level">The level that will contain this Ship.</param>
        /// <param name="description">The ship description details.</param>
        public Ship(GamePlay level, ShipDescription description)
            : this(level, description.TexturePath, description.ParticleColor, description.ParticleDuration, description.NormalAmmo, description.SpecialAmmo)
        {
            Mass = description.Mass;
            MaxSpeed = description.MaxSpeed;
            RotationStabilizer = description.RotationStabilizer;
            SpecialAttack = description.SpecialAttack.Special;

            Fuel = new AutoRegenContainer((int)description.FuelDuration.TotalMilliseconds, FuelRegenTime)
            {
                Reserve = (int)FuelReserve.TotalMilliseconds
            };
        }
        #endregion

        #region Actions
        /// <summary>
        /// Rotate the ship in the specified direction.
        /// If no direction is specified the ship will try to stop its rotation.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="force">
        /// How much force will be applied to rotation.
        /// Possible values range from -1 to 1.
        /// </param>
        public void Rotate(GameTime gameTime, float force)
        {
#if DEBUG
            if (Math.Abs(force) > 1)
                throw new ArgumentOutOfRangeException("force", "Force must be between -1 and 1.");
#endif

            _rotating = 0;

            if (Math.Abs(force) <= 0.1f)
            {
                if (Math.Abs(AngularMomentum) < 0.01f)
                {
                    _useReserve = false;
                    AngularMomentum = 0;
                    return;
                }
                if (Fuel.IsOnReserve && !_useReserve) return;
                force = MathHelper.Clamp(-AngularMomentum * RotationStabilizer, -1, 1);
                _useReserve = true;
            }

            var fuelNeeded = (int)(gameTime.ElapsedGameTime.TotalMilliseconds * Math.Abs(force));

            if (Fuel.Quantity > fuelNeeded)
            {
                _rotating = force;
                Fuel.Quantity -= fuelNeeded;
                ApplyRotation(force * RotationForce, isAcceleration: true);
            }
        }

        public void Accelerate(GameTime gameTime, float thrust)
        {
#if DEBUG
            if (Math.Abs(thrust) > 1)
                throw new ArgumentOutOfRangeException("thrust", "Thrust must be between -1 and 1.");
#endif
            _accelerating = 0;
            if (Math.Abs(thrust) < 0.1f)
                return;

            var fuelNeeded = (int)(gameTime.ElapsedGameTime.TotalMilliseconds * Math.Abs(thrust));

            if (Fuel.Quantity > fuelNeeded)
            {
                _accelerating = thrust;
                Fuel.Quantity -= fuelNeeded;
                ApplyForce(Vector2Extension.AngleToVector2(Rotation) * thrust * ThrotleForce);
            }
        }

        public void DeployAttack(SpecialAttack attack)
        {
            var direction = Vector2Extension.AngleToVector2(Rotation);

            attack.Position = Position + direction * Size / 2;

            attack.ApplyForce(direction * attack.MaxSpeed, instantaneous: true);
            ApplyForce(-direction * attack.Mass, instantaneous: true);

            Level.AddEntity(attack);
        }
        #endregion

        #region Game Loop
        #region Update
        public override void Update(GameTime gameTime)
        {
            const float InvisibleParticleIntensity = 0.3f;

            Fuel.Regenerate = _rotating == 0 && _accelerating == 0;
            Fuel.Update(gameTime);
            MainWeaponAmmo.Update(gameTime);
            SpecialWeaponAmmo.Update(gameTime);

            #region Particles
            _thrustParticleEmiter.Enabled = _accelerating != 0;
            if (_thrustParticleEmiter.Enabled)
            {
                _thrustParticleEmiter.Momentum = Momentum;
                _thrustParticleEmiter.Intensity = Math.Abs(_accelerating);

                if (!Visible)
                    _thrustParticleEmiter.Intensity *= InvisibleParticleIntensity;

                _thrustParticleEmiter.Position = this.Position + new Vector2(0, 10 * _accelerating > 0? 1 : -1).RotateRadians(Rotation);
                _thrustParticleEmiter.Direction = new Vector2(0, _accelerating > 0 ? 1 : -1).RotateRadians(Rotation);
            }
            _thrustParticleEmiter.Update(gameTime);

            _rotateParticleEmiter.Enabled = _rotating != 0;
            if (_rotateParticleEmiter.Enabled)
            {
                _rotateParticleEmiter.Momentum = Momentum;
                _rotateParticleEmiter.Intensity = Math.Abs(_rotating);

                if (!Visible)
                    _rotateParticleEmiter.Intensity *= InvisibleParticleIntensity;

                _rotateParticleEmiter.Position = this.Position + new Vector2(0, -8).RotateRadians(Rotation);
                _rotateParticleEmiter.Direction = new Vector2(_rotating < 0 ? 1 : -1, 0).RotateRadians(Rotation);
            }
            _rotateParticleEmiter.Update(gameTime);

            #endregion Particles

            base.Update(gameTime);
        }
        #endregion
        #endregion

        #region Private
        async Task FireWeapon(AutoRegenContainer container, Func<CancellationToken, Task<bool>> fire, CancellationToken cancellation)
        {
            if (container.IsEmpty)
                return;

            container.Regenerate = false;

            while (!cancellation.IsCancellationRequested)
                if (!await fire(cancellation))
                    break;

            container.Regenerate = true;
        }

        public async Task<bool> FireLaser(CancellationToken cancellation)
        {
            if (MainWeaponAmmo.IsEmpty)
                return false;

            var direction = Vector2Extension.AngleToVector2(Rotation);

            var laser = new Laser(this, Momentum, Position, direction);

            laser.ApplyAcceleration(direction * laser.MaxSpeed, instantaneous: true);
            ApplyForce(-direction * laser.Mass, instantaneous: true);

            MainWeaponAmmo.Quantity--;
            SoundManager.PlaySound("Shoot01");
            Level.AddEntity(laser);

            await UpdateContext.Delay(MainWeaponDelay);
            return true;
        }

        public async Task<bool> FireSpecialWeapon(CancellationToken cancellation)
        {
            if (SpecialAttack == null || SpecialWeaponAmmo.IsEmpty)
                return false;

            SpecialWeaponAmmo.Quantity--;
            SoundManager.PlaySound("Special03");
            await TaskEx.WhenAll(
                SpecialAttack(this),
                UpdateContext.Delay(SpecialWeaponDelay));
            return true;
        }
        #endregion
    }
}
