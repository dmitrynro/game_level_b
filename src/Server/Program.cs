using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using OpenTK;
using Shared;
using Shared.GameObjects;

namespace Server
{
    public static class Program
    {
        public static NetServer NetServer;

        public static Thread MainThread, InputThread;

        public static Dictionary<int, GameObject> GameObjects = new Dictionary<int, GameObject>();

        public static IEnumerable<Player> Players { get { return GameObjects.Where(obj => obj.Value is Player).Select(obj => obj.Value as Player); } }

        private static int CreateIdFromConnection(TcpClient client)
        {
            return 1000 + Math.Abs(client.Client.RemoteEndPoint.GetHashCode());
        }

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

            NetServer.ClientConnected += (s, client) =>
            {
                Console.WriteLine("Client connected {0}", client.Client.RemoteEndPoint);

                int playerId = CreateIdFromConnection(client);
                GameObjects.Add(playerId, new Player(playerId));

                NetServer.BroadcastMessage((byte)NetMessageType.ClientConnected, playerId);
                NetServer.SendMessage(client, (byte)NetMessageType.SetClientId, playerId);
            };

            NetServer.ClientDisconnected += (s, client) =>
            {
                Console.WriteLine("Client disconnected {0}", client.Client.RemoteEndPoint);

                int playerId = CreateIdFromConnection(client);
                GameObjects.Remove(playerId);

                NetServer.BroadcastMessage((byte)NetMessageType.ClientDisconnected, playerId);
            };

            NetServer.ReceivedData += (s, client) =>
            {
                while (client.Available > 0)
                {
                    Stream stream = client.GetStream();

                    switch ((NetMessageType)stream.ReadByte())
                    {
                        case NetMessageType.SetMoveDirection:
                            int playerId = CreateIdFromConnection(client);
                            Vector2 moveDirection = stream.ReadVector2();

                            Player player = GameObjects[playerId] as Player;
                            player.Velocity = 0.1f * moveDirection;
                            break;
                    }
                }
            };

            NetServer.Start();

            const int targetFps = 60;
            const float delay = 1000f / targetFps; // TODO нормально считать время с последнего апдейта

            int n = 0;

            while (NetServer.Running)
            {
                NetServer.RemoveDisconnectedClients();
                NetServer.ProcessMessages();

                foreach (var kv in GameObjects)
                    kv.Value.Update(delay);

                if (n % 10 == 0)
                    foreach (var player in Players)
                    {
                        NetServer.BroadcastMessage((byte)NetMessageType.SetPosition, player.Id, player.Position);
                        NetServer.BroadcastMessage((byte)NetMessageType.SetVelocity, player.Id, player.Velocity);
                    }

                n++;

                Thread.Sleep((int)Math.Ceiling(delay));
            }
        }
    }
}
