using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace cyberdyne.BattleshipCore.ship
{
    
    internal abstract class ShipClassBase
    {
        protected ShipPart[] parts;
        internal short ShipSize
        {
            get { return (short)this.parts.Length; }
        }

        internal virtual void Deploy(Orientation _orientation, Point _startPoint)
        {
            for (short i = 0; i < this.parts.Length; i++)
            {
                if (_orientation == Orientation.Horizontal)
                {
                    this.parts[i] = new ShipPart(new Point(_startPoint.X + i, _startPoint.Y));
                }
                else
                {
                    this.parts[i] = new ShipPart(new Point(_startPoint.X, _startPoint.Y + i));
                }
            }
        }
        //internal abstract void Hit(Point _position);
    }




    internal class ShipPart
    {
        private bool destroyed;
        private Point position;
        private Image imagePart;

        internal bool Destroyed
        {
            get { return this.destroyed; }
        }
        internal Point Position
        {
            get { return this.position; }
        }
        internal Image ImagePart
        {
            get { return this.imagePart; }
            set { this.imagePart = value; }
        }


        internal ShipPart(Point _position)
        {
            this.destroyed = false;
            this.position = _position;
        }
    }

}
