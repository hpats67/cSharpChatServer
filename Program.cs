using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace chatserver
{
    class Program
    {
        public static Hashtable clientsList = new Hashtable();
        static void Main(string[] args)
        {
            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
            TcpListener serverSocket = new TcpListener(ipAddress, 3500);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine("Chat Server started ...");
            counter = 0;
            while((true))
            {
                counter ++;
                clientSocket = serverSocket.AcceptTcpClient();

                byte[] bytesFrom = new byte[10025];
                string dataFromClient = null;

                NetworkStream nStream = clientSocket.GetStream();
                nStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                clientsList.Add(dataFromClient, clientSocket);

                Broadcast(dataFromClient + "Joined ", dataFromClient, false);

                Console.WriteLine(dataFromClient + " joined the chatroom ");
                HandleClinet client = new HandleClinet();
                client.StartClient(clientSocket, dataFromClient, clientsList);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }

        public static void Broadcast(string msg, string uName, bool flag)
        {
            foreach(DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if(flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(uName + " says: " + msg);
                }
                else
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(msg);
                }

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }
    }

    public class HandleClinet
    {
        TcpClient clientSocket;
        string clNo;
        Hashtable clientsList;

        public void StartClient(TcpClient inClientSocket, string clineNo, Hashtable clist)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            this.clientsList = clist;
            Thread ctThread = new Thread(DoChat);
            ctThread.Start();
        }

        private void DoChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rcount = null;
            requestCount = 0;

            while((true))
            {
                try
                {
                    requestCount++;
                    NetworkStream nStream = clientSocket.GetStream();
                    nStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    Console.WriteLine("From client - " + clNo + ":" + dataFromClient);
                    rcount = Convert.ToString(requestCount);

                    Program.Broadcast(dataFromClient, clNo, true);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
