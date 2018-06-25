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
        public static async Task<int> ReadAllAsync(this Stream a_stream, byte[] a_buffer, int a_offset, int a_size)
        {
            int remains = a_size;
            int currentOffset = a_offset;
            using (var signal = new SemaphoreSlim(0, 1))
            {
                AsyncCallback callback = null;
                callback = (ar) =>
                {
                    int readSize = a_stream.EndRead(ar);
                    remains       -= readSize;
                    currentOffset += readSize;
                    if (remains == 0)
                    {
                        signal.Release();
                    }
                    else
                    {
                        a_stream.BeginRead(a_buffer, currentOffset, remains, callback, a_stream);
                    }
                };

                a_stream.BeginRead(a_buffer, currentOffset, remains, callback, a_stream);
                await signal.WaitAsync();
            }
            return a_size;
        }
    }
}
