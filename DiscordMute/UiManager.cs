using System.Collections;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.InteractionSystem;
using static DiscordMute.DiscordMute;

namespace DiscordMute;

internal static class UiManager
{
    internal static void OnApplicationStart()
    {
        IEnumerator WaitUi()
        {
            yield return new WaitUntil(() => CVR_MenuManager.Instance != null);
            OnUiManagerInit();
        }
        MelonCoroutines.Start(WaitUi());
    }

    private static void OnUiManagerInit()
    {
        DiscordMute.Logger.Msg("Initializing UI integration...");
        
        BindManager.Initialize();

        CreateBindPopupButton();
        CreateQuickMenuButton();
        
        DiscordMute.Logger.Msg("UI integration done!");
    }

    private static void CreateBindPopupButton()
    {
        // Todo: Add button (Method: BindManager.Show)
    }

    private static void CreateQuickMenuButton()
    {
        void OnMuteUnmute()
        {
            var muteKeys = MuteKey.Value;
            if (muteKeys?.Count != 0)
                KeyboardManager.PressKeys(muteKeys);
            else BindManager.Show();
        };
        // Todo: Add button (Method: OnMuteUnmute)
    }
}