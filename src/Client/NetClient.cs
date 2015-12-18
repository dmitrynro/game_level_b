using System;
using System.Net.Sockets;

namespace Client
{
    public sealed class NetClient
    {
        public TcpClient TcpClient;

        public NetClient(string host, int port)
        {
            TcpClient = new TcpClient();
            TcpClient.Connect(host, port);
        }

        public void ProcessMessages(Action<TcpClient> messageCallback)
        {
            if (TcpClient.Available > 0 && messageCallback != null)
                messageCallback(TcpClient);
        }
    }
}
