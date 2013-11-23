using DefendUranus.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.GUI.Components;
using System;
using System.Collections.Generic;

namespace DefendUranus.Components
{
    /// <summary>
    /// Panel with player stats.
    /// </summary>
    class PlayerGUI : MonoGameLib.GUI.Base.Component
    {
        #region Properties
        public Ship Ship { get; set; }
        public List<MonoGameLib.GUI.Base.Component> Children { get; set; }
        #endregion

        #region Constructors
        public PlayerGUI(string name, Ship ship, Point position, Point size)
        {
            const string font = "Fonts/DefaultFont";
            const int itemMargin = 4;
            const int maxItemHeight = 31;

            Children = new List<MonoGameLib.GUI.Base.Component>();

            Ship = ship;
            Position = position;
            Size = size;

            NameLabel = new Label(name, font) { Color = Color.White };
            Health = new ProgressBar("Health", font);
            Fuel = new ProgressBar("Fuel", font);
            Ammo = new ProgressBar("Ammo", font);
            Special = new ProgressBar("Special", font);

            Children.Add(NameLabel);
            Children.Add(Health);
            Children.Add(Fuel);
            Children.Add(Ammo);
            Children.Add(Special);

            var pSize = new Point(size.X, Math.Min((size.Y - itemMargin * Children.Count) / Children.Count, maxItemHeight));
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].HorizontalOrigin = MonoGameLib.GUI.Base.HorizontalAlign.Center;
                Children[i].Position = new Point((int)position.X, (int)(position.Y + size.Y * (float)i / Children.Count));
                Children[i].Size = pSize;
            }
        }
        #endregion

        public Label NameLabel { get; set; }
        public ProgressBar Health { get; set; }
        public ProgressBar Fuel { get; set; }
        public ProgressBar Ammo { get; set; }

        public ProgressBar Special { get; set; }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            UpdateProgress(Health, Ship.Health);
            UpdateProgress(Fuel, Ship.Fuel);
            UpdateProgress(Ammo, Ship.MainWeaponAmmo);
            UpdateProgress(Special, Ship.SpecialWeaponAmmo);

            Special.Text = "SP: " + Ship.SpecialWeaponAmmo.Quantity + "/" + Ship.SpecialWeaponAmmo.Maximum;

            foreach (var it in Children)
                it.Draw(gameTime, spriteBatch);
        }

        static void UpdateProgress(ProgressBar progress, Container container)
        {
            progress.MaximumValue = container.Maximum.Value;
            progress.CurrentValue = container.Quantity;
            progress.HighlightColor = GetForegroundColor(container);
            progress.BackgroundColor = GetBackgroundColor(container);
            progress.BorderColor = new Color(progress.HighlightColor, 0.5f);
        }

        static Color GetForegroundColor(Container container)
        {
            const float alpha = 0.7f;
            /*// Static color
            if(fuelContainer.IsOnReserve)
                return new Color(Color.Red, alpha);
            if (fuelContainer.Quantity < fuelContainer.Maximum / 2)
                return new Color(1, 1, 0, alpha);

            return new Color(0, 1, 0, alpha); /*/

            // Dynamic color
            float ratio;
            if (container.Reserve != null)
                ratio = (float)(container.Quantity - container.Reserve) / (float)(container.Maximum - container.Reserve);
            else
                ratio = container.Quantity / (float)container.Maximum;

            return new Color(
                r: MathHelper.Clamp((1 - ratio) * 2, 0, 1),
                g: MathHelper.Clamp(ratio * 2, 0, 1),
                b: 0,
                alpha: alpha
            );//*/
        }

        static Color GetBackgroundColor(Container container)
        {
            const float alpha = 0.6f;
            /*// Static color
            if (fuelContainer.IsOnReserve)
                return new Color(1, 0.85f, 0.7f, alpha);
            return new Color(Color.White, alpha); /*/

            // Dynamic color
            var ratio = container.Quantity / (float)container.Maximum;

            // lighter colors
            ratio = ratio * 0.3f + 0.7f;

            return new Color(
                r: 1,
                g: ratio / 2 + 0.5f,
                b: ratio,
                alpha: alpha
            );//*/
        }
    }
}
