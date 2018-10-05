using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    public interface IHookCallback
    {
        void OnKeyDown(Keys key);
        void OnKeyUp(Keys key);
    }

    public class KeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, module.BaseAddress, 0);
            }
        }

        private IHookCallback m_Callback;
        private IntPtr m_HookID = IntPtr.Zero;

        private void InvokeCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                m_Callback.OnKeyDown((Keys) vkCode);
            }
            else if (nCode >= 0 && wParam == (IntPtr) WM_KEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                m_Callback.OnKeyUp((Keys) vkCode);
            }
            else if (nCode >= 0 && wParam == (IntPtr) WM_SYSKEYDOWN)
            {
                // TODO: Should this be handled in some other way?
                int vkCode = Marshal.ReadInt32(lParam);
                m_Callback.OnKeyDown((Keys) vkCode);
            }
            else if (nCode >= 0 && wParam == (IntPtr) WM_SYSKEYUP)
            {
                // TODO: Should this be handled in some other way?
                int vkCode = Marshal.ReadInt32(lParam);
                m_Callback.OnKeyUp((Keys) vkCode);
            }
            else
            {
                Console.WriteLine("Unknown wParam: {0}", wParam);
            }
        }

        private IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                InvokeCallback(nCode, wParam, lParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in keyboard proc: {0}", ex);
            }

            return CallNextHookEx(m_HookID, nCode, wParam, lParam);
        }

        public KeyboardHook(IHookCallback callback)
        {
            m_Callback = callback;
            m_HookID = SetHook(KeyboardProc);
        }

        private void Dispose(bool disposing)
        {
            try
            {
                if (!UnhookWindowsHookEx(m_HookID))
                    throw new Win32Exception("Cannot remove keyboard hook");
            }
            finally
            {
                if (disposing)
                    GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~KeyboardHook()
        {
            Dispose(false);
        }
    }
}
