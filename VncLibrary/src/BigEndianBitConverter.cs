using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public static class BigEndianBitConverter
    {
        public static byte[] GetBytes(bool a_value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var retVal = BitConverter.GetBytes(a_value);
                Array.Reverse(retVal);
                return retVal;
            }
            else
            {
                return BitConverter.GetBytes(a_value);
            }
        }

        public static byte[] GetBytes(char a_value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var retVal = BitConverter.GetBytes(a_value);
                Array.Reverse(retVal);
                return retVal;
            }
            else
            {
                return BitConverter.GetBytes(a_value);
            }
        }

        public static byte[] GetBytes(double a_value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var retVal = BitConverter.GetBytes(a_value);
                Array.Reverse(retVal);
                return retVal;
            }
            else
            {
                return BitConverter.GetBytes(a_value);
            }
        }

        public static byte[] GetBytes(short a_value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var retVal = BitConverter.GetBytes(a_value);
                Array.Reverse(retVal);
                return retVal;
            }
            else
            {
                return BitConverter.GetBytes(a_value);
            }
        }

        public static byte[] GetBytes(int a_value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var retVal = BitConverter.GetBytes(a_value);
                Array.Reverse(retVal);
                return retVal;
            }
            else
            {
                return BitConverter.GetBytes(a_value);
            }
        }

        public static byte[] GetBytes(long a_value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var retVal = BitConverter.GetBytes(a_value);
                Array.Reverse(retVal);
                return retVal;
            }
            else
            {
                return BitConverter.GetBytes(a_value);
            }
        }

        public static byte[] GetBytes(float a_value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var retVal = BitConverter.GetBytes(a_value);
                Array.Reverse(retVal);
                return retVal;
            }
            else
            {
                return BitConverter.GetBytes(a_value);
            }
        }

        public static byte[] GetBytes(ushort a_value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var retVal = BitConverter.GetBytes(a_value);
                Array.Reverse(retVal);
                return retVal;
            }
            else
            {
                return BitConverter.GetBytes(a_value);
            }
        }

        public static byte[] GetBytes(uint a_value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var retVal = BitConverter.GetBytes(a_value);
                Array.Reverse(retVal);
                return retVal;
            }
            else
            {
                return BitConverter.GetBytes(a_value);
            }
        }

        public static byte[] GetBytes(ulong a_value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var retVal = BitConverter.GetBytes(a_value);
                Array.Reverse(retVal);
                return retVal;
            }
            else
            {
                return BitConverter.GetBytes(a_value);
            }
        }

        public static bool ToBoolean(byte[] a_value, int a_startIndex)
        {
            // ToBoolean use 1bit. So there is no difference between Bigendian and Littleendian.
            return BitConverter.ToBoolean(a_value, a_startIndex);
        }

        public static char ToChar(byte[] a_value, int a_startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                var array = new byte[sizeof(char)];
                Array.Copy(a_value, a_startIndex, array, 0, array.Length);
                Array.Reverse(array);
                return BitConverter.ToChar(array, 0);
            }
            else
            {
                return BitConverter.ToChar(a_value, a_startIndex);
            }
        }

        public static double ToDouble(byte[] a_value, int a_startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                var array = new byte[sizeof(double)];
                Array.Copy(a_value, a_startIndex, array, 0, array.Length);
                Array.Reverse(array);
                return BitConverter.ToDouble(array, 0);
            }
            else
            {
                return BitConverter.ToDouble(a_value, a_startIndex);
            }
        }

        public static short ToInt16(byte[] a_value, int a_startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                var array = new byte[sizeof(Int16)];
                Array.Copy(a_value, a_startIndex, array, 0, array.Length);
                Array.Reverse(array);
                return BitConverter.ToInt16(array, 0);
            }
            else
            {
                return BitConverter.ToInt16(a_value, a_startIndex);
            }
        }

        public static int ToInt32(byte[] a_value, int a_startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                var array = new byte[sizeof(Int32)];
                Array.Copy(a_value, a_startIndex, array, 0, array.Length);
                Array.Reverse(array);
                return BitConverter.ToInt32(array, 0);
            }
            else
            {
                return BitConverter.ToInt32(a_value, a_startIndex);
            }
        }

        public static long ToInt64(byte[] a_value, int a_startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                var array = new byte[sizeof(Int64)];
                Array.Copy(a_value, a_startIndex, array, 0, array.Length);
                Array.Reverse(array);
                return BitConverter.ToInt64(array, 0);
            }
            else
            {
                return BitConverter.ToInt64(a_value, a_startIndex);
            }
        }

        public static float ToSingle(byte[] a_value, int a_startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                var array = new byte[sizeof(Single)];
                Array.Copy(a_value, a_startIndex, array, 0, array.Length);
                Array.Reverse(array);
                return BitConverter.ToSingle(array, 0);
            }
            else
            {
                return BitConverter.ToSingle(a_value, a_startIndex);
            }
        }

        public static string ToString(byte[] a_value)
        {
            // ToBoolean use each 1bit. So there is no difference between Bigendian and Littleendian.
            return BitConverter.ToString(a_value);
        }

        public static string ToString(byte[] a_value, int a_startIndex)
        {
            // ToBoolean use each 1bit. So there is no difference between Bigendian and Littleendian.
            return BitConverter.ToString(a_value, a_startIndex);
        }

        public static string ToString(byte[] a_value, int a_startIndex, int a_length)
        {
            // ToBoolean use each 1bit. So there is no difference between Bigendian and Littleendian.
            return BitConverter.ToString(a_value, a_startIndex, a_length);
        }

        public static ushort ToUInt16(byte[] a_value, int a_startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                var array = new byte[sizeof(UInt16)];
                Array.Copy(a_value, a_startIndex, array, 0, array.Length);
                Array.Reverse(array);
                return BitConverter.ToUInt16(array, 0);
            }
            else
            {
                return BitConverter.ToUInt16(a_value, a_startIndex);
            }
        }

        public static uint ToUInt32(byte[] a_value, int a_startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                var array = new byte[sizeof(UInt32)];
                Array.Copy(a_value, a_startIndex, array, 0, array.Length);
                Array.Reverse(array);
                return BitConverter.ToUInt32(array, 0);
            }
            else
            {
                return BitConverter.ToUInt32(a_value, a_startIndex);
            }
        }

        public static ulong ToUInt64(byte[] a_value, int a_startIndex)
        {
            if (BitConverter.IsLittleEndian)
            {
                var array = new byte[sizeof(UInt64)];
                Array.Copy(a_value, a_startIndex, array, 0, array.Length);
                Array.Reverse(array);
                return BitConverter.ToUInt64(array, 0);
            }
            else
            {
                return BitConverter.ToUInt64(a_value, a_startIndex);
            }
        }
    }
}
