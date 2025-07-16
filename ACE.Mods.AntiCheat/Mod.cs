namespace ACE.Mods.AntiCheat;

public class Mod : BasicMod
{
    public Mod() : base() => Setup("ACE.Mods.AntiCheat", new PatchClass(this));
}