

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
            services.AddSingleton<IUsersWorkflow, UsersWorkflow>();
            services.AddSingleton<IPersonasWorkflow, PersonasWorkflow>();

            // Data Access Layer
            services.AddSingleton<IUsersDal, UsersDal>();
            services.AddSingleton<IPersonasDal, PersonasDal>();
        }
    }
}
