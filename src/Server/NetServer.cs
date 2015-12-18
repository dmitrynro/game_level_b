using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server
{
    public sealed class NetServer
    {
        private TcpListener TcpListener;

        public bool Running { get; private set; } = true;

        public List<TcpClient> Clients = new List<TcpClient>();

        public NetServer(int port)
        {
            TcpListener = TcpListener.Create(port);
        }

        public async void Start(Action<TcpClient> connectedCallback)
        {
            TcpListener.Start();

            while (Running)
            {
                var client = await TcpListener.AcceptTcpClientAsync();
                Clients.Add(client);

                if (connectedCallback != null)
                    connectedCallback(client);
            }
        }

        public void Stop()
        {
            Running = false;
        }

        public void ProcessMessages(Action<TcpClient> messageCallback)
        {
            if (messageCallback != null)
                foreach (var client in Clients)
                    if (client.Available > 0)
                        messageCallback(client);
        }

        public void RemoveDisconnectedClients(Action<TcpClient> disconnectedCallback)
        {
            List<TcpClient> markedToDelete = new List<TcpClient>();

            foreach (var client in Clients)
                if (!client.Connected)
                    markedToDelete.Add(client);

            if (disconnectedCallback != null)
                foreach (var client in markedToDelete)
                    disconnectedCallback(client);

            Clients.RemoveAll(markedToDelete.Contains);
        }

        // TODO поменять callback-и на event-ы
    }
}
