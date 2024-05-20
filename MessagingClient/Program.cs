using AdvantestMessagingFoundation;

namespace AdvantestMessagingClient;

class Program
{
    static DependentClient dependentClient;
    static ILogProvider logProvider;
    static IStorageProvider storageProvider;

    static string serverIp = "127.0.0.1";
    static int serverPort = 9000;
    static string messageToServer = "";

    static void Main(string[] args)
    {
        try
        {
            logProvider = new LogProvider();
            storageProvider = new StorageProvider();
            dependentClient = new DependentClient(serverIp, serverPort, storageProvider, logProvider);
            bool isConnected = false;

            Console.WriteLine("Enter c/C to connect to server");

            if (Console.ReadLine().ToLower().Equals("c"))
            {
                try
                {
                    dependentClient.ConnectToServerAsync();
                    isConnected = true;
                }
                catch (Exception ex)
                {
                    isConnected = false;
                    Console.WriteLine(ex.Message);
                }
            }

            while (isConnected)
            {
                logProvider.Log("Connected to server. Provide input to send to server");
                storageProvider.StoreData("Connected to server. Provide input to send to server");

                Console.WriteLine("Connected to server. Provide input to send to server");

                messageToServer = Console.ReadLine();

                if (!string.IsNullOrEmpty(messageToServer))
                {
                    dependentClient.SendMessageAsync(messageToServer);
                }

                Console.WriteLine("Enter d/D to disconnect.");
                if (Console.ReadLine().ToLower().Equals("y"))
                {
                    dependentClient.CloseConnectionAsync();
                    isConnected = false;
                }
            }
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            logProvider.Log("Exception: " + ex.Message);
        }
    }
}
