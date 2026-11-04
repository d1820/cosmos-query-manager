using CosmosManager.Interfaces;

namespace CosmosManager.Tests.Unit.Fakes
{
    /// <summary>
    /// Hands out a single <see cref="CapturingTextWriter"/> so output-stream writes stay in memory.
    /// </summary>
    public class CapturingTextWriterFactory : ITextWriterFactory
    {
        public CapturingTextWriter Writer { get; } = new CapturingTextWriter();

        public ITextWriter Create(string path, bool append = false) => Writer;
    }
}
