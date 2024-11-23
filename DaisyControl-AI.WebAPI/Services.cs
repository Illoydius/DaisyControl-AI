using DaisyControl_AI.Core.Comms.Discord.Commands;
using DaisyControl_AI.Core.Comms.Discord.UserMessages;
using DaisyControl_AI.WebAPI.Workflows;
using DaisyControl_AI.WebAPI.Workflows.Main;

namespace DaisyControl_AI.WebAPI
{
    /// <summary>
    /// Internal DI
    /// </summary>
    internal static class Services
    {
        internal static void ConfigureServices(IServiceCollection services)
        {
            // Workflows
            services.AddScoped<IWorkflow, MainWorkflow>();
            services.AddSingleton<IDiscordBotUserMessageHandler, DiscordBotUserMessageHandler>();
            services.AddSingleton<IDiscordBotCommandHandler, DiscordBotCommandHandler>();

            // Background services workers
        }
    }
}
