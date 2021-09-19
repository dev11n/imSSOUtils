using System;
using System.Text;

namespace CNLibrary
{
    /// <summary>
    /// Memory manipulation structure
    /// </summary>
    public struct Memory
    {
        #region Write
        /// <summary>
        /// Write bytes to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        public void write_bytes(string address, byte[] write) =>
            Internals.WriteProcessMemory(Consult.currentHandle, Utils.get_real_address(address), write,
                (UIntPtr) write.Length, IntPtr.Zero);

        /// <summary>
        /// Write bytes to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        public void write_bytes(UIntPtr address, byte[] write) => Internals.WriteProcessMemory(Consult.currentHandle,
            address, write, (UIntPtr) write.Length, IntPtr.Zero);
        
        /// <summary>
        /// Write bytes to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        /// <param name="separator">Character used to separate bytes</param>
        public void write_bytes(string address, string write, char separator)
        {
            var fixedWrite = Utils.get_bytes(write, separator);
            Internals.WriteProcessMemory(Consult.currentHandle, Utils.get_real_address(address), fixedWrite,
                (UIntPtr) fixedWrite.Length, IntPtr.Zero);
        }

        /// <summary>
        /// Write a float value to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        public void write_float(string address, float write) => Internals.WriteProcessMemory(Consult.currentHandle,
            Utils.get_real_address(address), BitConverter.GetBytes(write), (UIntPtr) sizeof(float), IntPtr.Zero);

        /// <summary>
        /// Write a float value to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        public void write_float(UIntPtr address, float write) => Internals.WriteProcessMemory(Consult.currentHandle,
            address, BitConverter.GetBytes(write), (UIntPtr) sizeof(float), IntPtr.Zero);

        /// <summary>
        /// Write a int value to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        public void write_int(string address, int write) => Internals.WriteProcessMemory(Consult.currentHandle,
            Utils.get_real_address(address), BitConverter.GetBytes(Convert.ToInt32(write)), (UIntPtr) sizeof(int),
            IntPtr.Zero);

        /// <summary>
        /// Write a int value to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        public void write_int(UIntPtr address, int write) => Internals.WriteProcessMemory(Consult.currentHandle,
            address, BitConverter.GetBytes(Convert.ToInt32(write)), (UIntPtr) sizeof(int), IntPtr.Zero);

        /// <summary>
        /// Write a long value to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        public void write_long(string address, int write) => Internals.WriteProcessMemory(Consult.currentHandle,
            Utils.get_real_address(address), BitConverter.GetBytes(Convert.ToInt64(write)), (UIntPtr) sizeof(long),
            IntPtr.Zero);

        /// <summary>
        /// Write a long value to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        public void write_long(UIntPtr address, int write) => Internals.WriteProcessMemory(Consult.currentHandle,
            address, BitConverter.GetBytes(Convert.ToInt64(write)), (UIntPtr) sizeof(long), IntPtr.Zero);

        /// <summary>
        /// Write a double value to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        public void write_double(string address, double write) => Internals.WriteProcessMemory(Consult.currentHandle,
            Utils.get_real_address(address), BitConverter.GetBytes(Convert.ToDouble(write)), (UIntPtr) sizeof(double),
            IntPtr.Zero);

        /// <summary>
        /// Write a double value to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The value to be written</param>
        public void write_double(UIntPtr address, double write) => Internals.WriteProcessMemory(Consult.currentHandle,
            address, BitConverter.GetBytes(Convert.ToDouble(write)), (UIntPtr) sizeof(double), IntPtr.Zero);

        /// <summary>
        /// Write a string to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The string to be written</param>
        /// <param name="useDirty">Use a dirty fix for reducing overlapping issues</param>
        public void write_string(string address, string write, bool useDirty = true)
        {
            var bytes = Encoding.UTF8.GetBytes(write);
            // ? This simple line below fixes most string overlapping issues.
            // ? May not be pretty, but it works.
            if (useDirty) Array.Resize(ref bytes, write.Length * 2);
            Internals.WriteProcessMemory(Consult.currentHandle, Utils.get_real_address(address), bytes,
                (UIntPtr) bytes.Length, IntPtr.Zero);
        }

        /// <summary>
        /// Write a string to the specified address.
        /// </summary>
        /// <param name="address">The address to write to</param>
        /// <param name="write">The string to be written</param>
        /// <param name="useDirty">Use a dirty fix for reducing overlapping issues</param>
        public void write_string(UIntPtr address, string write, bool useDirty = true)
        {
            var bytes = Encoding.UTF8.GetBytes(write);
            // ? This simple line below fixes most string overlapping issues.
            // ? May not be pretty, but it works.
            if (useDirty) Array.Resize(ref bytes, write.Length * 2);
            Internals.WriteProcessMemory(Consult.currentHandle, address, bytes,
                (UIntPtr) bytes.Length, IntPtr.Zero);
        }
        #endregion
        #region Read
        /// <summary>
        /// Read a string.
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="length">How many chars of the string that should be read</param>
        /// <returns></returns>
        public string read_string(string address, int length = 64)
        {
            var buffer = new byte[length];
            return Internals.ReadProcessMemory(Consult.currentHandle, Utils.get_real_address(address), buffer, length,
                out _)
                ? Encoding.UTF8.GetString(buffer).Split('\0')[0]
                : string.Empty;
        }

        /// <summary>
        /// Read a string.
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="length">How many chars of the string that should be read</param>
        /// <returns></returns>
        public string read_string(UIntPtr address, int length = 64)
        {
            var buffer = new byte[length];
            return Internals.ReadProcessMemory(Consult.currentHandle, address, buffer, length,
                out _)
                ? Encoding.UTF8.GetString(buffer).Split('\0')[0]
                : string.Empty;
        }

        /// <summary>
        /// Read a byte-array.
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="length">How many bytes that should be read</param>
        /// <returns></returns>
        public byte[] read_bytes(string address, long length)
        {
            var buffer = new byte[length];
            return Internals.ReadProcessMemory(Consult.currentHandle, Utils.get_real_address(address), buffer, length,
                out _)
                ? buffer
                : null;
        }

        /// <summary>
        /// Read a byte-array.
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="length">How many bytes that should be read</param>
        /// <returns></returns>
        public byte[] read_bytes(UIntPtr address, long length)
        {
            var buffer = new byte[length];
            return Internals.ReadProcessMemory(Consult.currentHandle, address, buffer, length,
                out _)
                ? buffer
                : null;
        }

        /// <summary>
        /// Read a int.
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns></returns>
        public int read_int(string address)
        {
            var buffer = new byte[sizeof(int)];
            return Internals.ReadProcessMemory(Consult.currentHandle, Utils.get_real_address(address), buffer,
                sizeof(int), out _)
                ? BitConverter.ToInt32(buffer)
                : 0;
        }

        /// <summary>
        /// Read a int.
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns></returns>
        public int read_int(UIntPtr address)
        {
            var buffer = new byte[sizeof(int)];
            return Internals.ReadProcessMemory(Consult.currentHandle, address, buffer,
                sizeof(int), out _)
                ? BitConverter.ToInt32(buffer)
                : 0;
        }

        /// <summary>
        /// Read a long.
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns></returns>
        public long read_long(string address)
        {
            var buffer = new byte[sizeof(long)];
            return Internals.ReadProcessMemory(Consult.currentHandle, Utils.get_real_address(address), buffer,
                sizeof(long), out _)
                ? BitConverter.ToInt64(buffer)
                : 0;
        }

        /// <summary>
        /// Read a long.
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns></returns>
        public long read_long(UIntPtr address)
        {
            var buffer = new byte[sizeof(long)];
            return Internals.ReadProcessMemory(Consult.currentHandle, address, buffer,
                sizeof(long), out _)
                ? BitConverter.ToInt64(buffer)
                : 0;
        }

        /// <summary>
        /// Read a byte.
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns></returns>
        public byte read_byte(string address)
        {
            var buffer = new byte[sizeof(byte)];
            return Internals.ReadProcessMemory(Consult.currentHandle, Utils.get_real_address(address), buffer,
                sizeof(byte), out _)
                ? buffer[0]
                : (byte) 0;
        }

        /// <summary>
        /// Read a byte.
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns></returns>
        public byte read_byte(UIntPtr address)
        {
            var buffer = new byte[sizeof(byte)];
            return Internals.ReadProcessMemory(Consult.currentHandle, address, buffer,
                sizeof(byte), out _)
                ? buffer[0]
                : (byte) 0;
        }

        /// <summary>
        /// Read a float.
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="roundValue">Round down the value to 2 decimal places (0.00)</param>
        /// <returns></returns>
        public float read_float(string address, bool roundValue = true)
        {
            var buffer = new byte[sizeof(float)];
            return Internals.ReadProcessMemory(Consult.currentHandle, Utils.get_real_address(address), buffer,
                sizeof(float),
                out _)
                ? roundValue ? (float) Math.Round(BitConverter.ToSingle(buffer), 2) : BitConverter.ToSingle(buffer)
                : 0;
        }

        /// <summary>
        /// Read a float.
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="roundValue">Round down the value to 2 decimal places (0.00)</param>
        /// <returns></returns>
        public float read_float(UIntPtr address, bool roundValue = true)
        {
            var buffer = new byte[sizeof(float)];
            return Internals.ReadProcessMemory(Consult.currentHandle, address, buffer,
                sizeof(float),
                out _)
                ? roundValue ? (float) Math.Round(BitConverter.ToSingle(buffer), 2) : BitConverter.ToSingle(buffer)
                : 0;
        }
        #endregion
    }
}