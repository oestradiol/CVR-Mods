using System;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;
using System.Collections;
using System.Linq;
using MelonLoader;
using UnityEngine.UI;
using static DiscordMute.DiscordMute;

namespace DiscordMute;

public static class BindManager
{
    // BlackList Mouse Buttons + Extra keys (L | R global)
    private const Keys BlacklistedKeyFlags = Keys.LButton | Keys.RButton | Keys.ControlKey | Keys.ShiftKey | Keys.Menu;
    private static readonly List<Keys> AllowedKeys = Enum.GetValues(typeof(Keys))
        .Cast<Keys>().Where(k => !BlacklistedKeyFlags.HasFlag(k)).ToList();
        
    private static readonly HashSet<Keys> SelectedKeys = new();
    private static Text _textComponent;
    private static object _keyWaitingCoroutine;

    public static void Initialize()
    {
        // Todo: Create page, title: "Press the mute keys on your keyboard", label: "Waiting for key..."
        
        // Todo: Save label Text component to _textComponent
        _textComponent = null;

        void CloseBindManager()
        {
            if (_keyWaitingCoroutine != null)
                MelonCoroutines.Stop(_keyWaitingCoroutine);
            
            // Todo: Hide Popup/Page
            
            SelectedKeys.Clear();
        }
        
        void AcceptKeys()
        {
            MuteKey.Value = SelectedKeys.Select(k => (byte)k).ToList();
            CloseBindManager();
        }
        
        // Todo: Add Clear button (Method: SelectedKeys.Clear)
        // Todo: Add Accept button (Method: AcceptKeys)
        // Todo: Add Cancel button (Method: CloseBindManager)
    }

    public static void Show()
    {
        // Todo: Show Popup/Page
        
        _keyWaitingCoroutine = MelonCoroutines.Start(WaitForKeys());
    }

    private static IEnumerator WaitForKeys()
    {
        foreach (var k in AllowedKeys.Where(KeyboardManager.IsKeyDown))
        {
            if (SelectedKeys.Count >= 4) break;
            SelectedKeys.Add(k);
        }

        _textComponent.GetComponentInChildren<Text>().text = SelectedKeys.Count == 0 ? 
            "Waiting for key..." : string.Join(" + ", SelectedKeys.Select(GetKeyName));
            
        yield return new WaitForEndOfFrame();
    }
    
    private static string GetKeyName(Keys key) =>
        key switch
        {
            Keys.LMenu => "ALT",
            Keys.RMenu => "RIGHT ALT",
            Keys.LControlKey => "CTRL",
            Keys.RControlKey => "RIGHT CTRL",
            Keys.LShiftKey => "SHIFT",
            Keys.RShiftKey => "RIGHT SHIFT",
            Keys.XButton1 => "MOUSE3",
            Keys.XButton2 => "MOUSE4",
            _ => key.ToString()
        };
}