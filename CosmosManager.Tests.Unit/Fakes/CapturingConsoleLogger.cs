using CosmosManager.Interfaces;
using System;
using System.IO;
using System.Text;

namespace CosmosManager.Tests.Unit.Fakes
{
    /// <summary>
    /// Captures everything written through the McMaster IConsole.WriteLine extension (which writes
    /// to <see cref="Out"/>) into a buffer, and returns a scripted value for ReadLine prompts.
    /// </summary>
    public class CapturingConsoleLogger : IConsoleLogger
    {
        private readonly StringWriter _out = new StringWriter(new StringBuilder());

        public string ScriptedInput { get; set; } = "Y";

        public string CapturedOutput => _out.ToString();

        public TextWriter Out => _out;

        public TextWriter Error => _out;

        public TextReader In => TextReader.Null;

        public bool IsInputRedirected => true;

        public bool IsOutputRedirected => true;

        public bool IsErrorRedirected => true;

        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;

        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        public event ConsoleCancelEventHandler CancelKeyPress;

        public void ResetColor()
        {
        }

        public string ReadLine() => ScriptedInput;

        protected virtual void OnCancelKeyPress(ConsoleCancelEventArgs e) => CancelKeyPress?.Invoke(this, e);
    }
}
