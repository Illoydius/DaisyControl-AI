

using DaisyControl_AI.Storage.DataAccessLayer;
using DaisyControl_AI.Storage.Workflows;
using DaisyControl_AI.Storage.Workflows.Main;

namespace DaisyControl_AI.Storage
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
            services.AddScoped<IDaisyControlDal, DaisyControlDal>();
        }
    }
}
