using McMaster.Extensions.CommandLineUtils;
using SimpleInjector;

namespace CosmosManager.Configurations
{
    public static class CosmosManagerConfiguration
    {
        public static void Configure(CommandLineApplication mpCommand,
                                     string rootDir,
                                     string[] args,
                                     Container container)
        {
            mpCommand.Description = "Execute commands for script management for CosmosDB";
            mpCommand.HelpOption("-?|-h|-H|--help");
            mpCommand.Command("exec",  command2 => ExecConfiguration.Configure(command2, container));
            mpCommand.ThrowOnUnexpectedArgument = false;
            mpCommand.OnExecute(() =>
            {
                mpCommand.ShowHelp();
                return 0;
            });
        }
    }
}