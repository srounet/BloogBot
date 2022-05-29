using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace BloogBot
{
    public static unsafe class MemoryManager
    {
        #region flags

        [Flags]
        private enum ProcessAccessFlags
        {
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            SYNCHRONIZE = 0x00100000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            PROCESS_ALL_ACCESS = 0x001F0FFF,
            PROCESS_CREATE_PROCESS = 0x0080,
            PROCESS_CREATE_THREAD = 0x0002,
            PROCESS_DUP_HANDLE = 0x0040,
            PROCESS_QUERY_INFORMATION = 0x0400,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
            PROCESS_SET_INFORMATION = 0x0200,
            PROCESS_SET_QUOTA = 0x0100,
            PROCESS_SUSPEND_RESUME = 0x0800,
            PROCESS_TERMINATE = 0x0001,
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020
        }

        [Flags]
        public enum Protection
        {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }

        #endregion flags

        #region winapi

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static extern void MoveMemory(void* dest, void* src, int size);

        [DllImport("kernel32.dll")]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(IntPtr address, int size, uint newProtect, out uint oldProtect);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags desiredAccess, bool inheritHandle, int processId);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        #endregion winapi

        #region delegates

        public static T GetRegisterDelegate<T>(int address) where T : class
        {
            return GetRegisterDelegate<T>(new IntPtr(address));
        }

        public static T GetRegisterDelegate<T>(long address) where T : class
        {
            return GetRegisterDelegate<T>(new IntPtr(address));
        }

        public static T GetRegisterDelegate<T>(IntPtr address) where T : class
        {
            return (Marshal.GetDelegateForFunctionPointer(address, typeof(T)) as T);
        }

        #endregion delegates

        #region read

        [HandleProcessCorruptedStateExceptions]
        internal static byte ReadByte(IntPtr address)
        {
            try
            {
                return *(byte*)address;
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine("Access Violation on " + address + " with type Byte");
                return default;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        internal static int ReadInt(IntPtr address)
        {
            try
            {
                return *(int*)address;
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine("Access Violation on " + address + " with type Int");
                return default;
            }
            catch (NullReferenceException)
            {
                return default;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        internal static ulong ReadUlong(IntPtr address)
        {
            try
            {
                return *(ulong*)address;
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine("Access Violation on " + address + " with type Ulong");
                return default;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        internal static IntPtr ReadIntPtr(IntPtr address)
        {
            try
            {
                return *(IntPtr*)address;
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine("Access Violation on " + address + " with type IntPtr");
                return default;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        internal static float ReadFloat(IntPtr address)
        {
            try
            {
                return *(float*)address;
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine("Access Violation on " + address + " with type Float");
                return default;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        internal static string ReadString(IntPtr address, int length = 512)
        {
            var buffer = ReadBytes(address, length);
            if (buffer.Length == 0)
                return default;

            var ret = Encoding.ASCII.GetString(buffer);

            if (ret.IndexOf('\0') != -1)
                ret = ret.Remove(ret.IndexOf('\0'));

            return ret;
        }

        [HandleProcessCorruptedStateExceptions]
        internal static byte[] ReadBytes(IntPtr address, int count)
        {
            try
            {
                var ret = new byte[count];
                var ptr = (byte*)address;

                for (var i = 0; i < count; i++)
                    ret[i] = ptr[i];

                return ret;
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine("Access Violation on " + address + " with type Byte[]");
                return default;
            }
        }

        #endregion read

        #region write

        static internal void WriteByte(IntPtr address, byte value) => Marshal.StructureToPtr(value, address, false);

        static internal void WriteInt(IntPtr address, int value) => Marshal.StructureToPtr(value, address, false);

        static internal void WriteFloat(IntPtr address, float value) => Marshal.StructureToPtr(value, address, false);

        // certain memory locations (Warden for example) are protected from modification.
        // we use OpenAccess with ProcessAccessFlags to remove the protection.
        // you can check whether memory is successfully being modified by setting a breakpoint
        // here and checking Debug -> Windows -> Disassembly.
        // if you have further issues, you may need to use VirtualProtect from the Win32 API.
        static internal void WriteBytes(IntPtr address, byte[] bytes)
        {
            var access = ProcessAccessFlags.PROCESS_CREATE_THREAD |
                         ProcessAccessFlags.PROCESS_QUERY_INFORMATION |
                         ProcessAccessFlags.PROCESS_SET_INFORMATION |
                         ProcessAccessFlags.PROCESS_TERMINATE |
                         ProcessAccessFlags.PROCESS_VM_OPERATION |
                         ProcessAccessFlags.PROCESS_VM_READ |
                         ProcessAccessFlags.PROCESS_VM_WRITE |
                         ProcessAccessFlags.SYNCHRONIZE;

            var process = OpenProcess(access, false, Process.GetCurrentProcess().Id);

            int ret = 0;
            WriteProcessMemory(process, address, bytes, bytes.Length, ref ret);

            var protection = Protection.PAGE_EXECUTE_READWRITE;
            // now set the memory to be executable
            VirtualProtect(address, bytes.Length, (uint)protection, out uint _);
        }

        #endregion write

        [HandleProcessCorruptedStateExceptions]
        public static T InternalRead<T>(IntPtr address) where T : struct
        {
            try
            {
                // TODO: Optimize this more. The boxing/unboxing required tends to slow this down.
                // It may be worth it to simply use memcpy to avoid it, but I doubt thats going to give any noticeable increase in speed.
                if (address == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Cannot retrieve a value at address 0");
                }

                object ret;
                switch (MarshalCache<T>.TypeCode)
                {
                    case TypeCode.Object:

                        if (MarshalCache<T>.IsIntPtr)
                        {
                            return (T)(object)*(IntPtr*)address;
                        }

                        // If the type doesn't require an explicit Marshal call, then ignore it and memcpy the fuckin thing.
                        if (!MarshalCache<T>.TypeRequiresMarshal)
                        {
                            T o = default(T);
                            void* ptr = MarshalCache<T>.GetUnsafePtr(ref o);

                            MoveMemory(ptr, (void*)address, MarshalCache<T>.Size);

                            return o;
                        }

                        // All System.Object's require marshaling!
                        ret = Marshal.PtrToStructure(address, typeof(T));
                        break;

                    case TypeCode.Boolean:
                        ret = *(byte*)address != 0;
                        break;

                    case TypeCode.Char:
                        ret = *(char*)address;
                        break;

                    case TypeCode.SByte:
                        ret = *(sbyte*)address;
                        break;

                    case TypeCode.Byte:
                        ret = *(byte*)address;
                        break;

                    case TypeCode.Int16:
                        ret = *(short*)address;
                        break;

                    case TypeCode.UInt16:
                        ret = *(ushort*)address;
                        break;

                    case TypeCode.Int32:
                        ret = *(int*)address;
                        break;

                    case TypeCode.UInt32:
                        ret = *(uint*)address;
                        break;

                    case TypeCode.Int64:
                        ret = *(long*)address;
                        break;

                    case TypeCode.UInt64:
                        ret = *(ulong*)address;
                        break;

                    case TypeCode.Single:
                        ret = *(float*)address;
                        break;

                    case TypeCode.Double:
                        ret = *(double*)address;
                        break;

                    case TypeCode.Decimal:
                        // Probably safe to remove this. I'm unaware of anything that actually uses "decimal" that would require memory reading...
                        ret = *(decimal*)address;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return (T)ret;
            }
            catch (AccessViolationException)
            {
                Trace.WriteLine("Access Violation on " + address + " with type " + typeof(T).Name);
                return default(T);
            }
        }
    }

    public static class MarshalCache<T>
    {
        /// <summary> The size of the Type </summary>
        public static int Size;

        /// <summary> The size of the Type </summary>
        public static uint SizeU;

        /// <summary> The real, underlying type. </summary>
        public static Type RealType;

        /// <summary> The type code </summary>
        public static TypeCode TypeCode;

        /// <summary> True if this type requires the Marshaler to map variables. (No direct pointer dereferencing) </summary>
        public static bool TypeRequiresMarshal;

        public static bool IsIntPtr;

        internal static readonly GetUnsafePtrDelegate GetUnsafePtr;

        static MarshalCache()
        {
            TypeCode = Type.GetTypeCode(typeof(T));

            // Bools = 1 char.
            if (typeof(T) == typeof(bool))
            {
                Size = 1;
                RealType = typeof(T);
            }
            else if (typeof(T).IsEnum)
            {
                Type underlying = typeof(T).GetEnumUnderlyingType();
                Size = Marshal.SizeOf(underlying);
                RealType = underlying;
                TypeCode = Type.GetTypeCode(underlying);
            }
            else
            {
                Size = Marshal.SizeOf(typeof(T));
                RealType = typeof(T);
            }

            IsIntPtr = RealType == typeof(IntPtr);

            SizeU = (uint)Size;

            Debug.Write("[MarshalCache] " + typeof(T) + " Size: " + SizeU);

            // Basically, if any members of the type have a MarshalAs attrib, then we can't just pointer deref. :(
            // This literally means any kind of MarshalAs. Strings, arrays, custom type sizes, etc.
            // Ideally, we want to avoid the Marshaler as much as possible. It causes a lot of overhead, and for a memory reading
            // lib where we need the best speed possible, we do things manually when possible!
            TypeRequiresMarshal =
                RealType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(
                    m => m.GetCustomAttributes(typeof(MarshalAsAttribute), true).Any());

            // Generate a method to get the address of a generic type. We'll be using this for RtlMoveMemory later for much faster structure reads.
            var method = new DynamicMethod(string.Format("GetPinnedPtr<{0}>", typeof(T).FullName.Replace(".", "<>")),
                                           typeof(void*), new[] { typeof(T).MakeByRefType() },
                                           typeof(MarshalCache<>).Module);
            ILGenerator generator = method.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Conv_U);
            generator.Emit(OpCodes.Ret);
            GetUnsafePtr = (GetUnsafePtrDelegate)method.CreateDelegate(typeof(GetUnsafePtrDelegate));
        }

        #region Nested type: GetUnsafePtrDelegate

        internal unsafe delegate void* GetUnsafePtrDelegate(ref T value);

        #endregion Nested type: GetUnsafePtrDelegate
    }
}