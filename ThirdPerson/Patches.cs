using ABI.CCK.Components;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using MelonLoader;
using System.Linq;
using System.Reflection;
using static ThirdPerson.CameraLogic;

namespace ThirdPerson;

internal static class Patches
{
    internal static void Apply(HarmonyLib.Harmony harmony)
    {
        harmony.Patch(
            typeof(ViewManager).GetMethods().FirstOrDefault(x => x.Name == nameof(ViewManager.UiStateToggle) && x.GetParameters().Length > 0),
            prefix: typeof(Patches).GetMethod(nameof(ToggleMainMenu), BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod()
        );
        harmony.Patch(
            typeof(CVR_MenuManager).GetMethod(nameof(CVR_MenuManager.ToggleQuickMenu)),
            prefix: typeof(Patches).GetMethod(nameof(ToggleQuickMenu), BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod()
        );
        harmony.Patch(
            typeof(PlayerSetup).GetMethod(nameof(PlayerSetup.CalibrateAvatar), BindingFlags.Public | BindingFlags.Instance),
            postfix: typeof(Patches).GetMethod(nameof(OnCalibrateAvatar), BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod()
         );
        harmony.Patch(
            typeof(CVRWorld).GetMethod("SetDefaultCamValues", BindingFlags.NonPublic | BindingFlags.Instance),
            postfix: typeof(Patches).GetMethod(nameof(OnWorldStart), BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod()
         );
        harmony.Patch(
            typeof(CVRWorld).GetMethod("CopyRefCamValues", BindingFlags.NonPublic | BindingFlags.Instance),
            postfix: typeof(Patches).GetMethod(nameof(OnWorldStart), BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod()
         );
    }

    private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance;
    private static readonly FieldInfo MainMenuOpen =
        typeof(ViewManager).GetField("_gameMenuOpen", Flags);
    private static readonly FieldInfo QuickMenuOpen =
        typeof(CVR_MenuManager).GetField("_quickMenuOpen", Flags);
    private static bool IsMmOpen => (bool)MainMenuOpen.GetValue(ViewManager.Instance);
    private static bool IsQmOpen => (bool)QuickMenuOpen.GetValue(CVR_MenuManager.Instance);
    private static void ToggleMainMenu(bool __0) => ToggleMenus(__0, true);
    private static void ToggleQuickMenu(bool __0) => ToggleMenus(__0, false);
    private static void ToggleMenus(bool isOpen, bool isMain)
    {
        if ((IsMmOpen && !isMain) || (IsQmOpen && isMain)) return;
        State = State switch
        {
            false when !isOpen && PreviousState => true,
            true when isOpen => false,
            _ => State
        };
    }
    //DesktopVRSwitch Support
    private static void OnCalibrateAvatar() => CheckVRSwitch();
    //Copy camera settings & postprocessing components
    private static void OnWorldStart() => CopyFromPlayerCam();
}