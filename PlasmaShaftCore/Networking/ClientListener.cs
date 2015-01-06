using System;
using System.Net;
using System.Net.Sockets;

namespace PlasmaShaftCore
{
    public class ClientListener
    {
        /// <summary>
        /// The port to listen to
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// The listener (server)
        /// </summary>
        private TcpListener tcpListener;

        public delegate void OnConnect(TcpClient client);

        /// <summary>
        /// What to do when a client connects
        /// </summary>
        public event OnConnect OnConnection = null;

        /// <summary>
        /// Creates an socket which listens for connections
        /// </summary>
        /// <param name="Port">The port to listen to</param>
        /// <returns></returns>
        public static ClientListener Create(int Port)  {
            return new ClientListener(Port);
        }

        /// <summary>
        /// Begins listening for incoming connections
        /// </summary>
        public bool Run() {
            try {
                tcpListener.Start();
                Server.Log("Created listening port on: " + Port, LogMessage.INFO);
            }
            catch {
                Server.Log("Failed to create listening port on: " + Port, LogMessage.ERROR);
                return false;
            }
            tcpListener.BeginAcceptTcpClient(Accept, null);
            return true;
        }

        /// <summary>
        /// Stops listening for incoming connections
        /// </summary>
        public void End() {
            tcpListener.Stop();
        }

        /// <summary>
        /// Accepts an incoming connection
        /// </summary>
        private void Accept(IAsyncResult result) {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(Accept, null);
            if (OnConnection != null) OnConnection(client);
        }

        /// <summary>
        /// Creates an instance of a tcp listener
        /// </summary>
        private ClientListener(int Port) {
            this.Port = Port;
            tcpListener = new TcpListener(IPAddress.Any, Port);
        }
    }
}
