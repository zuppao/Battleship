using cyberdyne.Battleship.BSServer.Properties;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace cyberdyne.Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            #region TEST_AREA

            #endregion

            BattleshipServer bssServer = new BattleshipServer();
            //Thread trd1 = new Thread(new ThreadStart(bssServer.ServerStart));
            //trd1.Start();
            bssServer.ServerStart();

            ConsoleKeyInfo pressedKey;
            do
            {
                pressedKey = Console.ReadKey();

            }
            while (pressedKey.Key != ConsoleKey.Spacebar);
            bssServer.ServerStop();



            //while (trd1.ThreadState != ThreadState.Stopped)
            //{
            //    Thread.Sleep(1000);
            //}

            Console.WriteLine("Server end.");
            Console.ReadKey();
        }




    }

    class BattleshipServer
    {
        private bool running = false;
        private TcpListener listener;
        private TcpClient player1_TcpClient, player2_TcpClient;
        private bool player1_Ready, player2_Ready;
        public BattleshipServer()
        {
            this.player1_Ready = false;
            this.player2_Ready = false;
            this.listener = new TcpListener(IPAddress.Loopback, Settings.Default.ServerPort);

        }
        IAsyncResult result;
        public void ServerStart()
        {
            this.listener.Start();
            this.running = true;
            Console.WriteLine("Battleship server started at address [" + this.listener.LocalEndpoint.ToString() + "]. \nPress SPACEBAR to exit!");
            result = this.listener.BeginAcceptTcpClient(IncommingConnection, this.listener);

            Console.WriteLine("waiting players...");
            do
            {
                Thread.Sleep(3000);
            } while (this.player1_TcpClient == null && this.player2_TcpClient == null);
            Console.WriteLine("Players connected!");
            Thread.Sleep(2000);

            Console.WriteLine("Deploying ships...");
            do
            {
                Thread.Sleep(2000);
            } while (this.player1_Ready == false && this.player2_Ready == false);
            Console.WriteLine("Ships deployed!");
            Thread.Sleep(2000);

            Console.WriteLine("Choosing who will start...");
            Random rd = new Random();
            int rest;
            Math.DivRem(rd.Next(1, 10), 2, out rest);
            Thread.Sleep(1000);
            if (rest == 0)
            {
                Console.WriteLine("Player 1 Starts!");
            }
            else
            {
                Console.WriteLine("Player 2 Starts!");
            }

            #region GAME INTERACTION
            /********
             *  --> PRE-GAME
             * Commands Server->Client:
             *    name# (receive player´s name)
             *    deploy#  (receive shipModel (ou 'ready'), point 9,9, ship size 9, orientation h|v)
             *    
             * Commands Client->Server:
             *    name#x30  (send playser´s name, max 30bytes)
             *    deploy#destroyer|1,4|3|h   (send shipModel (ou 'ready'), point 9,9, ship size 9, orientation h|v)
             *    
             *    
             * --> GAME LOOP
             * Commands Server->Client:
             *    attack# (receive a Point)
             *    hit#0|1 (send true|false)
             *    
             *    incoming#9,9 (send a Point 9,9)
             *    
             *    winner
             *    loser
             *    
             * Commands Client->Server:
             *    target#9,9 (send a Point 9,9)
             * 
             * 
             * --> POST-GAME
             * Commands Client->Server:
             *    over#
             *    rematch# (receive 0|1)
             * 
             * Commands Server->Client:
             *    over#
             *    rematch# (receive 0|1)
             * 
             * //if some player(client) send 'over', server send 'over' to other player and the game ends.
             * //if some player(client) send 'rematch', server send 'rematch' to other player
             * //     if other player send '0' the game ends. if other plauer send '1' the game restarts from "deploy" phase
             * 
             */
            #endregion


            //while (this.running)
            //{
            //    Console.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + "\tloop heartbeat");
            //    if (!this.running)
            //    {
            //        break;
            //    }

            //    Thread.Sleep(10000);
            //}

            //Thread.Sleep(1000);
            //Console.WriteLine("Server Stopped!");
        }

        public void ServerStop()
        {
            Console.WriteLine("stopping server...");
            this.running = false;
            this.listener.Stop();

        }

        void IncommingConnection(IAsyncResult _result)
        {

            try
            {
                if (!this.running)
                {
                    return;
                }

                TcpListener list = (TcpListener)_result.AsyncState;
                if (this.player1_TcpClient == null)
                {
                    this.player1_TcpClient = list.EndAcceptTcpClient(this.result);
                    this.result = this.listener.BeginAcceptTcpClient(IncommingConnection, this.listener);
                    this.PlayerSetup(this.player1_TcpClient);
                    this.player1_Ready = true;
                }
                else
                {
                    this.player2_TcpClient = list.EndAcceptTcpClient(this.result);
                    this.PlayerSetup(this.player2_TcpClient);
                    this.player2_Ready = true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("General error on incomming connection... " + ex.ToString());

            }


        }

        private void PlayerSetup(TcpClient _playerTcpClient)
        {
            NetworkStream stream = _playerTcpClient.GetStream();
            while (!stream.DataAvailable)
            {
                Thread.Sleep(2000);
            }


            byte[] bufferInput = new byte[2048];
            int byteRead = stream.Read(bufferInput, 0, bufferInput.Length);
            Console.WriteLine("Message received from client: " + Encoding.Default.GetString(bufferInput, 0, byteRead));

            byte[] msgToClient = Encoding.Default.GetBytes("Msg received!");
            stream.Write(msgToClient, 0, msgToClient.Length);

            //so teste.......
            //stream.Close();
            //_playerTcpClient.Close();
            //stream = null;
            //_playerTcpClient = null;
        }

    }
}
