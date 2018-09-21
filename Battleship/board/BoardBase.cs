using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace cyberdyne.BattleshipCore.board
{
    internal class BoardBase
    {
        bool[,] board;

        internal BoardBase()
        {
            this.board = new bool[10, 10];//all false by default

        }

        internal void DeployShip(Point _startPosition, short _size, Orientation _orientation)
        {
            if(_orientation == Orientation.Horizontal)
            {
                for (short i = 0; i < _size; i++)
                {
                    this.board[_startPosition.X+i, _startPosition.Y] = true;
                }
            }
            else
            {
                for (short i = 0; i < _size; i++)
                {
                    this.board[_startPosition.X, _startPosition.Y + i] = true;
                }
            }
        }
    }
}
