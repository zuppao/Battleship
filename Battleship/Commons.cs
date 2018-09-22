using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyberdyne.BattleshipCore
{
    //internal enum ShipClass
    //{
    //    Battleship,//4
    //    Cruiser,//3
    //    Submarine,//3
    //    Destroyer//2
    //}
    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public enum Commands
    {
        name,
        deploy,
        attack,
        hit,
        incoming,
        winner,
        loser,
        target,
        over,
        rematch
    }

}
