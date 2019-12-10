using CosmosManager.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CosmosManager.Utilities
{
    public class TextWriterFactory : ITextWriterFactory
    {
        public ITextWriter Create(string path, bool append = false)
        {
            return new TextWriter(path, append);
        }
    }
    public class TextWriter : ITextWriter
    {
        private StreamWriter _sw;

        public TextWriter(string path, bool append = false)
        {
            _sw = new StreamWriter(path, append);
        }

        public async Task WriteAsync(string value)
        {
            if(_sw != null)
            {
                await _sw.WriteAsync(value);
                return;
            }
            throw new NullReferenceException("TextWriter not open");
        }

        public void WriteLine(string value)
        {
            if (_sw != null)
            {
                _sw.WriteLine(value);
                return;
            }
            throw new NullReferenceException("TextWriter not open");
        }

        public void Close()
        {
            if (_sw != null && _sw.BaseStream != null)
            {
                _sw.Flush();
                _sw.Close();
                _sw = null;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}