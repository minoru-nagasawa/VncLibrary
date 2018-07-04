using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace VncLibrary
{
    public static class StreamExtensions
    {
        public static int ReadAll(this Stream a_stream, byte[] a_buffer, int a_offset, int a_size)
        {
            int remains       = a_size;
            int currentOffset = a_offset;
            while (remains != 0)
            {
                int readSize = a_stream.Read(a_buffer, currentOffset, remains);
                remains       -= readSize;
                currentOffset += readSize;
            }
            return a_size;
        }
    }
}
