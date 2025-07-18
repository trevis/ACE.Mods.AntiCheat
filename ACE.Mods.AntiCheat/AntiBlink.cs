
using ACE.Mods.AntiCheat.Lib;
using ACE.Server.Network;
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
            // bail early if the player is teleporting
            if (__instance.Teleporting) {
                return true;
            }

            var now = DateTime.UtcNow;
            var currentPosition = __instance.GetPosition(PositionType.Location);

            // This could also use some filtering to only get nearby doors instead of all visible,
            // but this is pretty fast in benchmarks so i'm not too worried currently
            foreach (var obj in __instance.PhysicsObj.ObjMaint.GetVisibleObjectsValues())
            {
                // TODO: probably could check this zlevel in a better way...
                if (Math.Abs(obj.Position.Frame.Origin.Z - currentPosition.PositionZ) > 6f)
                {
                    continue;
                }

                Vector3? collisionPoint = null;
                var wo = obj.WeenieObj?.WorldObject;

                if (wo == null) {
                    continue;
                }

                // non-ethereal doors
                if (IsNonEtherealDoor(obj))
                {
                    collisionPoint = CollisionHelpers.GetDoorCollisionPoint(currentPosition, newPosition, wo);
                }
                // monster doors (Mana Barrier)
                else if (Settings.AntiBlinkMonsterDoors && IsMonsterDoor(obj))
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

        private bool IsMonsterDoor(PhysicsObj obj)
        {
            return 
                obj.WeenieObj != null
                && obj.WeenieObj.IsMonster
                && obj.WeenieObj.WorldObject.GetProperty(PropertyBool.AiImmobile) == true
                && obj.WeenieObj.WorldObject.GetProperty(PropertyInt.CreatureType) == (int)CreatureType.Wall;
        }

        private bool IsNonEtherealDoor(PhysicsObj obj)
        {
            return 
                obj.WeenieObj != null
                && obj.WeenieObj.WorldObject is Door door
                && obj.State.HasFlag(PhysicsState.Ethereal) != true;
        }

    }
}