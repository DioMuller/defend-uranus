using MonoGameLib.Activities;

namespace DefendUranus.Activities
{
    abstract class GameActivity<T> : Activity<T>
    {
        #region Properties
        new public MainGame Game { get { return (MainGame)base.Game; } }
        #endregion

        #region Constructors
        public GameActivity(MainGame game)
            : base(game)
        {
        }
        #endregion
    }
}
