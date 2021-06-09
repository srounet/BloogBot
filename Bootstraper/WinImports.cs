using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bootstraper
{
    public static class WinImports
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            uint cb;
            IntPtr lpReserved;
            IntPtr lpDesktop;
            IntPtr lpTitle;
            uint dwX;
            uint dwY;
            uint dwXSize;
            uint dwYSize;
            uint dwXCountChars;
            uint dwYCountChars;
            uint dwFillAttributes;
            uint dwFlags;
            ushort wShowWindow;
            ushort cbReserved;
            IntPtr lpReserved2;
            IntPtr hStdInput;
            IntPtr hStdOutput;
            IntPtr hStdErr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }
        #region Enums
        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
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

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [Flags]
        public enum FreeType
        {
            Decommit = 0x4000,
            Release = 0x8000,
        }

        [Flags]
        public enum ThreadFlags
        {
            /// <summary>
            /// The thread will execute immediately.
            /// </summary>
            THREAD_EXECUTE_IMMEDIATELY = 0,
            /// <summary>
            /// The thread will be created in a suspended state.  Use <see cref="Imports.ResumeThread"/> to resume the thread.
            /// </summary>
            CREATE_SUSPENDED = 0x04,
            /// <summary>
            /// The dwStackSize parameter specifies the initial reserve size of the stack. If this flag is not specified, dwStackSize specifies the commit size.
            /// </summary>
            STACK_SIZE_PARAM_IS_A_RESERVATION = 0x00010000,
            /// <summary>
            /// The thread is still active.
            /// </summary>
            STILL_ACTIVE = 259,
        }

        [Flags]
        public enum ThreadWaitValues : uint
        {
            /// <summary>
            /// The object is in a signaled state.
            /// </summary>
            WAIT_OBJECT_0 = 0x00000000,
            /// <summary>
            /// The specified object is a mutex object that was not released by the thread that owned the mutex object before the owning thread terminated. Ownership of the mutex object is granted to the calling thread, and the mutex is set to nonsignaled.
            /// </summary>
            WAIT_ABANDONED = 0x00000080,
            /// <summary>
            /// The time-out interval elapsed, and the object's state is nonsignaled.
            /// </summary>
            WAIT_TIMEOUT = 0x00000102,
            /// <summary>
            /// The wait has failed.
            /// </summary>
            WAIT_FAILED = 0xFFFFFFFF,
            /// <summary>
            /// Wait an infinite amount of time for the object to become signaled.
            /// </summary>
            INFINITE = 0xFFFFFFFF,
        }

        #endregion

        #region DLLImport

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CreateProcess(string lpApplicationName,
                                                 string lpCommandLine,
                                                 IntPtr lpProcAttribs,
                                                 IntPtr lpThreadAttribs,
                                                 bool bInheritHandles,
                                                 uint dwCreateFlags,
                                                 IntPtr lpEnvironment,
                                                 IntPtr lpCurrentDir, [In] ref STARTUPINFO lpStartinfo,
                                                 out PROCESS_INFORMATION lpProcInformation);


        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess,
                                                    IntPtr lpAddress,
                                                    int dwSize,
                                                    AllocationType flAllocationType,
                                                    MemoryProtection flProtect);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags DesiredAccess,
                                                    bool bInheritHandle,
                                                    int dwProcessId);

        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule,
                                                    string procedureName);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess,
                                                    IntPtr lpBaseAddress,
                                                    byte[] lpBuffer,
                                                    int nSize,
                                                    out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess,
                                                    IntPtr lpThreadAttributes,
                                                    uint dwStackSize,
                                                    IntPtr lpStartAddress,
                                                    IntPtr lpParameter,
                                                    ThreadFlags dwCreationFlags,
                                                    out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle,
                                                    UInt32 dwMilliseconds);

        [DllImport("kernel32.dll")]
        public static extern bool GetExitCodeThread(IntPtr hThread,
                                                out IntPtr lpExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess,
                                                IntPtr lpAddress,
                                                int dwSize,
                                                FreeType dwFreeType);

        #endregion
    }
}
