## 1.6
- Skip AntiCheat checks for admins / cloaked players

## 1.5
- Refactoring, cleanup
- Better null checking
- Cleaner player teleport check during AntiBlink

## 1.4
- Fixed false positive in AntiBlink when teleporting within the same dungeon/landblock. (thanks ripley)

## 1.3
- Fixed MonsterDoor check to use AiImmobile / CreatureType:Wall instead of Stuck. (thanks paradox)

## 1.2
- Added setting AntiBlinkMonsterDoors to add checks to monster doors like Mana Barrier.

## 1.1
- AntiBlink now uses PhysicsObject state ethereal instead of weenie property, to stay in sync better

## 1.0.0
- Initial Release