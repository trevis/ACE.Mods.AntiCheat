
using ACE.Mods.AntiCheat.Lib;
using ACE.Server.Network.Sequence;

namespace ACE.Mods.AntiCheat;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    internal static AntiBlink? AntiBlink { get; private set; }

    public override void Init()
    {
        base.Init();
    }

    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer?.Settings ?? new();
        StartServices();
    }

    private void StartServices()
    {
        if (Settings.EnableAntiBlink)
        {
            AntiBlink ??= new AntiBlink();
        }
        else {
            AntiBlink = null;
        }
    }

    protected override void SettingsChanged(object? sender, EventArgs e)
    {
        base.SettingsChanged(sender, e);
        Settings = SettingsContainer?.Settings ?? new();
        StartServices();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlayerPosition), new Type[] { typeof(Position), typeof(bool) })]
    public static bool PreUpdatePlayerPosition(Position newPosition, bool forceUpdate, ref Player __instance, ref bool __result)
    {
        try {
            return AntiBlink?.PreUpdatePlayerPosition(newPosition, forceUpdate, ref __instance, ref __result) != false;
        }
        catch (Exception ex) {
            Mod.Log($"Failed to call hook update player position: {ex.Message}", ModManager.LogLevel.Error);
        }
        
        return true;
    }

}