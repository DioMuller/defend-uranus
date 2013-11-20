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
using DefendUranus.Helpers;
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
                var gt = await DrawContext.WaitNextLoop();
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
                DrawContext.PostLoop += FadeScreen;

            _fadeCancellation = new CancellationTokenSource();
            var duration = (int)(Math.Abs(_screenOpacity - textureDesiredOpacity) * completeDuration);
            if (_screenOpacity != textureDesiredOpacity)
                await FloatAnimation(duration, _screenOpacity, textureDesiredOpacity, v => _screenOpacity = v, cancellationToken: _fadeCancellation.Token);

            DrawContext.PostLoop -= FadeScreen;
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
