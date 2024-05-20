using AdvantestMessagingFoundation;

namespace AdvantestMessagingServer
{
    internal class Program
    {
        static CentralMessageProcessor centralMessageProcessor = null;
        static ILogProvider logProvider = null;
        static IStorageProvider storageProvider = null;

        static string ip = "127.0.0.1";
        static int port = 9000;
        static int numberOfClient = 3;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Please provide number of customer to be allowed to connect to server");

                string numberOfAllowedClient = Console.ReadLine();

                if (!string.IsNullOrEmpty(numberOfAllowedClient) && int.TryParse(numberOfAllowedClient, out int n))
                {
                    numberOfClient = int.Parse(numberOfAllowedClient);
                }

                logProvider = new LogProvider();
                storageProvider = new StorageProvider();

                centralMessageProcessor = new CentralMessageProcessor(ip
                    , port
                    , storageProvider
                    , logProvider
                    , numberOfClient);

                Console.Write("Please enter y/Y to start the server.");

                if (Console.ReadLine().ToLower().Equals("y"))
                {
                    centralMessageProcessor.Start();
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                logProvider.Log("Exception: " + ex.Message);
            }
        }
    }
}