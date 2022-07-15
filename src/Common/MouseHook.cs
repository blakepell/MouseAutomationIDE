﻿/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace LuaAutomation.Common
{
    /// <summary>
    /// Class for intercepting low level Windows mouse hooks.
    /// </summary>
    [SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    public class MouseHook
    {
        /// <summary>
        /// Function to be called when defined even occurs
        /// </summary>
        /// <param name="mouseStruct">MSLLHOOKSTRUCT mouse structure</param>
        public delegate void MouseHookCallback(MouseHookStruct mouseStruct);

        private MouseHookHandler _hookHandler;

        /// <summary>
        /// Low level mouse hook's ID
        /// </summary>
        private IntPtr hookID = IntPtr.Zero;

        /// <summary>
        /// Install low level mouse hook
        /// </summary>
        public void Install()
        {
            _hookHandler = this.HookFunc;
            hookID = this.SetHook(_hookHandler);
        }

        /// <summary>
        /// Remove low level mouse hook
        /// </summary>
        public void Uninstall()
        {
            if (hookID == IntPtr.Zero)
            {
                return;
            }

            UnhookWindowsHookEx(hookID);
            hookID = IntPtr.Zero;
        }

        /// <summary>
        /// Destructor. Unhook current hook
        /// </summary>
        ~MouseHook()
        {
            this.Uninstall();
        }

        /// <summary>
        /// Sets hook and assigns its ID for tracking
        /// </summary>
        /// <param name="proc">Internal callback function</param>
        /// <returns>Hook ID</returns>
        private IntPtr SetHook(MouseHookHandler proc)
        {
            using (var module = Process.GetCurrentProcess().MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(module.ModuleName), 0);
            }
        }

        /// <summary>
        /// Callback function
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // parse system messages
            if (nCode >= 0)
            {
                if (MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
                {
                    this.LeftButtonDown?.Invoke(
                        (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
                {
                    this.LeftButtonUp?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_RBUTTONDOWN == (MouseMessages)wParam)
                {
                    this.RightButtonDown?.Invoke(
                        (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_RBUTTONUP == (MouseMessages)wParam)
                {
                    this.RightButtonUp?.Invoke(
                        (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_MOUSEMOVE == (MouseMessages)wParam)
                {
                    this.MouseMove?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_MOUSEWHEEL == (MouseMessages)wParam)
                {
                    this.MouseWheel?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_LBUTTONDBLCLK == (MouseMessages)wParam)
                {
                    this.DoubleClick?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_MBUTTONDOWN == (MouseMessages)wParam)
                {
                    this.MiddleButtonDown?.Invoke(
                        (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_MBUTTONUP == (MouseMessages)wParam)
                {
                    this.MiddleButtonUp?.Invoke(
                        (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_XBUTTONDOWN == (MouseMessages)wParam)
                {
                    this.XButtonDown?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_XBUTTONUP == (MouseMessages)wParam)
                {
                    this.XButtonUp?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        /// <summary>
        /// Internal callback processing function
        /// </summary>
        private delegate IntPtr MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam);

        #region Events

        public event MouseHookCallback? LeftButtonDown;
        public event MouseHookCallback? LeftButtonUp;
        public event MouseHookCallback? RightButtonDown;
        public event MouseHookCallback? RightButtonUp;
        public event MouseHookCallback? MouseMove;
        public event MouseHookCallback? MouseWheel;
        public event MouseHookCallback? DoubleClick;
        public event MouseHookCallback? MiddleButtonDown;
        public event MouseHookCallback? MiddleButtonUp;
        public event MouseHookCallback? XButtonDown;
        public event MouseHookCallback? XButtonUp;

        #endregion

        #region WinAPI

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseHookStruct
        {
            public Point pt;
            public uint mouseData;
            public LowLevelMouseEvent flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [Flags]
        public enum LowLevelMouseEvent : uint
        {
            None = 0x00,
            Injected = 0x01,
            LowerIntegrityLevelInjected = 0x02
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, MouseHookHandler lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}