using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace cyberdyne.BattleshipCore.ship
{
    internal class ShipModelSubmarine : ShipClassBase
    {
        internal ShipModelSubmarine()
        {
            this.parts = new ShipPart[3];
        }

        internal override void Deploy(Orientation _orientation, Point _startPoint)
        {
            //to deploy the parts on the right position
            base.Deploy(_orientation, _startPoint);

            //setting up the images
            this.parts[0].ImagePart = Image.FromFile("");
            this.parts[1].ImagePart = Image.FromFile("");
            this.parts[2].ImagePart = Image.FromFile("");
            //supposed that all images are in horizontal format
            if (_orientation == Orientation.Vertical)
            {//turn the images
                this.parts[0].ImagePart.RotateFlip(RotateFlipType.Rotate90FlipNone);
                this.parts[1].ImagePart.RotateFlip(RotateFlipType.Rotate90FlipNone);
                this.parts[2].ImagePart.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
        }
    }
}
