using CosmosManager.Domain;
using CosmosManager.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using SimpleInjector;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CosmosManager.Configurations
{
    public static class ExecConfiguration
    {
        public static void ConfigureCommand(CommandLineApplication deployCommand, Container container)
        {
            deployCommand.UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue;
            deployCommand.Description = "Run scripts through Cosmos Manager to invoke against CosmosDB";
            deployCommand.HelpOption("-?|-h|-H|--help");
            var connectionsOption = deployCommand.Option("--connections", "The JSON file that contains all the CosmosDB connections information.", CommandOptionType.SingleValue);
            var connectToOption = deployCommand.Option("--connectTo", "The name of the connection to use to connect to CosmosDB.", CommandOptionType.SingleValue);
            var scriptOption = deployCommand.Option("--script", "The .CSQL script to execute.", CommandOptionType.SingleValue);
            var outputOption = deployCommand.Option("--output", "The .CResult file the query output is written to.", CommandOptionType.SingleValue);
            var folderOption = deployCommand.Option("--folder", "The folder that contains the .CSQL scripts to execute.", CommandOptionType.SingleValue);
            var continueOnErrorOption = deployCommand.Option("--continueOnError", "Flag to indicate whether to continue script executions on error.", CommandOptionType.NoValue);
            var ignorePromptsOption = deployCommand.Option("--ignorePrompts", "Flag to indicate whether to continue without accepting user input on prompts. Used for executing data altering scripts without transactions.", CommandOptionType.NoValue);
            var includeDocumentInOutput = deployCommand.Option("--includeDocumentInOutput", "Flag to indicate whether to write the document results to the console and output file.", CommandOptionType.NoValue);

            deployCommand.OnExecuteAsync(async (cancelToken) =>
           {
               using (var presenter = container.GetInstance<ICommandlinePresenter>())
               {
                   //setup all the code here
                   var jsonString = File.ReadAllText(connectionsOption.Value());
                   var connections = JsonConvert.DeserializeObject<List<Connection>>(jsonString);
                   var selectedConnection = connections.FirstOrDefault(f => f.Name.Equals(connectToOption.Value(), System.StringComparison.InvariantCultureIgnoreCase));

                   presenter.SetConnections(connections);
                   presenter.SelectedConnection = selectedConnection;
                   presenter.InitializePresenter(
                           new
                           {
                               Options = new CommandlineOptions
                               {
                                   ContinueOnError = continueOnErrorOption.HasValue(),
                                   IgnorePrompts = ignorePromptsOption.HasValue(),
                                   OutputPath = outputOption.Value(),
                                   IncludeDocumentInOutput = includeDocumentInOutput.HasValue()
                               }
                           });
                   var scriptsToRun = new List<FileInfo>();
                   if (!string.IsNullOrEmpty(scriptOption.Value()) && File.Exists(scriptOption.Value()))
                   {
                       scriptsToRun.Add(new FileInfo(scriptOption.Value()));
                   }
                   if (!string.IsNullOrEmpty(folderOption.Value()) && Directory.Exists(folderOption.Value()))
                   {
                       var di = new DirectoryInfo(folderOption.Value());
                       var files = di.GetFiles("*.csql", SearchOption.AllDirectories);
                       scriptsToRun.AddRange(files);
                   }
                   if (scriptsToRun.Any())
                   {
                       foreach (var fi in scriptsToRun)
                       {
                           presenter.WriteHeader('*', 300, $"{fi.FullName}");
                           var query = File.ReadAllText(fi.FullName);
                           var result = await presenter.RunAsync(fi.Name, query, cancelToken);
                           //await presenter.WriteResults();
                           if (result != 0 && !continueOnErrorOption.HasValue())
                           {
                               return result;
                           }
                       }
                   }
                   else
                   {
                       presenter.AddToQueryOutput("No scripts found to execute.");
                   }
                   return 0;
               }
           });
        }
    }
}