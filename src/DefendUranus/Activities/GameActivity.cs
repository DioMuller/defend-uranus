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

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
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

        protected async Task FloatAnimation(int duration, Action<float> valueStep)
        {
            if (valueStep == null)
                throw new ArgumentNullException("valueStep");

            float maxDuration = duration;
            do
            {
                var gt = await SyncDraw();
                duration -= (int)gt.ElapsedGameTime.TotalMilliseconds;

                var alpha = duration / maxDuration;
                valueStep(MathHelper.Clamp(1 - alpha, 0, 1));
            } while (duration > 0);
        }

        protected async Task FadeIn(int duration, Action<Color> colorStep)
        {
            await FloatAnimation(duration, value =>
            {
                colorStep(new Color(Color.White, value));
            });
        }

        protected async Task FadeOut(int duration, Action<Color> colorStep)
        {
            await FloatAnimation(duration, value =>
            {
                colorStep(new Color(Color.White, 1 - value));
            });
        }
    }
}
