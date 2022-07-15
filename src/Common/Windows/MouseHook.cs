/*
 * Lua Automation IDE
 *
 * @project lead      : Blake Pell
 * @website           : http://www.blakepell.com
 * @copyright         : Copyright (c), 2022 All rights reserved.
 * @license           : Closed Source
 */

namespace LuaAutomation.Common.Windows
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

        /// <summary>
        /// Internal callback processing function reference.
        /// </summary>
        private MouseHookHandler? _hookHandler;

        /// <summary>
        /// Low level mouse hook's ID
        /// </summary>
        private IntPtr hookID = IntPtr.Zero;

        /// <summary>
        /// Install low level mouse hook
        /// </summary>
        public void Install()
        {
            _hookHandler = HookFunc;
            hookID = SetHook(_hookHandler);
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

            NativeMethods.UnhookWindowsHookEx(hookID);
            hookID = IntPtr.Zero;
        }

        /// <summary>
        /// Destructor. Unhook current hook
        /// </summary>
        ~MouseHook()
        {
            Uninstall();
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
                return NativeMethods.SetWindowsHookEx(NativeMethods.WH_MOUSE_LL, proc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
            }
        }

        /// <summary>
        /// Callback function
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Parse system messages
            if (nCode >= 0)
            {
                if (MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
                {
                    LeftButtonDown?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
                {
                    LeftButtonUp?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_RBUTTONDOWN == (MouseMessages)wParam)
                {
                    RightButtonDown?.Invoke(
                        (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_RBUTTONUP == (MouseMessages)wParam)
                {
                    RightButtonUp?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_MOUSEMOVE == (MouseMessages)wParam)
                {
                    MouseMove?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_MOUSEWHEEL == (MouseMessages)wParam)
                {
                    MouseWheel?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_LBUTTONDBLCLK == (MouseMessages)wParam)
                {
                    DoubleClick?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_MBUTTONDOWN == (MouseMessages)wParam)
                {
                    MiddleButtonDown?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_MBUTTONUP == (MouseMessages)wParam)
                {
                    MiddleButtonUp?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_XBUTTONDOWN == (MouseMessages)wParam)
                {
                    XButtonDown?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }

                if (MouseMessages.WM_XBUTTONUP == (MouseMessages)wParam)
                {
                    XButtonUp?.Invoke((MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct)));
                }
            }

            return NativeMethods.CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        /// <summary>
        /// Internal callback processing function
        /// </summary>
        public delegate IntPtr MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam);

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
    }
}