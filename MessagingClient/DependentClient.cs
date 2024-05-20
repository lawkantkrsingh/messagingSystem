using AdvantestMessagingFoundation;
using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdvantestMessagingClient
{
    public class DependentClient
    {
        private string serverIp;
        private int serverPort;
        private IStorageProvider storageProvider;
        private ILogProvider logProvider;
        private TcpClient client;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        /// <param name="storageProvider"></param>
        /// <param name="logProvider"></param>
        public DependentClient(string serverIp
            , int serverPort
            , IStorageProvider storageProvider
            , ILogProvider logProvider)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            this.storageProvider = storageProvider;
            this.logProvider = logProvider;

            client = new TcpClient();
        }

        public async Task ConnectToServerAsync()
        {
            try
            {
                await client.ConnectAsync(serverIp, serverPort);
            }
            catch (Exception ex)
            {
                logProvider.Log("Exception: " + ex.Message);
            }


            // Example: Send messages to the server
            //await SendMessageAsync(client, "Hello from client.");

            // Example: Receive messages from the server
            //await ReceiveMessagesAsync(client);
        }

        /// <summary>
        /// this method will close the client connection.
        /// </summary>
        public void CloseConnectionAsync()
        {
            try
            {
                client.Close();
                logProvider.Log("Disconnected to server.");
                Console.WriteLine("Disconnected to server.");
            }
            catch (Exception ex)
            {
                logProvider.Log("Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// this method will send message to server
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await client.GetStream().WriteAsync(buffer, 0, buffer.Length);

                logProvider.Log("Message sent to server: " + message);
                storageProvider.StoreData("Message sent to server: " + message);
                Console.WriteLine("Message sent to server.");
            }
            catch (Exception ex)
            {
                logProvider.Log("Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// this method will receive message from server
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task ReceiveMessagesAsync(TcpClient client)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await client.GetStream().ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    logProvider.Log("Message received from server: " + receivedMessage);
                    storageProvider.StoreData("Message received from server: " + receivedMessage);

                    Console.WriteLine("Message received from server: " + receivedMessage);
                }
            }
            catch (Exception ex)
            {
                logProvider.Log("Exception: " + ex.Message);
            }
        }
    }
}
