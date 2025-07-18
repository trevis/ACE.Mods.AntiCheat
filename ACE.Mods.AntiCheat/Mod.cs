
namespace ACE.Mods.AntiCheat;

public class Mod : BasicMod
{
    public Mod() : base() => Setup("ACE.Mods.AntiCheat", new PatchClass(this));

    internal static void Log(string message, ModManager.LogLevel info)
    {
        ModManager.Log($"[AntiCheat] {message}", info);
    }
}