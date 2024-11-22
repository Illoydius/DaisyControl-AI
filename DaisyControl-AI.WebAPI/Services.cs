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
            services.AddScoped<IWorkflow, MainWorkflow>();
        }
    }
}
