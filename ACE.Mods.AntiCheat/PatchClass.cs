
using ACE.Mods.AntiCheat.Lib;
using ACE.Server.Network.Sequence;

namespace ACE.Mods.AntiCheat;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    private static DateTime _serverStart = DateTime.Now;

    public override void Init()
    {
        base.Init();
    }

    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer?.Settings ?? new();
    }

    public override void Stop()
    {
        base.Stop();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlayerPosition), new Type[] { typeof(Position), typeof(bool) })]
    public static bool PreUpdatePlayerPosition(Position newPosition, bool forceUpdate, ref Player __instance, ref bool __result)
    {
        try
        {
            // bail if not enabled
            if (Settings.EnableAntiBlink != true)
            {
                return true;
            }

            var currentPosition = __instance.GetPosition(PositionType.Location);

            // get all visible doors, that aren't ethereal
            var doors = __instance.PhysicsObj.ObjMaint.GetVisibleObjects(__instance.PhysicsObj.CurCell)
                .Where((obj) => {
                    // TODO: probably could check this zlevel in a better way...
                    // This could also use some filtering to only get nearby doors instead of all visible,
                    // but this is pretty fast in benchmarks so i'm not too worried currently
                    if (Math.Abs(obj.Position.Frame.Origin.Z - currentPosition.PositionZ) > 6f) {
                        return false;
                    }

                    return obj.WeenieObj.WorldObject is Door door && door.Ethereal != true;
                });

            foreach (var visibleObj in doors)
            {
                // check if a line from the players old position to its new position crosses this door
                var door = visibleObj.WeenieObj.WorldObject as Door;
                var collisionPoint = CollisionHelpers.GetCollisionPoint(currentPosition, newPosition, door);

                if (collisionPoint.HasValue)
                {
                    var lastBlink = __instance.GetProperty(PropertyFloat.AbuseLoggingTimestamp) ?? 0;
                    if (Math.Abs(lastBlink - (_serverStart - DateTime.Now).TotalMilliseconds) > Settings.AntiBlinkLogIntervalMilliseconds) {
                        __instance.SetProperty(PropertyFloat.AbuseLoggingTimestamp, (_serverStart - DateTime.Now).TotalMilliseconds);
                        
                        ModManager.Log($"Player {__instance.Name} blinked through a door at {currentPosition}");
                    }
                    __instance.Sequences.GetNextSequence(SequenceType.ObjectForcePosition);
                    __instance.SendUpdatePosition();
                    __result = false;
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ModManager.Log($"PreUpdatePlayerPosition (AntiBlink): {ex.Message}", ModManager.LogLevel.Error);
        }
        
        return true;
    }

}