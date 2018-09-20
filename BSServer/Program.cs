using cyberdyne.Battleship.Properties;
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
        private bool running=false;
        private TcpListener listener;
        public BattleshipServer()
        {
            
            this.listener = new TcpListener(IPAddress.Loopback, Settings.Default.ServerPort);

        }
        IAsyncResult result;
        public void ServerStart()
        {
            this.listener.Start();
            this.running = true;
            Console.WriteLine("Battleship server started at address [" + this.listener.LocalEndpoint.ToString() + "]. \nPress SPACEBAR to exit!");
            result = this.listener.BeginAcceptTcpClient(IncommingConnection, this.listener);




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
                    return;

                TcpListener list = (TcpListener)_result.AsyncState;
                TcpClient client = list.EndAcceptTcpClient(this.result);
                this.result = this.listener.BeginAcceptTcpClient(IncommingConnection, this.listener);

                NetworkStream stream = client.GetStream();
                while (!stream.DataAvailable)
                {
                    Thread.Sleep(2000);
                }


                byte[] bufferInput = new byte[2048];
                int byteRead = stream.Read(bufferInput, 0, bufferInput.Length);
                Console.WriteLine("Message received from client: " + Encoding.Default.GetString(bufferInput, 0, byteRead));

                byte[] msgToClient = Encoding.Default.GetBytes("Msg received!");
                stream.Write(msgToClient, 0, msgToClient.Length);
                stream.Close();
                client.Close();
                stream = null;
                client = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("General error on incomming connection... " + ex.ToString());

            }


        }
    }
}
