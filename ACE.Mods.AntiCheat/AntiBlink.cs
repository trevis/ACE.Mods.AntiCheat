
using ACE.Mods.AntiCheat.Lib;
using ACE.Server.Network.Sequence;

namespace ACE.Mods.AntiCheat
{
    internal class AntiBlink
    {
        private Settings Settings => PatchClass.Settings;
        private DateTime _serverStart = DateTime.UtcNow;

        public AntiBlink()
        {
            Mod.Log($"Enabling AntiBlink: AntiBlinkMonsterDoors:{Settings.AntiBlinkMonsterDoors}", ModManager.LogLevel.Info);
        }


        internal bool PreUpdatePlayerPosition(Position newPosition, bool forceUpdate, ref Player __instance, ref bool __result)
        {
            var now = DateTime.UtcNow;
            var currentPosition = __instance.GetPosition(PositionType.Location);

            // get all visible doors that aren't ethereal, and monster doors like "Mana Barrier"
            var objsToCheck = __instance.PhysicsObj.ObjMaint.GetVisibleObjects(__instance.PhysicsObj.CurCell)
                .Where((obj) => {
                    // TODO: probably could check this zlevel in a better way...
                    // This could also use some filtering to only get nearby doors instead of all visible,
                    // but this is pretty fast in benchmarks so i'm not too worried currently
                    if (Math.Abs(obj.Position.Frame.Origin.Z - currentPosition.PositionZ) > 6f)
                    {
                        return false;
                    }

                    // ethereal doors
                    if (obj.WeenieObj.WorldObject is Door door && door.PhysicsObj?.State.HasFlag(PhysicsState.Ethereal) != true) {
                        return true;
                    }

                    // monster doors (Mana Barrier)
                    if (Settings.AntiBlinkMonsterDoors && obj.WeenieObj.IsMonster && obj.WeenieObj.WorldObject.GetProperty(PropertyBool.Stuck) == true) {
                        return true;
                    }

                    return false;
                });

            foreach (var visibleObj in objsToCheck)
            {
                Vector3? collisionPoint = null;
                var wo = visibleObj.WeenieObj.WorldObject;
                // non-ethereal doors
                if (wo is Door door)
                {
                    collisionPoint = CollisionHelpers.GetDoorCollisionPoint(currentPosition, newPosition, wo);
                }
                // monster doors (Mana Barrier)
                else if (Settings.AntiBlinkMonsterDoors && visibleObj.WeenieObj.IsMonster && visibleObj.WeenieObj.WorldObject.GetProperty(PropertyBool.Stuck) == true)
                {
                    collisionPoint = CollisionHelpers.GetDoorCollisionPoint(currentPosition, newPosition, wo);
                }

                // if there was a collision, cancel current move and send a force position
                if (collisionPoint.HasValue)
                {
                    var lastBlink = __instance.GetProperty(PropertyFloat.AbuseLoggingTimestamp) ?? 0;
                    if (Math.Abs(lastBlink - (_serverStart - now).TotalMilliseconds) > Settings.AntiBlinkLogIntervalMilliseconds)
                    {
                        __instance.SetProperty(PropertyFloat.AbuseLoggingTimestamp, (_serverStart - now).TotalMilliseconds);
                        Mod.Log($"Player {__instance.Name} attempted to blink through (0x{wo.Guid.Full:X8} {wo.Name}) at {currentPosition}", ModManager.LogLevel.Info);
                    }
                    __instance.Sequences.GetNextSequence(SequenceType.ObjectForcePosition);
                    __instance.SendUpdatePosition();
                    __result = false;

                    return false;
                }
            }

            return true;
        }
    }
}