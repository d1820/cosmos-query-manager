using CosmosManager.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CosmosManager.Utilities
{
    public class TextWriter : ITextWriter
    {
        private StreamWriter _sw;

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

        public ITextWriter Open(string path, bool append = false)
        {
            if(_sw != null)
            {
                return this;
            }
            _sw =  new StreamWriter(path, append);
            return this;
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