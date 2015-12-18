using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Shared;

namespace Server
{
    public sealed class NetServer
    {
        private TcpListener TcpListener;
        private List<TcpClient> Clients = new List<TcpClient>();

        public bool Running { get; private set; } = true;

        public event EventHandler<TcpClient> ClientConnected = (s, e) => { };
        public event EventHandler<TcpClient> ReceivedData = (s, e) => { };
        public event EventHandler<TcpClient> ClientDisconnected = (s, e) => { };

        public NetServer(int port)
        {
            TcpListener = TcpListener.Create(port);
            TcpListener.Server.ReceiveTimeout = 1000;
            TcpListener.Server.SendTimeout = 1000;
        }

        public async void Start()
        {
            TcpListener.Start();

            while (Running)
            {
                var client = await TcpListener.AcceptTcpClientAsync();
                Clients.Add(client);
                ClientConnected(this, client);
            }
        }

        public void Stop()
        {
            Running = false;
        }

        public void ProcessMessages()
        {
            foreach (var client in Clients)
                if (client.Available > 0)
                    ReceivedData(this, client);
        }

        public void RemoveDisconnectedClients()
        {
            List<TcpClient> markedToDelete = new List<TcpClient>();

            foreach (var client in Clients)
                if (!client.Connected)
                    markedToDelete.Add(client);

            foreach (var client in markedToDelete)
                ClientDisconnected(this, client);

            Clients.RemoveAll(markedToDelete.Contains);
        }

        public void SendMessage(TcpClient client, params object[] args)
        {
            try
            {
                Stream stream = client.GetStream();
                stream.Write(args);
            }
            catch
            {
            }
        }

        public void BroadcastMessage(params object[] args)
        {
            foreach (var client in Clients)
                SendMessage(client, args);
        }
    }
}
