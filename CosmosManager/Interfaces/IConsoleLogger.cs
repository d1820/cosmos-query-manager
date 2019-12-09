using McMaster.Extensions.CommandLineUtils;
using System;

namespace CosmosManager.Interfaces
{

    public interface IConsoleLogger : IConsole
    {
        string ReadLine();
    }

    public class ConsoleLogger : PhysicalConsole, IConsoleLogger
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}