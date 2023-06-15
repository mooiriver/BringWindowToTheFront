using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace MakeManagerKing
{
    static class IsManagerFound
    {
        public static bool managerFound = false;
    }

    class Program
    {     
        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;
        private const uint Restore = 9;        

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ShowWindow(IntPtr hWnd, uint Msg);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
       
        static void Main(string[] args)
        {
            int threadSleepTime = args.Length == 0 ? 300 : Int32.Parse(args[1]);            

            while (true)
            {
                Process[] processes = Process.GetProcesses();
                Process[] processesByName = Process.GetProcessesByName("TTMine_Manager");

                foreach (var process in processes)
                {
                    if (process.ProcessName.Contains("TTMine_Manager") &&
                        IsManagerFound.managerFound == false)
                    {
                        IntPtr handle = process.MainWindowHandle;
                        ActivateWindow(handle);

                        IsManagerFound.managerFound = true;
                        break;
                        //Thread.Sleep(60000);
                    }
                    if (IsManagerFound.managerFound == true &&
                        processesByName.Length == 0)
                    {
                        IsManagerFound.managerFound = false;
                    }
                }
                Thread.Sleep(threadSleepTime);
            }            
        }

        private static void ActivateWindow(IntPtr mainWindowHandle)
        {
            //check if already has focus
            if (mainWindowHandle == GetForegroundWindow())
            {
                return;
            }

            //check if window is minimized
            if (IsIconic(mainWindowHandle))
            {
                ShowWindow(mainWindowHandle, Restore);
            }

            // Simulate a key press
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);

            //SetForegroundWindow(mainWindowHandle);

            // Simulate a key release
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);

            SetForegroundWindow(mainWindowHandle);
        }
    }
    
}