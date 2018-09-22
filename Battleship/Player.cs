using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using cyberdyne.BattleshipCore.board;

namespace cyberdyne.BattleshipCore
{
    public class Player
    {
        ushort score;
        string name;
        bool ready;
        TcpClient playerTcpClient;
        BoardBase playerBoard;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public ushort Score
        {
            get { return this.score; }
        }
        public bool Ready
        {
            get { return this.ready; }
            set { this.ready = value; }
        }
        public TcpClient PlayerTcpClient
        {
            get { return this.playerTcpClient; }
            set { this.playerTcpClient = value; }
        }
        public BoardBase Board
        {
            get { return this.playerBoard; }
        }

        public Player()
        {
            this.score = 0;
            this.ready = false;
            this.playerBoard = new BoardBase();
        }
        public void AddScore(ushort _points)
        {
            this.score += _points;

        }
    }
}
