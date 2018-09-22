
using cyberdyne.Battleship.BSServer.Properties;
using cyberdyne.BattleshipCore;
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
        private Player[] players;
        //private bool player1_Ready, player2_Ready;
        public BattleshipServer()
        {
            this.players = new Player[2];
            this.listener = new TcpListener(IPAddress.Loopback, Settings.Default.ServerPort);

        }
        IAsyncResult result;
        public void ServerStart()
        {
            this.listener.Start();
            this.running = true;
            Console.WriteLine("Battleship server started at address [" + this.listener.LocalEndpoint.ToString() + "]. \nPress SPACEBAR to exit!");
            this.result = this.listener.BeginAcceptTcpClient(this.IncommingConnection, this.listener);

            Console.WriteLine("waiting players...");
            do
            {
                Thread.Sleep(2000);
            } while (this.players[0].Ready == false && this.players[1].Ready == false);
            Console.WriteLine("Players connected!");
            Thread.Sleep(2000);

            Console.WriteLine("Deploying ships...");
            Thread threadPlayer0 = null, threadPlayer1 = null;
            threadPlayer0 = new Thread(new ParameterizedThreadStart(this.DeployClientShips));
            threadPlayer1 = new Thread(new ParameterizedThreadStart(this.DeployClientShips));
            threadPlayer0.Start(this.players[0]);
            threadPlayer1.Start(this.players[1]);
            do
            {
                Thread.Sleep(3000);
            } while (threadPlayer0.ThreadState == ThreadState.Running && threadPlayer1.ThreadState == ThreadState.Running);
            Console.WriteLine("Ships deployed!");
            Thread.Sleep(2000);

            //todo:continuar daqui.







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


                if (this.players[0] == null)
                {
                    this.players[0] = new Player();
                    this.players[0].PlayerTcpClient = list.EndAcceptTcpClient(this.result);
                    this.result = this.listener.BeginAcceptTcpClient(this.IncommingConnection, this.listener);
                    if (this.PlayerSetup(this.players[0]))
                    {
                        this.players[0].Ready = true;
                    }
                    else
                    {
                        this.players[0].PlayerTcpClient.Close();
                        this.players[0] = this.players[1];
                        this.players[1] = null;
                    }
                }
                else if (this.players[1] == null)
                {
                    this.players[1] = new Player();
                    this.players[1].PlayerTcpClient = list.EndAcceptTcpClient(this.result);

                    if (this.PlayerSetup(this.players[1]))
                    {
                        this.players[1].Ready = true;
                    }
                    else
                    {
                        this.players[1].PlayerTcpClient.Close();
                        this.players[1] = null;
                        this.result = this.listener.BeginAcceptTcpClient(this.IncommingConnection, this.listener);
                    }
                }
                else
                {
                    list.EndAcceptTcpClient(this.result).Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("General error on incomming connection... " + ex.ToString());

            }


        }

        private bool PlayerSetup(Player _player)
        {
            ushort retries = 0;
            try
            {
                do
                {
                    this.SendClientCommand(_player.PlayerTcpClient, Commands.name);
                    string cmdReceived = this.GetClientCommand(_player.PlayerTcpClient);
                    if (!string.IsNullOrEmpty(cmdReceived))
                    {
                        string[] cmdReceivedParts = cmdReceived.Split('#');
                        if (cmdReceivedParts[0] == Commands.name.ToString())
                        {
                            _player.Name = cmdReceivedParts[1].Trim();
                            return true;
                        }
                    }
                    retries++;
                } while (retries < 3);

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on [PlayerSetup]: " + _player.PlayerTcpClient.ToString() + "\n" + ex.ToString());
                return false;
            }
        }

        private void DeployClientShips(Object _playerObj)
        {
            // *Commands Server->Client:
            // *name# (receive player´s name)
            // * deploy#  (receive shipModel (ou 'ready'), point 9,9, ship size 9, orientation h|v)
            // *
            // *Commands Client->Server:
            // *name#x30  (send playser´s name, max 30bytes)
            // * deploy#destroyer|1,4|3|h   (send shipModel (ou 'ready'), point 9,9, ship size 9, orientation h|v)

            Player player = (Player)_playerObj;
            player.Ready = false;

            do
            {
                this.SendClientCommand(player.PlayerTcpClient, Commands.deploy);
                string[] cmdReceived = this.GetClientCommand(player.PlayerTcpClient).Split('#');
                if (cmdReceived[0] == "deploy")
                {
                    string[] cmdReceivedParts = cmdReceived[1].Split('|');
                    if (cmdReceivedParts[0].Equals("ready"))
                    {
                        player.Ready = true;
                    }
                    else if (cmdReceived[0] == "deploy")
                    {
                        //cmdReceivedParts[0] = modelo de Ship
                        player.Board.DeployShip(new System.Drawing.Point(int.Parse(cmdReceivedParts[1].Split(',')[0]), int.Parse(cmdReceivedParts[1].Split(',')[1])),
                                                short.Parse(cmdReceivedParts[2]),
                                                (cmdReceivedParts[2] == "h" ? Orientation.Horizontal : Orientation.Vertical));
                    }
                }


            } while (!player.Ready);
        }


        private void SendClientCommand(TcpClient _tcpClient, Commands _command)
        {
            NetworkStream stream = _tcpClient.GetStream();
            byte[] bufferCmd;
            bufferCmd = Encoding.Default.GetBytes(_command.ToString() + "#");
            stream.Write(bufferCmd, 0, bufferCmd.Length);
        }

        private string GetClientCommand(TcpClient _tcpClient)
        {
            NetworkStream stream = _tcpClient.GetStream();
            while (!stream.DataAvailable)
            {
                Thread.Sleep(500);
            }
            byte[] bufferCmd = new byte[1024];
            int byteRead = stream.Read(bufferCmd, 0, bufferCmd.Length);
            string cmdReceived = Encoding.Default.GetString(bufferCmd, 0, byteRead);

            Console.WriteLine("Message received from client[" + _tcpClient.Client.ToString() + "]: " + cmdReceived);

            return cmdReceived;
        }
    }
}
