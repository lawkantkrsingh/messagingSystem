using AdvantestMessagingFoundation;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdvantestMessagingServer
{
    public class CentralMessageProcessor
    {
        private TcpListener listener;
        private ConcurrentDictionary<string, TcpClient> clients;
        private IStorageProvider storageProvider;
        private ILogProvider logProvider;
        private ConcurrentQueue<string> messageQueue;
        private int numberOfClient;

        /// <summary>
        /// this constructor will initialize the server with all required objects.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="storageProvider"></param>
        public CentralMessageProcessor(string ipAddress
            , int port
            , IStorageProvider storageProvider
            , ILogProvider logProvider,
              int numberOfClient = 0)
        {
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            clients = new ConcurrentDictionary<string, TcpClient>();
            this.storageProvider = storageProvider;
            messageQueue = new ConcurrentQueue<string>();
            this.logProvider = logProvider;
            this.numberOfClient = numberOfClient;
        }

        /// <summary>
        /// this method will star the server to listen from clients.
        /// </summary>
        /// <returns></returns>
        public async Task<string> Start()
        {
            listener.Start();

            logProvider.Log("Server started. Waiting for connections...");
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                if (clients.Count < numberOfClient)
                {
                    _ = HandleClientAsync(client);
                }
                else
                {
                    logProvider.Log("Can not connect to the current client as maximum limit of allowed client has reached.");
                }
            }
        }

        /// <summary>
        /// this method will stop the server
        /// </summary>
        /// <returns></returns>
        public async Task Stop()
        {
            try
            {
                listener.Stop();
                logProvider.Log("Server stoped.");
                Console.WriteLine("Server stoped.");
            }
            catch (Exception ex)
            {
                logProvider.Log("Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task HandleClientAsync(TcpClient client)
        {
            //Need to identify the unique name for client
            string clientId = Guid.NewGuid().ToString();
            clients.TryAdd(clientId, client);

            logProvider.Log($"Client connected: {clientId}");

            Console.WriteLine($"Client connected: {clientId}");

            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    logProvider.Log($"Received from {clientId}: {message}");

                    Console.WriteLine($"Received from {clientId}: {message}");

                    //Store data
                    storageProvider.StoreData(message);

                    //add the message to message queue
                    messageQueue.Enqueue(message);

                    //Process the message to send it to all connected clients
                    await ProcessMessageAsync(message, client);
                }
            }
            catch (Exception ex)
            {
                logProvider.Log($"Error handling client {clientId}: {ex.Message}");
                Console.WriteLine($"Error handling client {clientId}: {ex.Message}");
            }
            finally
            {
                client.Close();
                clients.TryRemove(clientId, out _);
                logProvider.Log($"Client disconnected: {clientId}");
                Console.WriteLine($"Client disconnected: {clientId}");
            }
        }

        /// <summary>
        /// this method is used to process the data for any purpose.
        /// </summary>
        /// <param name="data"></param>
        private async Task ProcessMessageAsync(string message, TcpClient client)
        {
            logProvider.Log("Broadcasting the message to all client: " + message);
            try
            {
                TcpClient _tcpClient = null;
                foreach (var _client in clients)
                {
                    byte[] buffer = Encoding.ASCII.GetBytes(_client.Key + " says: " + message);
                    _tcpClient = (TcpClient)_client.Value;
                    if (_tcpClient != client)
                    {
                        NetworkStream stream = _tcpClient.GetStream();
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                logProvider.Log("Error while broadcasting the message to all client: " + ex.Message);
            }
        }
    }
}

