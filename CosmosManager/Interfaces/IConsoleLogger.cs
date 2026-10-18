using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO;

// claude-allow-multiple-types
namespace CosmosManager.Interfaces
{

    public interface IConsoleLogger : IConsole
    {
        string ReadLine();
    }

    public class ConsoleLogger : IConsoleLogger
    {
        private readonly IConsole _console = PhysicalConsole.Singleton;

        public TextWriter Out => _console.Out;

        public TextWriter Error => _console.Error;

        public TextReader In => _console.In;

        public bool IsInputRedirected => _console.IsInputRedirected;

        public bool IsOutputRedirected => _console.IsOutputRedirected;

        public bool IsErrorRedirected => _console.IsErrorRedirected;

        public ConsoleColor ForegroundColor
        {
            get => _console.ForegroundColor;
            set => _console.ForegroundColor = value;
        }

        public ConsoleColor BackgroundColor
        {
            get => _console.BackgroundColor;
            set => _console.BackgroundColor = value;
        }

        public event ConsoleCancelEventHandler CancelKeyPress
        {
            add => _console.CancelKeyPress += value;
            remove => _console.CancelKeyPress -= value;
        }

        public void ResetColor() => _console.ResetColor();

        public string ReadLine() => Console.ReadLine();
    }
}
