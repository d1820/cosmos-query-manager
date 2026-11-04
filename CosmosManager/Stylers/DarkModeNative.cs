using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CosmosManager.Stylers
{
    internal static class DarkModeNative
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        /// <summary>
        /// Switches a control's native scrollbars (and ListView headers) to the dark Explorer theme.
        /// Applied once the handle exists; re-applied on HandleCreated so it survives handle recreation.
        /// </summary>
        public static void UseDarkScrollbars(Control control)
        {
            if (control == null)
            {
                return;
            }

            void Apply()
            {
                SetWindowTheme(control.Handle, "DarkMode_Explorer", null);
            }

            if (control.IsHandleCreated)
            {
                Apply();
            }
            control.HandleCreated += (s, e) => Apply();
        }
    }
}
