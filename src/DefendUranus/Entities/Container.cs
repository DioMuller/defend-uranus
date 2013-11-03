using Microsoft.Xna.Framework;
using System;

namespace DefendUranus.Entities
{
    public class Container
    {
        private int _quantity;
        private int? _maximum;

        public event EventHandler ValueChanged;

        public Container()
        {
        }

        public Container(int quantity)
        {
            _quantity = quantity;
            _maximum = quantity;
        }

        public Container(int quantity, int? maximum)
        {
            _quantity = quantity;
            _maximum = maximum;
        }

        public int? Maximum
        {
            get { return _maximum; }
            set
            {
                if (_maximum == value)
                    return;
                _maximum = value;

                if (_maximum != null && _quantity > _maximum.Value)
                    _quantity = _maximum.Value;

                FireValueChanged();
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity == value)
                    return;
                _quantity = MathHelper.Clamp(value, 0, _maximum ?? int.MaxValue);
                FireValueChanged();
            }
        }

        public int? Reserve { get; set; }

        public bool IsOnReserve
        {
            get
            {
                if (Reserve == null)
                    return false;
                return _quantity < Reserve.Value;
            }
        }

        void FireValueChanged()
        {
            var handler = ValueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public static bool operator <=(Container a, int quantity)
        {
            if (a == null)
                a = new Container(0);

            return a._quantity <= quantity;
        }

        public static bool operator >=(Container a, int quantity)
        {
            if (a == null)
                a = new Container(0);

            return a._quantity >= quantity;
        }

        public static bool operator <(Container a, int quantity)
        {
            if (a == null)
                a = new Container(0);

            return a._quantity < quantity;
        }

        public static bool operator >(Container a, int quantity)
        {
            if (a == null)
                a = new Container(0);
            return a._quantity > quantity;
        }

        public static bool operator ==(Container a, int quantity)
        {
            return a._quantity == quantity;
        }

        public static bool operator !=(Container a, int quantity)
        {
            return a._quantity != quantity;
        }

        public override bool Equals(object obj)
        {
            return this == obj as Container;
        }

        public override int GetHashCode()
        {
            return _quantity;
        }

        public static implicit operator int(Container container)
        {
            if (container == null)
                return 0;
            return container._quantity;
        }

        public static implicit operator int?(Container container)
        {
            if (container == null)
                return null;
            return container._quantity;
        }

        public bool IsFull { get { return _maximum != null && _quantity >= _maximum; } }

        public bool IsEmpty { get { return _quantity <= 0; } }

        public void Fill()
        {
            if (Maximum == null)
                throw new InvalidOperationException("This container does not have a maximum set");

            Quantity = Maximum.Value;
        }
    }
}
