
namespace ACE.Mods.AntiCheat;

public class Settings
{
    /// <summary>
    /// Enable anti-blink, which prevents players from teleporting through closed doors
    /// </summary>
    public bool EnableAntiBlink { get; set; } = true;
    public double AntiBlinkLogIntervalMilliseconds { get; set; } = 5000;
}


