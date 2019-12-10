using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CosmosManager.Utilities
{
    public interface IHashProvider
    {
        uint Create(string data);

        uint Create<T>(IEnumerable<T> byteStream);
    }

    //https://stackoverflow.com/questions/8128/how-do-i-calculate-crc32-of-a-string
    public class Crc32HashProvider : IHashProvider
    {
        /// <summary>
        /// Contains a cache of calculated checksum chunks.
        /// </summary>
        private readonly UInt32[] m_checksumTable;

        /// <summary>
        /// Generator polynomial (modulo 2) for the reversed CRC32 algorithm.
        /// </summary>
        private const UInt32 s_generator = 0xEDB88320;

        /// <summary>
        /// Creates a new instance of the Crc32 class.
        /// </summary>
        public Crc32HashProvider()
        {
            // Constructs the checksum lookup table. Used to optimize the checksum.
            m_checksumTable = Enumerable.Range(0, 256).Select(i =>
            {
                var tableEntry = (uint)i;
                for (var j = 0; j < 8; ++j)
                {
                    tableEntry = ((tableEntry & 1) != 0)
                        ? (s_generator ^ (tableEntry >> 1))
                        : (tableEntry >> 1);
                }
                return tableEntry;
            }).ToArray();
        }

        public UInt32 Create(string data)
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            return Create(bytes);
        }

        /// <summary>
        /// Calculates the checksum of the byte stream.
        /// </summary>
        /// <param name="byteStream">The byte stream to calculate the checksum for.</param>
        /// <returns>A 32-bit reversed checksum.</returns>
        public UInt32 Create<T>(IEnumerable<T> byteStream)
        {
            if (byteStream == null)
            {
                throw new ArgumentNullException(nameof(byteStream));
            }

            try
            {
                // Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
                return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
                          (m_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
            }
            catch (FormatException e)
            {
                throw new Exception("Could not read the stream out as bytes.", e);
            }
            catch (InvalidCastException e)
            {
                throw new Exception("Could not read the stream out as bytes.", e);
            }
            catch (OverflowException e)
            {
                throw new Exception("Could not read the stream out as bytes.", e);
            }
        }
    }
}