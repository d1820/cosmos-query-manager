using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace CosmosManager.Utilities
{

    public static class ConsoleHelper
    {
        /// <summary>
        /// Allocates a console and resets the standard stream handles.
        /// </summary>
        public static void Alloc(bool alwaysCreateNewConsole = true)
        {

            var consoleAttached = true;
            if (alwaysCreateNewConsole
                || (AttachConsole(ATTACH_PARRENT) == 0
                && Marshal.GetLastWin32Error() != ERROR_ACCESS_DENIED))
            {
                consoleAttached = AllocConsole();
            }

            if (consoleAttached)
            {
                SetStdHandle(StdHandle.Output, GetConsoleStandardOutput());
                SetStdHandle(StdHandle.Input, GetConsoleStandardInput());
            }

            //if (!AllocConsole())
            //    throw new Win32Exception();


        }

        private static IntPtr GetConsoleStandardInput()
        {
            var handle = CreateFile
                ("CONIN$"
                , DesiredAccess.GenericRead | DesiredAccess.GenericWrite
                , FileShare.ReadWrite
                , IntPtr.Zero
                , FileMode.Open
                , FileAttributes.Normal
                , IntPtr.Zero
                );
            if (handle == InvalidHandleValue)
                throw new Win32Exception();
            return handle;
        }

        private static IntPtr GetConsoleStandardOutput()
        {
            var handle = CreateFile
                ("CONOUT$"
                , DesiredAccess.GenericWrite | DesiredAccess.GenericWrite
                , FileShare.ReadWrite
                , IntPtr.Zero
                , FileMode.Open
                , FileAttributes.Normal
                , IntPtr.Zero
                );
            if (handle == InvalidHandleValue)
                throw new Win32Exception();
            return handle;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UInt32 AttachConsole(UInt32 dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool SetStdHandle(StdHandle nStdHandle, IntPtr hHandle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile
            (string lpFileName
            , [MarshalAs(UnmanagedType.U4)] DesiredAccess dwDesiredAccess
            , [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode
            , IntPtr lpSecurityAttributes
            , [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition
            , [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes
            , IntPtr hTemplateFile
            );

        [Flags]
        enum DesiredAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000
        }

        private enum StdHandle : int
        {
            Input = -10,
            Output = -11,
            Error = -12
        }

        private const UInt32 ATTACH_PARRENT = 0xFFFFFFFF;
        private const UInt32 ERROR_ACCESS_DENIED = 5;

        private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);
    }

}