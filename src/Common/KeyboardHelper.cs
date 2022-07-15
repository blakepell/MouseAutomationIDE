/*
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
    /// Helper methods for use with the keyboard.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class KeyboardHelper
    {
        /// <summary>
        /// If either the left shift or right shift is down.
        /// </summary>
        public static bool IsShiftDown()
        {
            return (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
        }

        /// <summary>
        /// Whether the right shift, the left shift or the caps lock is down.
        /// </summary>
        public static bool IsShiftOrCapsLockDown()
        {
            return IsShiftDown() || IsCapsLockDown();
        }

        /// <summary>
        /// Whether the caps lock is locked on or not.
        /// </summary>
        /// <returns></returns>
        public static bool IsCapsLockDown()
        {
            return Keyboard.IsKeyToggled(Key.CapsLock);
        }

        /// <summary>
        /// If the left or right control is down.
        /// </summary>
        /// <returns></returns>
        public static bool IsCtrlDown()
        {
            return (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }

        /// <summary>
        /// If the left or right alt is down.
        /// </summary>
        /// <returns></returns>
        public static bool IsAltDown()
        {
            return (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
        }

        /// <summary>
        /// Converts a <see cref="Key"/> to a <see cref="KeyState"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyDecode"></param>
        public static void KeyToChar(Key key, out KeyState keyDecode)
        {
            keyDecode.Key = key;
            keyDecode.Alt = IsAltDown();
            keyDecode.Ctrl = IsCtrlDown();
            keyDecode.Shift = IsShiftDown();
            keyDecode.CapsLock = IsCapsLockDown();

            if (keyDecode.Alt || keyDecode.Ctrl)
            {
                keyDecode.Printable = false;
            }
            else
            {
                keyDecode.Printable = true;
            }

            bool isCaps = (keyDecode.CapsLock && !keyDecode.Shift) || (!keyDecode.CapsLock && keyDecode.Shift);

            switch (key)
            {
                case Key.Enter:
                    keyDecode.Character = '\n';
                    return;
                case Key.A:
                    keyDecode.Character = (isCaps ? 'A' : 'a');
                    return;
                case Key.B:
                    keyDecode.Character = (isCaps ? 'B' : 'b');
                    return;
                case Key.C:
                    keyDecode.Character = (isCaps ? 'C' : 'c');
                    return;
                case Key.D:
                    keyDecode.Character = (isCaps ? 'D' : 'd');
                    return;
                case Key.E:
                    keyDecode.Character = (isCaps ? 'E' : 'e');
                    return;
                case Key.F:
                    keyDecode.Character = (isCaps ? 'F' : 'f');
                    return;
                case Key.G:
                    keyDecode.Character = (isCaps ? 'G' : 'g');
                    return;
                case Key.H:
                    keyDecode.Character = (isCaps ? 'H' : 'h');
                    return;
                case Key.I:
                    keyDecode.Character = (isCaps ? 'I' : 'i');
                    return;
                case Key.J:
                    keyDecode.Character = (isCaps ? 'J' : 'j');
                    return;
                case Key.K:
                    keyDecode.Character = (isCaps ? 'K' : 'k');
                    return;
                case Key.L:
                    keyDecode.Character = (isCaps ? 'L' : 'l');
                    return;
                case Key.M:
                    keyDecode.Character = (isCaps ? 'M' : 'm');
                    return;
                case Key.N:
                    keyDecode.Character = (isCaps ? 'N' : 'n');
                    return;
                case Key.O:
                    keyDecode.Character = (isCaps ? 'O' : 'o');
                    return;
                case Key.P:
                    keyDecode.Character = (isCaps ? 'P' : 'p');
                    return;
                case Key.Q:
                    keyDecode.Character = (isCaps ? 'Q' : 'q');
                    return;
                case Key.R:
                    keyDecode.Character = (isCaps ? 'R' : 'r');
                    return;
                case Key.S:
                    keyDecode.Character = (isCaps ? 'S' : 's');
                    return;
                case Key.T:
                    keyDecode.Character = (isCaps ? 'T' : 't');
                    return;
                case Key.U:
                    keyDecode.Character = (isCaps ? 'U' : 'u');
                    return;
                case Key.V:
                    keyDecode.Character = (isCaps ? 'V' : 'v');
                    return;
                case Key.W:
                    keyDecode.Character = (isCaps ? 'W' : 'w');
                    return;
                case Key.X:
                    keyDecode.Character = (isCaps ? 'X' : 'x');
                    return;
                case Key.Y:
                    keyDecode.Character = (isCaps ? 'Y' : 'y');
                    return;
                case Key.Z:
                    keyDecode.Character = (isCaps ? 'Z' : 'z');
                    return;
                case Key.D0:
                    keyDecode.Character = (keyDecode.Shift ? ')' : '0');
                    return;
                case Key.D1:
                    keyDecode.Character = (keyDecode.Shift ? '!' : '1');
                    return;
                case Key.D2:
                    keyDecode.Character = (keyDecode.Shift ? '@' : '2');
                    return;
                case Key.D3:
                    keyDecode.Character = (keyDecode.Shift ? '#' : '3');
                    return;
                case Key.D4:
                    keyDecode.Character = (keyDecode.Shift ? '$' : '4');
                    return;
                case Key.D5:
                    keyDecode.Character = (keyDecode.Shift ? '%' : '5');
                    return;
                case Key.D6:
                    keyDecode.Character = (keyDecode.Shift ? '^' : '6');
                    return;
                case Key.D7:
                    keyDecode.Character = (keyDecode.Shift ? '&' : '7');
                    return;
                case Key.D8:
                    keyDecode.Character = (keyDecode.Shift ? '*' : '8');
                    return;
                case Key.D9:
                    keyDecode.Character = (keyDecode.Shift ? '(' : '9');
                    return;
                case Key.OemPlus:
                    keyDecode.Character = (keyDecode.Shift ? '+' : '=');
                    return;
                case Key.OemMinus:
                    keyDecode.Character = (keyDecode.Shift ? '_' : '-');
                    return;
                case Key.OemQuestion:
                    keyDecode.Character = (keyDecode.Shift ? '?' : '/');
                    return;
                case Key.OemComma:
                    keyDecode.Character = (keyDecode.Shift ? '<' : ',');
                    return;
                case Key.OemPeriod:
                    keyDecode.Character = (keyDecode.Shift ? '>' : '.');
                    return;
                case Key.OemOpenBrackets:
                    keyDecode.Character = (keyDecode.Shift ? '{' : '[');
                    return;
                case Key.OemQuotes:
                    keyDecode.Character = (keyDecode.Shift ? '"' : '\'');
                    return;
                case Key.Oem1:
                    keyDecode.Character = (keyDecode.Shift ? ':' : ';');
                    return;
                case Key.Oem3:
                    keyDecode.Character = (keyDecode.Shift ? '~' : '`');
                    return;
                case Key.Oem5:
                    keyDecode.Character = (keyDecode.Shift ? '|' : '\\');
                    return;
                case Key.Oem6:
                    keyDecode.Character = (keyDecode.Shift ? '}' : ']');
                    return;
                case Key.Tab:
                    keyDecode.Character = '\t';
                    return;
                case Key.Space:
                    keyDecode.Character = ' ';
                    return;

                // Number Pad
                case Key.NumPad0:
                    keyDecode.Character = '0';
                    return;
                case Key.NumPad1:
                    keyDecode.Character = '1';
                    return;
                case Key.NumPad2:
                    keyDecode.Character = '2';
                    return;
                case Key.NumPad3:
                    keyDecode.Character = '3';
                    return;
                case Key.NumPad4:
                    keyDecode.Character = '4';
                    return;
                case Key.NumPad5:
                    keyDecode.Character = '5';
                    return;
                case Key.NumPad6:
                    keyDecode.Character = '6';
                    return;
                case Key.NumPad7:
                    keyDecode.Character = '7';
                    return;
                case Key.NumPad8:
                    keyDecode.Character = '8';
                    return;
                case Key.NumPad9:
                    keyDecode.Character = '9';
                    return;
                case Key.Subtract:
                    keyDecode.Character = '-';
                    return;
                case Key.Add:
                    keyDecode.Character = '+';
                    return;
                case Key.Decimal:
                    keyDecode.Character = '.';
                    return;
                case Key.Divide:
                    keyDecode.Character = '/';
                    return;
                case Key.Multiply:
                    keyDecode.Character = '*';
                    return;

                default:
                    keyDecode.Printable = false;
                    keyDecode.Character = '\x00';
                    return;
            }
        }

        /// <summary>
        /// The state that existed when a key was pressed.
        /// </summary>
        public struct KeyState
        {
            public Key Key;
            public bool Printable;
            public char Character;
            public bool Shift;
            public bool Ctrl;
            public bool Alt;
            public bool CapsLock;
        }
    }
}