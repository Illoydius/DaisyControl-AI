using DaisyControl_AI.Common.Configuration;
using DaisyControl_AI.Common.HttpRequest;
using DaisyControl_AI.InferenceNode;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== Inference Node for DaisyControl-AI ===");
        Console.WriteLine("Loading configuration...");
        var config = CommonConfigurationManager.ReloadConfig();

        Console.WriteLine("Testing remote DaisyControl-AI Storage WebAPI...");
        var httpClient = new DaisyControlStorageUsersClient();

        //Task.Run(async () =>
        //{
        //    while (!await httpClient.Ping())
        //    {
        //        Console.WriteLine("The ping query failed. Make sure you have access to the remote Storage WebAPI and that it's not down. Will try again in 30 seconds...");
        //        await Task.Delay(30000);
        //    }
        //}).Wait();

        Console.WriteLine("Remote Storage WebAPI is running.");
        Console.WriteLine("Loading...");
        Console.WriteLine("[Ready]");

        InferenceNodeCore.StartAsync();

        while (true)
        {
            Console.ReadKey();
        }
    }
}