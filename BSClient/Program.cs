using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BSClient
{
    //first commit
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();

            try
            {
                client.Connect(IPAddress.Parse("127.0.0.1"), 1234);

                if (!client.Connected)
                {
                    Console.WriteLine("Client NOT conncected.. aborting.....");
                    return;
                }
                Console.WriteLine("Client Connected!");
                NetworkStream stream = client.GetStream();
                if (stream != null)
                {
                    if(stream.CanWrite)
                    {
                        string message = "msg from Client";
                        byte[] bufferOutput = Encoding.Default.GetBytes(message);
                        stream.Write(bufferOutput, 0, bufferOutput.Length);


                        while (!stream.DataAvailable)
                        {
                            Thread.Sleep(2000);
                        }


                        byte[] bufferInput = new byte[2048];
                        int byteRead = stream.Read(bufferInput, 0, bufferInput.Length);
                        Console.WriteLine("Message received from server: "+Encoding.Default.GetString(bufferInput,0,byteRead));
                        Console.WriteLine("\t Client ending...");
                        stream.Close();
                        client.Close();
                    }
                    else
                    {
                        Console.WriteLine("Stream CANNOT be written... aborting..");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Stream Null... aborting..");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("general error: "+ex.ToString());
            }



            Console.WriteLine("Client finished.");
            Console.ReadKey();
        }
    }
}
