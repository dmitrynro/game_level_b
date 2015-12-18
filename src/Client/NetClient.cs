using System;
using System.IO;
using System.Net.Sockets;
using Shared;

namespace Client
{
    public sealed class NetClient : IDisposable
    {
        private TcpClient TcpClient;

        public event EventHandler<TcpClient> ReceivedData = (s, e) => { };

        public NetClient()
        {
            TcpClient = new TcpClient();
            TcpClient.ReceiveTimeout = 1000;
            TcpClient.SendTimeout = 1000;
        }

        public void Connect(string host, int port)
        {
            TcpClient.Connect(host, port);
        }

        public void Disconnect()
        {
            if (TcpClient.Connected)
                TcpClient.Close();
        }

        public void ProcessMessages()
        {
            if (TcpClient.Available > 0)
                ReceivedData(this, TcpClient);
        }

        public void SendMessage(params object[] args)
        {
            try
            {
                Stream stream = TcpClient.GetStream();
                stream.Write(args);
            }
            catch
            {
            }
        }

        public bool Disposed { get; private set; } = false;

        public void Dispose()
        {
            if (!Disposed)
            {
                Disconnect();

                Disposed = true;
            }
        }
    }
}
