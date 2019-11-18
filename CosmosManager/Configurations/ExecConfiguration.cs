using McMaster.Extensions.CommandLineUtils;
using SimpleInjector;

namespace CosmosManager.Configurations
{
    public static class ExecConfiguration
    {
        public static void Configure(CommandLineApplication deployCommand, string rootDir, string[] args, Container container)
        {

            deployCommand.ThrowOnUnexpectedArgument = false;
            deployCommand.Description = "Run scripts through Cosmos Manager to invoke against CosmosDB";
            deployCommand.HelpOption("-?|-h|-H|--help");
            var buildOption = new CommandOption("--connections", CommandOptionType.SingleValue)
            {
                Description = "The JSON file that contains all the CosmosDB connections information."
            };

            var connectToOption = new CommandOption("--connectTo", CommandOptionType.SingleValue)
            {
                Description = "The name of the connection to use to connect to CosmosDB."
            };
            var scriptOption = new CommandOption("--script", CommandOptionType.SingleValue)
            {
                Description = "The .CSQL script to execute."
            };
            var folderOption = new CommandOption("--folder", CommandOptionType.SingleValue)
            {
                Description = "The folder that contains the .CSQL scripts to execute."
            };
            var continueOnErrorOption = new CommandOption("--continueOnError", CommandOptionType.NoValue)
            {
                Description = "Flag to indicate whether to continue script executions on error."
            };


            deployCommand.Options.Add(buildOption);
            deployCommand.Options.Add(scriptOption);
            deployCommand.Options.Add(folderOption);
            deployCommand.Options.Add(continueOnErrorOption);


            deployCommand.OnExecute(() =>
            {
                //setup all the code here

                return 0;
            });
        }


    }
}