#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Activities;
using MonoGameLib.Core.Events;
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
        #region Constants
        readonly Texture2D BlackTexture;
        #endregion

        #region Attributes
        float _screenOpacity = 1;
        CancellationTokenSource _fadeCancellation;

        TaskCompletionSource<GameTime> _syncDraw, _syncUpdate;
        int _waitingDraw, _waitingUpdate;
        #endregion

        #region Properties
        new public MainGame Game { get { return (MainGame)base.Game; } }
        #endregion

        #region Constructors
        public GameActivity(MainGame game)
            : base(game)
        {
            BlackTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            BlackTexture.SetData(new[] { Color.Black });
        }
        #endregion

        #region Activity Life-Cycle

        /// <summary>
        /// Animation to run when the activity is starting.
        /// </summary>
        /// <returns>The animation task.</returns>
        protected virtual Task IntroductionAnimation()
        {
            return FadeIn(100);
        }

        /// <summary>
        /// Animation to run when the activity is complete.
        /// </summary>
        /// <returns>The animation task.</returns>
        protected virtual Task ConclusionAnimation()
        {
            return FadeOut(100);
        }

        /// <summary>
        /// Animate the screen before / after completion.
        /// </summary>
        /// <returns>A task that represents the activity execution.</returns>
        protected async override Task<T> RunActivity()
        {
            await IntroductionAnimation();
            var result = await base.RunActivity();
            await ConclusionAnimation();

            return result;
        }

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
        #endregion

        #region Sync
        /// <summary>
        /// Wait asynchronously for the next Draw.
        /// </summary>
        /// <returns>A task that completes on PostDraw.</returns>
        public async Task<GameTime> WaitDraw()
        {
            if (_waitingDraw++ == 0)
                PostDraw += NotifyDrawCompleted;

            if (_syncDraw == null)
                _syncDraw = new TaskCompletionSource<GameTime>();
            var res = await _syncDraw.Task;

            if (--_waitingDraw == 0)
                PostDraw -= NotifyDrawCompleted;

            return res;
        }

        /// <summary>
        /// Wait asynchronously for the next Update.
        /// </summary>
        /// <returns>A task that completes on PostUpdate.</returns>
        public async Task<GameTime> WaitUpdate()
        {
            if (_waitingUpdate++ == 0)
                PostUpdate += NotifyUpdateCompleted;

            if (_syncUpdate == null)
                _syncUpdate = new TaskCompletionSource<GameTime>();
            var res = await _syncUpdate.Task;

            if (--_waitingUpdate == 0)
                PostUpdate -= NotifyUpdateCompleted;

            return res;
        }

        void NotifyUpdateCompleted(object sender, GameLoopEventArgs e)
        {
            if (_syncUpdate != null)
            {
                _syncUpdate.TrySetResult(e.GameTime);
                _syncUpdate = null;
            }
        }

        void NotifyDrawCompleted(object sender, GameLoopEventArgs e)
        {
            if (_syncDraw != null)
            {
                _syncDraw.TrySetResult(e.GameTime);
                _syncDraw = null;
            }
        }
        #endregion

        #region Animations
        public async Task FloatAnimation(int duration, float startValue, float endValue, Action<float> valueStep, TweeningFunction easingFunction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (duration <= 0)
                throw new ArgumentOutOfRangeException("duration", "Duration must be greater than zero");

            if (valueStep == null)
                throw new ArgumentNullException("valueStep");

            Action<float> updateValue = dur =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                valueStep(GetValue(dur, duration, startValue, endValue, easingFunction));
            };

            float curDuration = 0;
            do
            {
                updateValue(curDuration);
                var gt = await WaitDraw();
                curDuration += (int)gt.ElapsedGameTime.TotalMilliseconds;
            } while (curDuration < duration);
            updateValue(curDuration);
        }

        async Task FadeTo(float value, int completeDuration)
        {
            var textureDesiredOpacity = 1 - value;

            if (_fadeCancellation != null)
                _fadeCancellation.Cancel();
            else
                PostDraw += FadeScreen;

            _fadeCancellation = new CancellationTokenSource();
            var duration = (int)(Math.Abs(_screenOpacity - textureDesiredOpacity) * completeDuration);
            if (_screenOpacity != textureDesiredOpacity)
                await FloatAnimation(duration, _screenOpacity, textureDesiredOpacity, v => _screenOpacity = v, cancellationToken: _fadeCancellation.Token);

            PostDraw -= FadeScreen;
            _fadeCancellation = null;
        }

        public Task FadeIn(int completeDuration = 100)
        {
            return FadeTo(1, completeDuration);
        }

        public Task FadeOut(int completeDuration = 100)
        {
            return FadeTo(0, completeDuration);
        }

        void FadeScreen(object sender, GameLoopEventArgs args)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BlackTexture, drawRectangle: GraphicsDevice.Viewport.Bounds, color: new Color(Color.White, _screenOpacity));
            SpriteBatch.End();
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
