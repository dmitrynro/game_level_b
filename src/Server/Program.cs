using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using OpenTK;
using Shared;

namespace Server
{
    public static class Program
    {
        public static NetServer NetServer;

        public static Thread MainThread, InputThread;

        public static Dictionary<int, Player> Players = new Dictionary<int, Player>();

        private static void Main(string[] args)
        {
            MainThread = Thread.CurrentThread;

            InputThread = new Thread(() =>
            {
                while (true)
                {
                    string cmd = Console.ReadLine();

                    if (cmd == "quit")
                    {
                        NetServer?.Stop();
                        break;
                    }
                }
            });
            InputThread.Start();

            NetServer = new NetServer(10000);
            NetServer.Start((client) =>
            {
                Console.WriteLine("Client connected {0}", client.Client.RemoteEndPoint);
                Players.Add(client.Client.RemoteEndPoint.GetHashCode(), new Player());
            });

            while (NetServer.Running)
            {
                const float elapsedTime = 0.01f; // TODO нормально считать время с последнего апдейта

                NetServer.ProcessMessages((client) =>
                {
                    while (client.Available > 0)
                    {
                        Stream stream = client.GetStream();

                        switch ((NetMessageType)stream.ReadByte())
                        {
                            case NetMessageType.Move:
                                int id = client.Client.RemoteEndPoint.GetHashCode();
                                Player player = Players[id];
                                Vector2 moveDirection = new Vector2(stream.ReadSingle(), stream.ReadSingle());
                                Vector2 position = player.Position + 100 * moveDirection * elapsedTime;
                                player.Position = position;

                                foreach (var cl in NetServer.Clients)
                                {
                                    Stream responseStream = cl.GetStream();
                                    responseStream.WriteByte((byte)NetMessageType.SetPosition);
                                    responseStream.Write(id);
                                    responseStream.Write(position.X);
                                    responseStream.Write(position.Y);
                                }
                                break;
                        }
                    }
                });

                NetServer.RemoveDisconnectedClients((client) =>
                {
                    Players.Remove(client.Client.RemoteEndPoint.GetHashCode());
                    Console.WriteLine("Client disconnected {0}", client.Client.RemoteEndPoint);
                });

                Thread.Sleep(10);
            }
        }
    }
}
