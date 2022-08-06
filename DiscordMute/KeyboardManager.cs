using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DiscordMute;

public static class KeyboardManager
{
    #region Key Checking
    internal static bool IsKeyDown(Keys key) => GetKeyState(key).HasFlag(KeyStates.Down);
    
    private static KeyStates GetKeyState(Keys key) => (KeyStates)GetKeyState((int)key);
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern short GetKeyState(int keyCode);
    
    [Flags] private enum KeyStates { Down = 0x8000 }
    #endregion

    #region Key Pressing
    internal static void PressKeys(List<byte> keys)
    {
        keys.ForEach(HoldKey);
        keys.ForEach(ReleaseKey);
    }
    
    private static void HoldKey(byte key) =>
        keybd_event(key, key, 0, 0);
    
    private static void ReleaseKey(byte key) =>
        keybd_event(key, key, 0x0002, 0);
    
    [DllImport("user32.dll", SetLastError = true)] // ReSharper disable once IdentifierTypo
    private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
    #endregion
}