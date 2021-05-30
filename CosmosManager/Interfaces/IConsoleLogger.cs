using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO;

namespace CosmosManager.Interfaces
{

    public interface IConsoleLogger : IConsole
    {
        string ReadLine();
    }

    public class ConsoleLogger :  IConsoleLogger
    {
        /// <summary>
        /// A shared instance of <see cref="PhysicalConsole"/>.
        /// </summary>
        public static IConsole Singleton { get; } = new ConsoleLogger();

        /// <summary>
        /// <see cref="Console.CancelKeyPress"/>.
        /// </summary>
        public event ConsoleCancelEventHandler? CancelKeyPress
        {
            add => Console.CancelKeyPress += value;
            remove
            {
                try
                {
                    Console.CancelKeyPress -= value;
                }
                catch (PlatformNotSupportedException)
                {
                    // https://github.com/natemcmaster/CommandLineUtils/issues/344
                    // Suppress this error during unsubscription on some Xamarin platforms.
                }
            }
        }

        /// <summary>
        /// <see cref="Console.Error"/>.
        /// </summary>
        public TextWriter Error => Console.Error;

        /// <summary>
        /// <see cref="Console.In"/>.
        /// </summary>
        public TextReader In => Console.In;

        /// <summary>
        /// <see cref="Console.Out"/>.
        /// </summary>
        public TextWriter Out => Console.Out;

        /// <summary>
        /// <see cref="Console.IsInputRedirected"/>.
        /// </summary>
        public bool IsInputRedirected => Console.IsInputRedirected;

        /// <summary>
        /// <see cref="Console.IsOutputRedirected"/>.
        /// </summary>
        public bool IsOutputRedirected => Console.IsOutputRedirected;

        /// <summary>
        /// <see cref="Console.IsErrorRedirected"/>.
        /// </summary>
        public bool IsErrorRedirected => Console.IsErrorRedirected;

        /// <summary>
        /// <see cref="Console.ForegroundColor"/>.
        /// </summary>
        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        /// <summary>
        /// <see cref="Console.BackgroundColor"/>.
        /// </summary>
        public ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        /// <summary>
        /// <see cref="Console.ResetColor"/>.
        /// </summary>
        public void ResetColor() => Console.ResetColor();

        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}