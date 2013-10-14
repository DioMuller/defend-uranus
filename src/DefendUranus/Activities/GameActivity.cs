﻿#region Using Statements
using Microsoft.Xna.Framework;
using MonoGameLib.Activities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using XNATweener;
#endregion

namespace DefendUranus.Activities
{
    abstract class GameActivity<T> : Activity<T>
    {
        #region Attributes
        TaskCompletionSource<GameTime> _syncDraw, _syncUpdate;
        #endregion

        #region Properties
        new public MainGame Game { get { return (MainGame)base.Game; } }
        #endregion

        #region Constructors
        public GameActivity(MainGame game)
            : base(game)
        {
        }
        #endregion

        #region Activity Life-Cycle
#if DEBUG
        protected override void Activating()
        {
            base.Activating();
            Console.WriteLine("{0} Activating", GetType().Name);
        }

        protected override void Deactivating()
        {
            base.Deactivating();
            Console.WriteLine("{0} Deactivating", GetType().Name);
        }

        /*protected override void Starting()
        {
            base.Starting();
            Console.WriteLine("{0} Starting", GetType().Name);
        }

        protected override void Completing()
        {
            base.Completing();
            Console.WriteLine("{0} Completing", GetType().Name);
        }*/
#endif
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            if (_syncUpdate != null)
            {
                _syncUpdate.TrySetResult(gameTime);
                _syncUpdate = null;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_syncDraw != null)
            {
                _syncDraw.TrySetResult(gameTime);
                _syncDraw = null;
            }
        }
        #endregion

        #region Sync
        public Task<GameTime> SyncDraw()
        {
            if (_syncDraw == null)
                _syncDraw = new TaskCompletionSource<GameTime>();
            return _syncDraw.Task;
        }

        public Task<GameTime> SyncUpdate()
        {
            if (_syncUpdate == null)
                _syncUpdate = new TaskCompletionSource<GameTime>();
            return _syncUpdate.Task;
        }
        #endregion

        #region Animations
        public async Task FloatAnimation(int duration, float startValue, float endValue, Action<float> valueStep, TweeningFunction easingFunction = null, CancellationToken ct = default(CancellationToken))
        {
            if (duration <= 0)
                throw new ArgumentOutOfRangeException("duration", "Duration must be greater than zero");

            if (valueStep == null)
                throw new ArgumentNullException("valueStep");

            float curDuration = 0;
            do
            {
                if (ct.IsCancellationRequested)
                    return;

                valueStep(GetValue(curDuration, duration, startValue, endValue, easingFunction));

                var gt = await SyncDraw();
                curDuration += (int)gt.ElapsedGameTime.TotalMilliseconds;
            } while (curDuration < duration);

            if (ct.IsCancellationRequested)
                return;

            valueStep(GetValue(curDuration, duration, startValue, endValue, easingFunction));
        }

        public async Task FadeIn(int duration, Action<Color> colorStep)
        {
            if (colorStep == null)
                throw new ArgumentNullException("colorStep");

            await FloatAnimation(duration, 0, 1, value =>
            {
                colorStep(new Color(Color.White, value));
            });
        }

        public async Task FadeOut(int duration, Action<Color> colorStep)
        {
            if (colorStep == null)
                throw new ArgumentNullException("colorStep");

            await FloatAnimation(duration, 1, 0, value =>
            {
                colorStep(new Color(Color.White, value));
            });
        }
        #endregion

        #region Private
        static float GetValue(float curDuration, int duration, float startValue, float endValue, TweeningFunction easing)
        {
            if (easing != null)
                return easing(curDuration, startValue, endValue - startValue, duration);

            var curValue = curDuration / duration;
            return MathHelper.Lerp(startValue, endValue, MathHelper.Clamp(curValue, 0, 1));
        }
        #endregion
    }
}
