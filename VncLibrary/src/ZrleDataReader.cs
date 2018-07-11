using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace VncLibrary
{
    public class ZrleDataReader : IDisposable
    {
        private bool          m_first;
        private MemoryStream  m_memoryStream;
        private DeflateStream m_deflateStream;

        #region IDisposable Support
        private bool m_disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposedValue)
            {
                if (disposing)
                {
                    m_memoryStream?.Dispose();  m_memoryStream  = null;
                    m_deflateStream?.Dispose(); m_deflateStream = null;
                }
                m_disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public ZrleDataReader()
        {
            m_first         = true;
            m_memoryStream  = new MemoryStream();
            m_deflateStream = new DeflateStream(m_memoryStream, CompressionMode.Decompress);
        }

        public byte[] Read(byte[] a_buffer, int a_offset, int a_count)
        {
            // DeflateStream is RFC1951.
            // VNC use RFC1950.
            // Excluding the first 2 bytes (CMF + FLG) and the last 4 bytes (ADLER-32 checksum) from RFC 1950, it is equal to RFC 1951.
            // If the head is 0x78, it is CMF so skip 2 bytes and read it.
            // However, since the head of the data may be 0x78, skip only the first time.
            // Also, since the stream continues, there is no checksum.
            int skipHeader = 0;
            if (m_first)
            {
                m_first = false;
                if (a_buffer[a_offset] == 0x78)
                {
                    skipHeader = 2;
                }
            }
            m_memoryStream.SetLength(0);
            m_memoryStream.Write(a_buffer, a_offset + skipHeader, a_count - skipHeader);
            m_memoryStream.Flush();
            m_memoryStream.Seek(0, SeekOrigin.Begin);
            using (var mem = new MemoryStream())
            {
                m_deflateStream.CopyTo(mem);
                return mem.ToArray();
            }
        }
    }
}
