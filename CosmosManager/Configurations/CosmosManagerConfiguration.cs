using McMaster.Extensions.CommandLineUtils;
using SimpleInjector;

namespace CosmosManager.Configurations
{
    public static class CosmosManagerConfiguration
    {
        public static int Create(string[] args,
                                     Container container)
        {
            var app = new CommandLineApplication { Name = "CosmosManager2019" };
            app.Description = "Execute commands for script management for CosmosDB";
            app.HelpOption("-?|-h|-H|--help");
            app.Command("exec", command => ExecConfiguration.ConfigureCommand(command, container));
            app.UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw;
            return app.Execute(args);
        }
    }
}