using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static TaiSentry.AppObserver.Servicers.AppManager;

namespace TaiSentry.Utils.Win32API
{
    /// <summary>
    /// 与窗口操作有关的Win32API
    /// </summary>
    public static class Win32WindowAPI
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner

            public int Width
            {
                get
                {
                    return Right - Left;
                }
            }
            public int Height
            {
                get
                {
                    return Bottom - Top;
                }
            }

        }

        public static RECT GetWindowRect(IntPtr handle_)
        {
            try
            {
                GetWindowRect(handle_, out RECT rect);
                return rect;

            }
            catch (Exception e)
            {
                return new RECT()
                {
                    Left = 0,
                    Bottom = 0,
                    Right = 0,
                    Top = 0
                };
            }
        }

        public static string GetWindowClassName(IntPtr handle_)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder(256);
                GetClassName(handle_, stringBuilder, stringBuilder.Capacity);

                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}
