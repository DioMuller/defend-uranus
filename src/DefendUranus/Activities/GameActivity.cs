using Microsoft.Xna.Framework;
using MonoGameLib.Activities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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

        Task<GameTime> SyncDraw()
        {
            if (_syncDraw == null)
                _syncDraw = new TaskCompletionSource<GameTime>();
            return _syncDraw.Task;
        }

        Task<GameTime> SyncUpdate()
        {
            if (_syncUpdate == null)
                _syncUpdate = new TaskCompletionSource<GameTime>();
            return _syncUpdate.Task;
        }

        protected async Task FloatAnimation(int duration, Action<float> valueStep, int begin = 0)
        {
            if (duration <= 0)
                throw new ArgumentOutOfRangeException("duration", "Duration must be greater than zero");

            if (valueStep == null)
                throw new ArgumentNullException("valueStep");

            if (begin > 0)
                await TaskEx.Delay(begin);

            float curDuration = 0;
            do
            {
                var gt = await SyncDraw();
                curDuration += (int)gt.ElapsedGameTime.TotalMilliseconds;

                var curValue = curDuration / duration;
                valueStep(MathHelper.Clamp(curValue, 0, 1));
            } while (curDuration < duration);
        }

        protected Task FloatAnimation(int duration, float start, float end, Action<float> valueStep, int begin = 0)
        {
            if (valueStep == null)
                throw new ArgumentNullException("valueStep");

            return FloatAnimation(duration, value =>
            {
                valueStep(start + (end - start) * value);
            }, begin);
        }

        protected async Task FadeIn(int duration, Action<Color> colorStep, int begin = 0)
        {
            if (colorStep == null)
                throw new ArgumentNullException("colorStep");

            await FloatAnimation(duration, value =>
            {
                colorStep(new Color(Color.White, value));
            }, begin);
        }

        protected async Task FadeOut(int duration, Action<Color> colorStep, int begin = 0)
        {
            if (colorStep == null)
                throw new ArgumentNullException("colorStep");

            await FloatAnimation(duration, value =>
            {
                colorStep(new Color(Color.White, 1 - value));
            }, begin);
        }
    }
}
