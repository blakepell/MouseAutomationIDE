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
    /// Class for intercepting low level keyboard hooks
    /// </summary>
    [SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    public class KeyHook
    {
        /// <summary>
        /// Function that will be called when defined events occur
        /// </summary>
        /// <param name="key">VKeys</param>
        public delegate void KeyboardHookCallback(VirtualKeys key);

        /// <summary>
        /// Internal callback processing function reference.
        /// </summary>
        private KeyboardHookHandler? _hookHandler;

        /// <summary>
        /// Hook ID
        /// </summary>
        private IntPtr hookID = IntPtr.Zero;

        /// <summary>
        /// Install low level keyboard hook
        /// </summary>
        public void Install()
        {
            _hookHandler = this.HookFunc;
            hookID = this.SetHook(_hookHandler);
        }

        /// <summary>
        /// Remove low level keyboard hook
        /// </summary>
        public void Uninstall()
        {
            NativeMethods.UnhookWindowsHookEx(hookID);
        }

        /// <summary>
        /// Registers hook with Windows API
        /// </summary>
        /// <param name="proc">Callback function</param>
        /// <returns>Hook ID</returns>
        private IntPtr SetHook(KeyboardHookHandler proc)
        {
            using (var module = Process.GetCurrentProcess().MainModule)
            {
                return NativeMethods.SetWindowsHookEx(13, proc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
            }
        }

        /// <summary>
        /// Default hook call, which analyses pressed keys
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Parse system messages
            if (nCode >= 0)
            {
                int iwParam = wParam.ToInt32();

                if (iwParam == NativeMethods.WM_KEYDOWN || iwParam == NativeMethods.WM_SYSKEYDOWN)
                {
                    if (this.KeyDown != null)
                    {
                        this.KeyDown((VirtualKeys)Marshal.ReadInt32(lParam));
                    }
                }

                if (iwParam == NativeMethods.WM_KEYUP || iwParam == NativeMethods.WM_SYSKEYUP)
                {
                    if (this.KeyUp != null)
                    {
                        this.KeyUp((VirtualKeys)Marshal.ReadInt32(lParam));
                    }
                }
            }

            return NativeMethods.CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        /// <summary>
        /// Destructor. Unhook current hook
        /// </summary>
        ~KeyHook()
        {
            this.Uninstall();
        }

        /// <summary>
        /// Internal callback processing function
        /// </summary>
        public delegate IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Event for when a key is pressed down.
        /// </summary>
        public event KeyboardHookCallback? KeyDown;

        /// <summary>
        /// Event for when a key is released.
        /// </summary>
        public event KeyboardHookCallback? KeyUp;
    }
}