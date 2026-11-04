using CosmosManager.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace CosmosManager.Tests.Unit.Fakes
{
    /// <summary>
    /// In-memory <see cref="ITextWriter"/> that captures the output stream instead of writing a file.
    /// </summary>
    public class CapturingTextWriter : ITextWriter
    {
        private readonly StringBuilder _buffer = new StringBuilder();
        public bool Closed { get; private set; }

        public string Captured => _buffer.ToString();

        public void WriteLine(string value) => _buffer.AppendLine(value);

        public Task WriteAsync(string value)
        {
            _buffer.Append(value);
            return Task.CompletedTask;
        }

        public void Close() => Closed = true;

        public void Dispose() => Close();
    }
}
