using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace VncLibrary
{
    public static class NetworkStreamExtensions
    {
        public static async Task<int> ReadAllAsync(this NetworkStream a_stream, byte[] a_buffer, int a_offset, int a_size)
        {
            await Task.Run(() =>
            {
                int remains = a_size;
                int currentOffset = a_offset;
                while (remains != 0)
                {
                    if (a_stream.DataAvailable)
                    {
                        int readSize = a_stream.Read(a_buffer, currentOffset, remains);
                        remains -= readSize;
                        currentOffset += readSize;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            });

            return a_size;
        }
    }
}
