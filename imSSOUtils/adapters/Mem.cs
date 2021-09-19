using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CNLibrary;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Memory.dll class. Full documentation at https://github.com/erfg12/memory.dll/wiki
    /// </summary>
    public class Mem
    {
        #region DllImports
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] IntPtr lpBuffer,
            UIntPtr nSize, out ulong lpNumberOfBytesRead);

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        private static extern UIntPtr Native_VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress,
            out MEMORY_BASIC_INFORMATION32 lpBuffer, UIntPtr dwLength);

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        private static extern UIntPtr Native_VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress,
            out MEMORY_BASIC_INFORMATION64 lpBuffer, UIntPtr dwLength);

        private UIntPtr VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer)
        {
            UIntPtr retVal;

            if (Is64Bit || IntPtr.Size == 8)
            {
                // 64 bit
                var tmp64 = new MEMORY_BASIC_INFORMATION64();
                retVal = Native_VirtualQueryEx(hProcess, lpAddress, out tmp64,
                    new UIntPtr((uint) Marshal.SizeOf(tmp64)));

                lpBuffer.BaseAddress = tmp64.BaseAddress;
                lpBuffer.AllocationBase = tmp64.AllocationBase;
                lpBuffer.AllocationProtect = tmp64.AllocationProtect;
                lpBuffer.RegionSize = (long) tmp64.RegionSize;
                lpBuffer.State = tmp64.State;
                lpBuffer.Protect = tmp64.Protect;
                lpBuffer.Type = tmp64.Type;
                return retVal;
            }

            var tmp32 = new MEMORY_BASIC_INFORMATION32();
            retVal = Native_VirtualQueryEx(hProcess, lpAddress, out tmp32, new UIntPtr((uint) Marshal.SizeOf(tmp32)));
            lpBuffer.BaseAddress = tmp32.BaseAddress;
            lpBuffer.AllocationBase = tmp32.AllocationBase;
            lpBuffer.AllocationProtect = tmp32.AllocationProtect;
            lpBuffer.RegionSize = tmp32.RegionSize;
            lpBuffer.State = tmp32.State;
            lpBuffer.Protect = tmp32.Protect;
            lpBuffer.Type = tmp32.Type;
            return retVal;
        }

        [DllImport("kernel32.dll")] private static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            UIntPtr nSize, IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")] private static extern bool VirtualProtectEx(IntPtr hProcess, UIntPtr lpAddress,
            IntPtr dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer,
            UIntPtr nSize, IntPtr lpNumberOfBytesWritten);

        // Added to avoid casting to UIntPtr
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer,
            UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32")] private static extern bool IsWow64Process(IntPtr hProcess, out bool lpSystemInfo);

        private const uint MEM_FREE = 0x10000,
            MEM_COMMIT = 0x00001000,
            MEM_RESERVE = 0x00002000,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_GUARD = 0x100,
            PAGE_NOACCESS = 0x01;

        private const uint MEM_PRIVATE = 0x20000, MEM_IMAGE = 0x1000000;
        #endregion

        /// <summary>
        /// The process handle that was opened. (Use OpenProcess function to populate this variable)
        /// </summary>
        private IntPtr pHandle;

        private readonly Dictionary<string, CancellationTokenSource> FreezeTokenSrcs = new();
        private Process theProc;

        /// <summary>
        /// Open the PC game process with all security and access rights.
        /// </summary>
        /// <param name="pid">Use process name or process ID here.</param>
        /// <returns></returns>
        private bool OpenProcess(int pid)
        {
            if (!IsAdmin())
                Debug.WriteLine(
                    "WARNING: This program may not be running with raised privileges! Visit https://github.com/erfg12/memory.dll/wiki/Administrative-Privileges");

            if (pid <= 0)
            {
                Debug.WriteLine("ERROR: OpenProcess given proc ID 0.");
                return false;
            }

            if (theProc is not null && theProc.Id == pid) return true;
            try
            {
                theProc = Process.GetProcessById(pid);

                if (theProc is {Responding: false})
                {
                    Debug.WriteLine("ERROR: OpenProcess: Process is not responding or null.");
                    return false;
                }

                pHandle = OpenProcess(0x1F0FFF, true, pid);

                try
                {
                    Process.EnterDebugMode();
                }
                catch (Win32Exception) { }

                if (pHandle == IntPtr.Zero)
                {
                    var eCode = Marshal.GetLastWin32Error();
                    Debug.WriteLine(
                        $"ERROR: OpenProcess has failed opening a handle to the target process (GetLastWin32ErrorCode: {eCode})");
                    Process.LeaveDebugMode();
                    theProc = null;
                    return false;
                }

                // Lets set the process to 64bit or not here (cuts down on api calls)
                Is64Bit = Environment.Is64BitOperatingSystem && IsWow64Process(pHandle, out var retVal) && !retVal;
                mainModule = theProc.MainModule;
                GetModules();
                Debug.WriteLine($"Process #{theProc} is now open.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: OpenProcess has crashed. {ex}");
                return false;
            }
        }

        /// <summary>
        /// Open the PC game process with all security and access rights.
        /// </summary>
        /// <param name="proc">Use process name or process ID here.</param>
        /// <returns></returns>
        public bool OpenProcess(string proc) => OpenProcess(GetProcIdFromName(proc));

        /// <summary>
        /// Check if program is running with administrative privileges. Read about it here: https://github.com/erfg12/memory.dll/wiki/Administrative-Privileges
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            try
            {
                #pragma warning disable CA1416
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
                #pragma warning restore CA1416
            }
            catch
            {
                Debug.WriteLine(
                    "ERROR: Could not determin if program is running as admin. Is the NuGet package \"System.Security.Principal.Windows\" missing?");
                return false;
            }
        }

        /// <summary>
        /// Check if opened process is 64bit. Used primarily for Utils.get_real_address().
        /// </summary>
        /// <returns>True if 64bit false if 32bit.</returns>
        private bool _is64Bit;

        private bool Is64Bit
        {
            get => _is64Bit;
            set => _is64Bit = value;
        }

        /// <summary>
        /// Builds the process modules dictionary (names with addresses).
        /// </summary>
        private void GetModules()
        {
            if (theProc is null) return;
            switch (_is64Bit)
            {
                case true when IntPtr.Size != 8:
                    Debug.WriteLine(
                        "WARNING: Game is x64, but your Trainer is x86! You will be missing some modules, change your Trainer's Solution Platform.");
                    break;
                case false when IntPtr.Size == 8:
                    Debug.WriteLine(
                        "WARNING: Game is x86, but your Trainer is x64! You will be missing some modules, change your Trainer's Solution Platform.");
                    break;
            }

            modules.Clear();

            foreach (ProcessModule Module in theProc.Modules)
            {
                if (!string.IsNullOrEmpty(Module.ModuleName) && !modules.ContainsKey(Module.ModuleName))
                    modules.Add(Module.ModuleName, Module.BaseAddress);
            }

            Debug.WriteLine($"Found {modules.Count()} process modules.");
        }

        /// <summary>
        /// Get the process ID number by process name.
        /// </summary>
        /// <param name="name">Example: "eqgame". Use task manager to find the name. Do not include .exe</param>
        /// <returns></returns>
        public int GetProcIdFromName(string name)
        {
            var processlist = Process.GetProcesses();

            if (name.ToLower().Contains(".exe"))
                name = name.Replace(".exe", string.Empty);
            if (name.ToLower().Contains(".bin")) // test
                name = name.Replace(".bin", string.Empty);

            foreach (var proc in processlist)
                if (proc.ProcessName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return proc.Id;

            return 0; //if we fail to find it
        }

        public string LoadCode(string name) => new StringBuilder(1024).Append(name).ToString();

        private int LoadIntCode(string name)
        {
            try
            {
                int intValue = Convert.ToInt32(LoadCode(name), 16);
                if (intValue >= 0)
                    return intValue;
                return 0;
            }
            catch
            {
                Debug.WriteLine("ERROR: LoadIntCode function crashed!");
                return 0;
            }
        }

        /// <summary>
        /// Dictionary with our opened process module names with addresses.
        /// </summary>
        public readonly Dictionary<string, IntPtr> modules = new();

        private ProcessModule mainModule;

        /// <summary>
        /// Cut a string that goes on for too long or one that is possibly merged with another string.
        /// </summary>
        /// <param name="str">The string you want to cut.</param>
        /// <returns></returns>
        public string CutString(string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if (c is >= ' ' and <= '~') sb.Append(c);
                else break;
            }

            return sb.ToString();
        }

        #region protection
        [Flags]
        public enum MemoryProtection : uint
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        public bool ChangeProtection(string code, MemoryProtection newProtection, out MemoryProtection oldProtection)
        {
            var theCode = Utils.get_real_address(code);
            if (theCode != UIntPtr.Zero && pHandle != IntPtr.Zero)
                return VirtualProtectEx(pHandle, theCode, (IntPtr) (Is64Bit ? 8 : 4), newProtection, out oldProtection);
            oldProtection = default;
            return false;
        }
        #endregion

        #region readMemory
        /// <summary>
        /// Reads up to `length ` bytes from an address.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <param name="length">The maximum bytes to read.</param>
        /// <returns>The bytes read or null</returns>
        public byte[] ReadBytes(string code, long length)
        {
            var memory = new byte[length];
            var theCode = Utils.get_real_address(code);
            return !ReadProcessMemory(pHandle, theCode, memory, (UIntPtr) length, IntPtr.Zero) ? null : memory;
        }

        /// <summary>
        /// Read a float value from an address.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <param name="round">Round the value to 2 decimal places</param>
        /// <returns></returns>
        public float ReadFloat(string code, bool round = true)
        {
            var memory = new byte[4];

            var theCode = Utils.get_real_address(code);
            try
            {
                if (!ReadProcessMemory(pHandle, theCode, memory, (UIntPtr) 4, IntPtr.Zero)) return 0;
                var address = BitConverter.ToSingle(memory, 0);
                var returnValue = address;
                if (round) returnValue = (float) Math.Round(address, 2);
                return returnValue;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Read a string value from an address.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <param name="length">length of bytes to read (OPTIONAL)</param>
        /// <param name="zeroTerminated">terminate string at null char</param>
        /// <param name="stringEncoding">System.Text.Encoding.UTF8 (DEFAULT). Other options: ascii, unicode, utf32, utf7</param>
        /// <returns></returns>
        public string ReadString(string code, int length = 32, bool zeroTerminated = true,
            Encoding stringEncoding = null)
        {
            if (stringEncoding == null)
                stringEncoding = Encoding.UTF8;

            var memoryNormal = new byte[length];
            var theCode = Utils.get_real_address(code);

            if (ReadProcessMemory(pHandle, theCode, memoryNormal, (UIntPtr) length, IntPtr.Zero))
                return (zeroTerminated)
                    ? stringEncoding.GetString(memoryNormal).Split('\0')[0]
                    : stringEncoding.GetString(memoryNormal);
            return string.Empty;
        }

        /// <summary>
        /// Read a double value
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <param name="round">Round the value to 2 decimal places</param>
        /// <returns></returns>
        public double ReadDouble(string code, bool round = true)
        {
            var memory = new byte[8];
            var theCode = Utils.get_real_address(code);
            try
            {
                if (!ReadProcessMemory(pHandle, theCode, memory, (UIntPtr) 8, IntPtr.Zero)) return 0;
                var address = BitConverter.ToDouble(memory, 0);
                var returnValue = address;
                if (round)
                    returnValue = Math.Round(address, 2);
                return returnValue;
            }
            catch
            {
                return 0;
            }
        }

        public int ReadUIntPtr(UIntPtr code)
        {
            var memory = new byte[4];
            return ReadProcessMemory(pHandle, code, memory, (UIntPtr) 4, IntPtr.Zero)
                ? BitConverter.ToInt32(memory, 0)
                : 0;
        }

        /// <summary>
        /// Read an integer from an address.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <returns></returns>
        public int ReadInt(string code)
        {
            var memory = new byte[4];
            var theCode = Utils.get_real_address(code);
            return ReadProcessMemory(pHandle, theCode, memory, (UIntPtr) 4, IntPtr.Zero)
                ? BitConverter.ToInt32(memory, 0)
                : 0;
        }

        /// <summary>
        /// Read a long value from an address.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <returns></returns>
        public long ReadLong(string code)
        {
            var memory = new byte[16];
            var theCode = Utils.get_real_address(code);
            return ReadProcessMemory(pHandle, theCode, memory, (UIntPtr) 8, IntPtr.Zero)
                ? BitConverter.ToInt64(memory, 0)
                : 0;
        }

        /// <summary>
        /// Read a UInt value from address.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <returns></returns>
        public uint ReadUInt(string code)
        {
            var memory = new byte[4];
            var theCode = Utils.get_real_address(code);
            return ReadProcessMemory(pHandle, theCode, memory, (UIntPtr) 4, IntPtr.Zero)
                ? BitConverter.ToUInt32(memory, 0)
                : (uint) 0;
        }

        /// <summary>
        /// Reads a 2 byte value from an address and moves the address.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <param name="moveQty">Quantity to move.</param>
        /// <returns></returns>
        public int Read2ByteMove(string code, int moveQty)
        {
            var memory = new byte[4];
            var theCode = Utils.get_real_address(code);
            var newCode = UIntPtr.Add(theCode, moveQty);
            return ReadProcessMemory(pHandle, newCode, memory, (UIntPtr) 2, IntPtr.Zero)
                ? BitConverter.ToInt32(memory, 0)
                : 0;
        }

        /// <summary>
        /// Reads an integer value from address and moves the address.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <param name="moveQty">Quantity to move.</param>
        /// <returns></returns>
        public int ReadIntMove(string code, int moveQty)
        {
            var memory = new byte[4];
            var theCode = Utils.get_real_address(code);
            var newCode = UIntPtr.Add(theCode, moveQty);
            return ReadProcessMemory(pHandle, newCode, memory, (UIntPtr) 4, IntPtr.Zero)
                ? BitConverter.ToInt32(memory, 0)
                : 0;
        }

        /// <summary>
        /// Get UInt and move to another address by moveQty. Use in a for loop.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <param name="moveQty">Quantity to move.</param>
        /// <param name="file">path and name of ini file (OPTIONAL)</param>
        /// <returns></returns>
        public ulong ReadUIntMove(string code, int moveQty)
        {
            var memory = new byte[8];
            var theCode = Utils.get_real_address(code);
            var newCode = UIntPtr.Add(theCode, moveQty);
            return ReadProcessMemory(pHandle, newCode, memory, (UIntPtr) 8, IntPtr.Zero)
                ? BitConverter.ToUInt64(memory, 0)
                : (ulong) 0;
        }

        /// <summary>
        /// Read a 2 byte value from an address. Returns an integer.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <returns></returns>
        public int Read2Byte(string code)
        {
            var memoryTiny = new byte[4];
            var theCode = Utils.get_real_address(code);
            return ReadProcessMemory(pHandle, theCode, memoryTiny, (UIntPtr) 2, IntPtr.Zero)
                ? BitConverter.ToInt32(memoryTiny, 0)
                : 0;
        }

        /// <summary>
        /// Read 1 byte from address.
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <returns></returns>
        public int ReadByte(string code)
        {
            var memoryTiny = new byte[1];
            var theCode = Utils.get_real_address(code);
            return ReadProcessMemory(pHandle, theCode, memoryTiny, (UIntPtr) 1, IntPtr.Zero) ? memoryTiny[0] : 0;
        }

        /// <summary>
        /// Reads a byte from memory and splits it into bits
        /// </summary>
        /// <param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        /// <returns>Array of 8 booleans representing each bit of the byte read</returns>
        public bool[] ReadBits(string code)
        {
            var buf = new byte[1];
            var theCode = Utils.get_real_address(code);
            var ret = new bool[8];
            if (!ReadProcessMemory(pHandle, theCode, buf, (UIntPtr) 1, IntPtr.Zero)) return ret;
            if (!BitConverter.IsLittleEndian) throw new Exception("Should be little endian");
            for (var i = 0; i < 8; i++) ret[i] = Convert.ToBoolean(buf[0] & (1 << i));
            return ret;
        }

        public int ReadPByte(UIntPtr address, string code)
        {
            var memory = new byte[4];
            return ReadProcessMemory(pHandle, address + LoadIntCode(code), memory, (UIntPtr) 1, IntPtr.Zero)
                ? BitConverter.ToInt32(memory, 0)
                : 0;
        }

        public float ReadPFloat(UIntPtr address, string code)
        {
            var memory = new byte[4];
            if (!ReadProcessMemory(pHandle, address + LoadIntCode(code), memory, (UIntPtr) 4, IntPtr.Zero)) return 0;
            var spawn = BitConverter.ToSingle(memory, 0);
            return (float) Math.Round(spawn, 2);
        }

        public int ReadPInt(UIntPtr address, string code)
        {
            var memory = new byte[4];
            return ReadProcessMemory(pHandle, address + LoadIntCode(code), memory, (UIntPtr) 4, IntPtr.Zero)
                ? BitConverter.ToInt32(memory, 0)
                : 0;
        }

        public string ReadPString(UIntPtr address, string code)
        {
            var memoryNormal = new byte[32];
            return ReadProcessMemory(pHandle, address + LoadIntCode(code), memoryNormal, (UIntPtr) 32, IntPtr.Zero)
                ? CutString(Encoding.ASCII.GetString(memoryNormal))
                : "";
        }
        #endregion

        #region writeMemory
        public void WriteLong(string code, long write, bool RemoveWriteProtection = true)
        {
            var theCode = Utils.get_real_address(code);
            var memory = BitConverter.GetBytes(Convert.ToInt64(write));
            const int size = 8;

            MemoryProtection OldMemProt = 0;
            if (RemoveWriteProtection)
                ChangeProtection(code, MemoryProtection.ExecuteReadWrite, out OldMemProt); // change protection
            WriteProcessMemory(pHandle, theCode, memory, (UIntPtr) size, IntPtr.Zero);
            if (RemoveWriteProtection)
                ChangeProtection(code, OldMemProt, out _); // restore
        }

        public void WriteByte(string code, string write, bool RemoveWriteProtection = true)
        {
            var theCode = Utils.get_real_address(code);
            var memory = new byte[1];
            memory[0] = Convert.ToByte(write, 16);
            const int size = 1;

            MemoryProtection OldMemProt = 0;
            if (RemoveWriteProtection)
                ChangeProtection(code, MemoryProtection.ExecuteReadWrite, out OldMemProt); // change protection
            WriteProcessMemory(pHandle, theCode, memory, (UIntPtr) size, IntPtr.Zero);
            if (RemoveWriteProtection)
                ChangeProtection(code, OldMemProt, out _); // restore
        }

        public void WriteDouble(string code, long write, bool RemoveWriteProtection = true)
        {
            var theCode = Utils.get_real_address(code);
            var memory = BitConverter.GetBytes(Convert.ToDouble(write));
            const int size = 8;

            MemoryProtection OldMemProt = 0;
            if (RemoveWriteProtection)
                ChangeProtection(code, MemoryProtection.ExecuteReadWrite, out OldMemProt); // change protection
            WriteProcessMemory(pHandle, theCode, memory, (UIntPtr) size, IntPtr.Zero);
            if (RemoveWriteProtection)
                ChangeProtection(code, OldMemProt, out _); // restore
        }

        public void WriteInt(string code, int write, bool RemoveWriteProtection = true)
        {
            var theCode = Utils.get_real_address(code);
            var memory = BitConverter.GetBytes(Convert.ToInt32(write));
            const int size = 4;

            MemoryProtection OldMemProt = 0;
            if (RemoveWriteProtection)
                ChangeProtection(code, MemoryProtection.ExecuteReadWrite, out OldMemProt); // change protection
            WriteProcessMemory(pHandle, theCode, memory, (UIntPtr) size, IntPtr.Zero);
            if (RemoveWriteProtection)
                ChangeProtection(code, OldMemProt, out _); // restore
        }

        public void WriteString(string code, string write, Encoding stringEncoding = null,
            bool RemoveWriteProtection = true)
        {
            var theCode = Utils.get_real_address(code);
            var memory = stringEncoding == null ? Encoding.UTF8.GetBytes(write) : stringEncoding.GetBytes(write);

            MemoryProtection OldMemProt = 0;
            if (RemoveWriteProtection)
                ChangeProtection(code, MemoryProtection.ExecuteReadWrite, out OldMemProt); // change protection
            WriteProcessMemory(pHandle, theCode, memory, (UIntPtr) memory.Length, IntPtr.Zero);
            if (RemoveWriteProtection)
                ChangeProtection(code, OldMemProt, out _); // restore
        }

        public void WriteFloat(string code, float write, bool RemoveWriteProtection = true)
        {
            var theCode = Utils.get_real_address(code);
            var memory = BitConverter.GetBytes(write);
            const int size = 4;

            MemoryProtection OldMemProt = 0;
            if (RemoveWriteProtection)
                ChangeProtection(code, MemoryProtection.ExecuteReadWrite, out OldMemProt); // change protection
            WriteProcessMemory(pHandle, theCode, memory, (UIntPtr) size, IntPtr.Zero);
            if (RemoveWriteProtection)
                ChangeProtection(code, OldMemProt, out _); // restore
        }

        /// <summary>
        /// Write to address and move by moveQty. Good for byte arrays. See https://github.com/erfg12/memory.dll/wiki/Writing-a-Byte-Array for more information.
        /// </summary>
        ///<param name="code">address, module + pointer + offset, module + offset OR label in .ini file.</param>
        ///<param name="type">byte, bytes, float, int, string or long.</param>
        /// <param name="write">byte to write</param>
        /// <param name="MoveQty">quantity to move</param>
        /// <param name="SlowDown">milliseconds to sleep between each byte</param>
        /// <returns></returns>
        public bool WriteMove(string code, string type, string write, int MoveQty, int SlowDown = 0)
        {
            var memory = new byte[4];
            var size = 4;
            var theCode = Utils.get_real_address(code);
            switch (type)
            {
                case "float":
                    memory = new byte[write.Length];
                    memory = BitConverter.GetBytes(Convert.ToSingle(write));
                    size = write.Length;
                    break;
                case "int":
                    memory = BitConverter.GetBytes(Convert.ToInt32(write));
                    size = 4;
                    break;
                case "double":
                    memory = BitConverter.GetBytes(Convert.ToDouble(write));
                    size = 8;
                    break;
                case "long":
                    memory = BitConverter.GetBytes(Convert.ToInt64(write));
                    size = 8;
                    break;
                case "byte":
                    memory = new byte[1];
                    memory[0] = Convert.ToByte(write, 16);
                    size = 1;
                    break;
                case "string":
                    memory = new byte[write.Length];
                    memory = Encoding.UTF8.GetBytes(write);
                    size = write.Length;
                    break;
            }

            var newCode = UIntPtr.Add(theCode, MoveQty);
            Thread.Sleep(SlowDown);
            return WriteProcessMemory(pHandle, newCode, memory, (UIntPtr) size, IntPtr.Zero);
        }

        /// <summary>
        /// Write byte array to addresses.
        /// </summary>
        /// <param name="code">address to write to</param>
        /// <param name="write">byte array to write</param>
        public void WriteBytes(string code, byte[] write) =>
            WriteProcessMemory(pHandle, Utils.get_real_address(code), write, (UIntPtr) write.Length, IntPtr.Zero);

        /// <summary>
        /// Takes an array of 8 booleans and writes to a single byte
        /// </summary>
        /// <param name="code">address to write to</param>
        /// <param name="bits">Array of 8 booleans to write</param>
        public void WriteBits(string code, bool[] bits)
        {
            if (bits.Length != 8) throw new ArgumentException("Not enough bits for a whole byte", nameof(bits));
            var buf = new byte[1];
            var theCode = Utils.get_real_address(code);
            for (var i = 0; i < 8; i++)
                if (bits[i])
                    buf[0] |= (byte) (1 << i);
            WriteProcessMemory(pHandle, theCode, buf, (UIntPtr) 1, IntPtr.Zero);
        }

        /// <summary>
        /// Write byte array to address
        /// </summary>
        /// <param name="address">Address to write to</param>
        /// <param name="write">Byte array to write to</param>
        public void WriteBytes(UIntPtr address, byte[] write) =>
            WriteProcessMemory(pHandle, address, write, (UIntPtr) write.Length, out _);
        #endregion

        void AppendAllBytes(string path, byte[] bytes)
        {
            using var stream = new FileStream(path, FileMode.Append);
            stream.Write(bytes, 0, bytes.Length);
        }

        public string MSize() => Is64Bit ? "x16" : "x8";

        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            ushort reserved;
            public uint pageSize;
            public UIntPtr minimumApplicationAddress;
            public UIntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }

        public struct MEMORY_BASIC_INFORMATION32
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public uint RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        public struct MEMORY_BASIC_INFORMATION64
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public uint __alignment1;
            public ulong RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
            public uint __alignment2;
        }

        public struct MEMORY_BASIC_INFORMATION
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public long RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        /// <summary>
        /// Dump memory page by page to a dump.dmp file. Can be used with Cheat Engine.
        /// </summary>
        public bool DumpMemory(string file = "dump.dmp")
        {
            Debug.Write($"[DEBUG] memory dump starting... ({DateTime.Now:h:mm:ss tt}){Environment.NewLine}");
            GetSystemInfo(out var sys_info);
            var proc_min_address = sys_info.minimumApplicationAddress;

            // saving the values as long ints so I won't have to do a lot of casts later
            long proc_min_address_l = (long) proc_min_address,
                proc_max_address_l = theProc.VirtualMemorySize64 + proc_min_address_l;

            //int arrLength = 0;
            if (File.Exists(file)) File.Delete(file);

            while (proc_min_address_l < proc_max_address_l)
            {
                VirtualQueryEx(pHandle, proc_min_address, out var memInfo);
                var buffer = new byte[memInfo.RegionSize];
                UIntPtr test = (UIntPtr) memInfo.RegionSize, test2 = (UIntPtr) (long) memInfo.BaseAddress;

                ReadProcessMemory(pHandle, test2, buffer, test, IntPtr.Zero);

                AppendAllBytes(file, buffer); //due to memory limits, we have to dump it then store it in an array.
                //arrLength += buffer.Length;

                proc_min_address_l += memInfo.RegionSize;
                proc_min_address = new UIntPtr((ulong) proc_min_address_l);
            }

            Debug.Write(
                $"[DEBUG] memory dump completed. Saving dump file to {file}. ({DateTime.Now:h:mm:ss tt}){Environment.NewLine}");
            return true;
        }

        /// <summary>
        /// Array of byte scan.
        /// </summary>
        /// <param name="search">array of bytes to search for, OR your ini code label.</param>
        /// <param name="writable">Include writable addresses in scan</param>
        /// <param name="executable">Include executable addresses in scan</param>
        /// <param name="file">ini file (OPTIONAL)</param>
        /// <returns>IEnumerable of all addresses found.</returns>
        public Task<IEnumerable<long>> AoBScan(string search, bool writable = false, bool executable = true) =>
            AoBScan(0, long.MaxValue, search, writable, executable);

        /// <summary>
        /// Array of byte scan.
        /// </summary>
        /// <param name="search">array of bytes to search for, OR your ini code label.</param>
        /// <param name="readable">Include readable addresses in scan</param>
        /// <param name="writable">Include writable addresses in scan</param>
        /// <param name="executable">Include executable addresses in scan</param>
        /// <param name="file">ini file (OPTIONAL)</param>
        /// <returns>IEnumerable of all addresses found.</returns>
        public Task<IEnumerable<long>> AoBScan(string search, bool readable, bool writable, bool executable) =>
            AoBScan(0, long.MaxValue, search, readable, writable, executable);

        /// <summary>
        /// Array of Byte scan.
        /// </summary>
        /// <param name="start">Your starting address.</param>
        /// <param name="end">ending address</param>
        /// <param name="search">array of bytes to search for, OR your ini code label.</param>
        /// <param name="file">ini file (OPTIONAL)</param>
        /// <param name="writable">Include writable addresses in scan</param>
        /// <param name="executable">Include executable addresses in scan</param>
        /// <returns>IEnumerable of all addresses found.</returns>
        public Task<IEnumerable<long>> AoBScan(long start, long end, string search, bool writable = false,
            bool executable = true) => AoBScan(start, end, search, false, writable, executable);

        private unsafe int FindPattern(byte* body, int bodyLength, byte[] pattern, byte[] masks, int start = 0)
        {
            int foundIndex = -1;

            if (bodyLength <= 0 || pattern.Length <= 0 || start > bodyLength - pattern.Length ||
                pattern.Length > bodyLength) return foundIndex;

            for (int index = start; index <= bodyLength - pattern.Length; index++)
            {
                if (((body[index] & masks[0]) == (pattern[0] & masks[0])))
                {
                    var match = true;
                    for (int index2 = 1; index2 <= pattern.Length - 1; index2++)
                    {
                        if ((body[index + index2] & masks[index2]) == (pattern[index2] & masks[index2])) continue;
                        match = false;
                        break;
                    }

                    if (!match) continue;

                    foundIndex = index;
                    break;
                }
            }

            return foundIndex;
        }

        /// <summary>
        /// Array of Byte scan.
        /// </summary>
        /// <param name="start">Your starting address.</param>
        /// <param name="end">ending address</param>
        /// <param name="search">array of bytes to search for, OR your ini code label.</param>
        /// <param name="file">ini file (OPTIONAL)</param>
        /// <param name="readable">Include readable addresses in scan</param>
        /// <param name="writable">Include writable addresses in scan</param>
        /// <param name="executable">Include executable addresses in scan</param>
        /// <returns>IEnumerable of all addresses found.</returns>
        public Task<IEnumerable<long>> AoBScan(long start, long end, string search, bool readable, bool writable,
            bool executable) => Task.Run(() =>
        {
            var memRegionList = new List<MemoryRegionResult>();

            var memCode = LoadCode(search);

            var stringByteArray = memCode.Split(' ');

            byte[] aobPattern = new byte[stringByteArray.Length], mask = new byte[stringByteArray.Length];

            for (var i = 0; i < stringByteArray.Length; i++)
            {
                var ba = stringByteArray[i];

                if (ba == "??" || ba.Length == 1 && ba == "?")
                {
                    mask[i] = 0x00;
                    stringByteArray[i] = "0x00";
                }
                else if (char.IsLetterOrDigit(ba[0]) && ba[1] == '?')
                {
                    mask[i] = 0xF0;
                    stringByteArray[i] = ba[0] + "0";
                }
                else if (char.IsLetterOrDigit(ba[1]) && ba[0] == '?')
                {
                    mask[i] = 0x0F;
                    stringByteArray[i] = "0" + ba[1];
                }
                else mask[i] = 0xFF;
            }

            for (var i = 0; i < stringByteArray.Length; i++)
                aobPattern[i] = (byte) (Convert.ToByte(stringByteArray[i], 16) & mask[i]);

            GetSystemInfo(out var sys_info);

            UIntPtr proc_min_address = sys_info.minimumApplicationAddress,
                proc_max_address = sys_info.maximumApplicationAddress;

            if (start < (long) proc_min_address.ToUInt64()) start = (long) proc_min_address.ToUInt64();

            if (end > (long) proc_max_address.ToUInt64()) end = (long) proc_max_address.ToUInt64();

            Debug.WriteLine(
                $"[DEBUG] memory scan starting... (start:0x{start.ToString(MSize())} end:0x{end.ToString(MSize())} time:{DateTime.Now:h:mm:ss tt})");
            var currentBaseAddress = new UIntPtr((ulong) start);

            while (VirtualQueryEx(pHandle, currentBaseAddress, out var memInfo).ToUInt64() != 0 &&
                   currentBaseAddress.ToUInt64() < (ulong) end &&
                   currentBaseAddress.ToUInt64() + (ulong) memInfo.RegionSize >
                   currentBaseAddress.ToUInt64())
            {
                var isValid = memInfo.State == MEM_COMMIT;
                isValid &= memInfo.BaseAddress.ToUInt64() < proc_max_address.ToUInt64();
                isValid &= ((memInfo.Protect & PAGE_GUARD) == 0);
                isValid &= ((memInfo.Protect & PAGE_NOACCESS) == 0);
                isValid &= (memInfo.Type == MEM_PRIVATE) || (memInfo.Type == MEM_IMAGE);

                if (isValid)
                {
                    var isReadable = (memInfo.Protect & PAGE_READONLY) > 0;

                    var isWritable = ((memInfo.Protect & PAGE_READWRITE) > 0) ||
                                     ((memInfo.Protect & PAGE_WRITECOPY) > 0) ||
                                     ((memInfo.Protect & PAGE_EXECUTE_READWRITE) > 0) ||
                                     ((memInfo.Protect & PAGE_EXECUTE_WRITECOPY) > 0);

                    var isExecutable = ((memInfo.Protect & PAGE_EXECUTE) > 0) ||
                                       ((memInfo.Protect & PAGE_EXECUTE_READ) > 0) ||
                                       ((memInfo.Protect & PAGE_EXECUTE_READWRITE) > 0) ||
                                       ((memInfo.Protect & PAGE_EXECUTE_WRITECOPY) > 0);

                    isReadable &= readable;
                    isWritable &= writable;
                    isExecutable &= executable;

                    isValid &= isReadable || isWritable || isExecutable;
                }

                if (!isValid)
                {
                    currentBaseAddress = new UIntPtr(memInfo.BaseAddress.ToUInt64() + (ulong) memInfo.RegionSize);
                    continue;
                }

                var memRegion = new MemoryRegionResult
                {
                    CurrentBaseAddress = currentBaseAddress,
                    RegionSize = memInfo.RegionSize,
                    RegionBase = memInfo.BaseAddress
                };

                currentBaseAddress = new UIntPtr(memInfo.BaseAddress.ToUInt64() + (ulong) memInfo.RegionSize);

                if (memRegionList.Count > 0)
                {
                    var previousRegion = memRegionList[^1];

                    if ((long) previousRegion.RegionBase + previousRegion.RegionSize == (long) memInfo.BaseAddress)
                    {
                        memRegionList[^1] = new MemoryRegionResult
                        {
                            CurrentBaseAddress = previousRegion.CurrentBaseAddress,
                            RegionBase = previousRegion.RegionBase,
                            RegionSize = previousRegion.RegionSize + memInfo.RegionSize
                        };

                        continue;
                    }
                }

                memRegionList.Add(memRegion);
            }

            var bagResult = new ConcurrentBag<long>();

            Parallel.ForEach(memRegionList,
                (item, _, _) =>
                {
                    var compareResults = CompareScan(item, aobPattern, mask);
                    foreach (var result in compareResults)
                        bagResult.Add(result);
                });

            Debug.WriteLine($"[DEBUG] memory scan completed. (time:{DateTime.Now:h:mm:ss tt})");
            return bagResult.ToList().OrderBy(c => c).AsEnumerable();
        });

        private List<long> CompareScan(MemoryRegionResult item, byte[] aobPattern, byte[] mask)
        {
            if (mask.Length != aobPattern.Length)
                throw new ArgumentException($"{nameof(aobPattern)}.Length != {nameof(mask)}.Length");

            IntPtr buffer = Marshal.AllocHGlobal((int) item.RegionSize);

            ReadProcessMemory(pHandle, item.CurrentBaseAddress, buffer, (UIntPtr) item.RegionSize, out ulong bytesRead);

            int result = 0 - aobPattern.Length;
            List<long> ret = new List<long>();
            unsafe
            {
                do
                {
                    result = FindPattern((byte*) buffer.ToPointer(), (int) bytesRead, aobPattern, mask,
                        result + aobPattern.Length);

                    if (result >= 0)
                        ret.Add((long) item.CurrentBaseAddress + result);
                } while (result != -1);
            }

            Marshal.FreeHGlobal(buffer);

            return ret.ToList();
        }
    }
}