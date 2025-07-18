
namespace ACE.Mods.AntiCheat;

public class Settings
{
    /// <summary>
    /// Whether or not admins are immune to the anti-cheat
    /// </summary>
    public bool AdminsAreImmune { get; set; } = true;

    /// <summary>
    /// Whether or not cloaked players are immune to the anti-cheat
    /// </summary>
    public bool CloakedPlayersAreImmune { get; set; } = true;

    /// <summary>
    /// Enable anti-blink, which prevents players from teleporting through closed doors
    /// </summary>
    public bool EnableAntiBlink { get; set; } = true;

    /// <summary>
    /// The number of milliseconds between anti-blink logging for a specific player
    /// </summary>
    public double AntiBlinkLogIntervalMilliseconds { get; set; } = 5000;

    /// <summary>
    /// "Monster Doors" like Mana Barrier will be considered during the anti-blink check when enabled.
    /// 
    /// Mana Barrier Example: 0x00E5018F [89.917587 -220.604492 -77.994995] 0.995699 0.000000 0.000000 0.092643
    /// </summary>
    public bool AntiBlinkMonsterDoors { get; set; } = true;
}


