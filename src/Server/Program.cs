using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using FarseerPhysics.Dynamics;
using OpenTK;
using Shared;
using Shared.GameObjects;
using Shared.Ini;
using FPVector2 = Microsoft.Xna.Framework.Vector2;

namespace Server
{
    public static class Program
    {
        public static NetServer NetServer;
        public static World World;

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

            IniFile settings = IniParser.Parse(File.ReadAllLines("settings.ini"));

            string port;

            settings.TryGetValue("ServerPort", out port, "10000");

            NetServer = new NetServer(int.Parse(port));

            NetServer.ClientConnected += (s, client) =>
            {
                Console.WriteLine("Client connected {0}", client.Client.RemoteEndPoint);

                int playerId = CreateIdFromConnection(client);
                GameObjects.Add(playerId, new Player(playerId, World));

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
                            player.Fixture.Body.ApplyForce(100000 * moveDirection.ToFPVector());
                            break;
                    }
                }
            };

            NetServer.Start();

            World = new World(new FPVector2(0.0f, 9.81f));

            GameObjects.Add(0, new Platform(0, World, new Vector2(0, 256), new Vector2(256, 16)));

            const int targetFps = 60;
            float delay = 1000f / targetFps;

            Stopwatch sw = new Stopwatch();

            for (int n = 0; NetServer.Running; n++)
            {
                sw.Reset();
                sw.Start();

                NetServer.RemoveDisconnectedClients();
                NetServer.ProcessMessages();

                World.Step(1f / targetFps);

                foreach (var kv in GameObjects)
                    kv.Value.Update(1000f / targetFps);

                if (n % 10 == 0)
                    foreach (var player in Players)
                    {
                        NetServer.BroadcastMessage((byte)NetMessageType.SetPosition, player.Id, player.Position);
                        NetServer.BroadcastMessage((byte)NetMessageType.SetVelocity, player.Id, player.Velocity);
                    }

                sw.Stop();

                delay = 1000f / targetFps - sw.ElapsedMilliseconds;

                if (delay > 0)
                    Thread.Sleep((int)Math.Ceiling(delay));
            }
        }
    }
}
