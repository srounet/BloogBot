using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using static Bootstraper.WinImports;


namespace Bootstraper
{
    class Program
    {
        const string PATH_TO_GAME = @"D:\Wow\Wow.exe";

        static void Main(string[] args)
        {
            var startupInfo = new STARTUPINFO();

            // run BloogsQuest.exe in a new process
            CreateProcess(
                PATH_TO_GAME,
                null,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                0x04000000,
                IntPtr.Zero,
                IntPtr.Zero,
                ref startupInfo,
                out PROCESS_INFORMATION processInfo);

            // this seems to help prevent timing issues
            Thread.Sleep(500);

            // get a handle to the BloogsQuest process
            var processHandle = Process.GetProcessById((int)processInfo.dwProcessId).Handle;

            // resolve the file path to Loader.dll relative to our current working directory
            var loaderPath = Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Loader.dll");

            // allocate enough memory to hold the full file path to Loader.dll within the BloogsQuest process
            var loaderPathPtr = VirtualAllocEx(
                processHandle,
                (IntPtr)0,
                loaderPath.Length,
                AllocationType.Commit,
                MemoryProtection.ExecuteReadWrite);

            // this seems to help prevent timing issues
            Thread.Sleep(500);

            // write the file path to Loader.dll to the BloogsQuest process's memory
            var bytes = Encoding.Unicode.GetBytes(loaderPath);
            var bytesWritten = 0; // throw away
            WriteProcessMemory(processHandle, loaderPathPtr, bytes, bytes.Length, out bytesWritten);

            // search current process's for the memory address of the LoadLibraryW function within the kernel32.dll module
            var loaderDllPointer = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryW");

            // this seems to help prevent timing issues
            Thread.Sleep(500);

            // create a new thread with the execution starting at the LoadLibraryW function, 
            // with the path to our Loader.dll passed as a parameter
            IntPtr throwaway = IntPtr.Zero;
            CreateRemoteThread(processHandle, (IntPtr)null, (uint)0, loaderDllPointer, loaderPathPtr, 0, out throwaway);

            // this seems to help prevent timing issues
            Thread.Sleep(500);

            // free the memory that was allocated by VirtualAllocEx
            VirtualFreeEx(processHandle, loaderPathPtr, 0, FreeType.Release);
        }
    }
}
