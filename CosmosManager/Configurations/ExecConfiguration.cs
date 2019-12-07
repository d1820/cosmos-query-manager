using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Presenters;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using SimpleInjector;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosManager.Configurations
{
    public static class ExecConfiguration
    {
        public static void Configure(CommandLineApplication deployCommand, Container container)
        {

            deployCommand.ThrowOnUnexpectedArgument = false;
            deployCommand.Description = "Run scripts through Cosmos Manager to invoke against CosmosDB";
            deployCommand.HelpOption("-?|-h|-H|--help");
            var connectionsOption = new CommandOption("--connections", CommandOptionType.SingleValue)
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
            var outputOption = new CommandOption("--output", CommandOptionType.SingleValue)
            {
                Description = "The .CResult file the query output is written to."
            };
            var folderOption = new CommandOption("--folder", CommandOptionType.SingleValue)
            {
                Description = "The folder that contains the .CSQL scripts to execute."
            };
            var continueOnErrorOption = new CommandOption("--continueOnError", CommandOptionType.NoValue)
            {
                Description = "Flag to indicate whether to continue script executions on error."
            };
            var ignorePromptsOption = new CommandOption("--ignorePrompts", CommandOptionType.NoValue)
            {
                Description = "Flag to indicate whether to continue without accepting user input on prompts."
            };
            var includeDocumentInOutput = new CommandOption("--includeDocumentInOutput", CommandOptionType.NoValue)
            {
                Description = "Flag to indicate whether to write the document results to the console and output file."
            };

            deployCommand.Options.Add(connectToOption);
            deployCommand.Options.Add(connectionsOption);
            deployCommand.Options.Add(scriptOption);
            deployCommand.Options.Add(outputOption);
            deployCommand.Options.Add(folderOption);
            deployCommand.Options.Add(continueOnErrorOption);
            deployCommand.Options.Add(ignorePromptsOption);
            deployCommand.Options.Add(includeDocumentInOutput);

            deployCommand.OnExecuteAsync(async (cancelToken) =>
           {
               //setup all the code here
               var jsonString = File.ReadAllText(connectionsOption.Value());
               var connections = JsonConvert.DeserializeObject<List<Connection>>(jsonString);
               var selectedConnection = connections.FirstOrDefault(f => f.Name.Equals(connectToOption.Value(), System.StringComparison.InvariantCultureIgnoreCase));
               var presenter = container.GetInstance<ICommandlinePresenter>();
               presenter.SetConnections(connections);
               presenter.SelectedConnection = selectedConnection;
               presenter.InitializePresenter(null);
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
                       var query = File.ReadAllText(fi.FullName);
                       var result = await presenter.RunAsync(query, new CommandlineOptions
                       {
                           ContinueOnError = continueOnErrorOption.HasValue(),
                           IgnorePrompts = ignorePromptsOption.HasValue(),
                           OutputPath = outputOption.Value(),
                           IncludeDocumentInOutput = includeDocumentInOutput.HasValue()
                       }, cancelToken);
                       if (result != 0 && !continueOnErrorOption.HasValue())
                       {
                           return result;
                       }
                   }
               }
               return 0;
           });
        }

    }
}