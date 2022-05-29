using BloogBot.Game.Enums;
using BloogBot.Game.Structs;
using System;

namespace BloogBot.Game.Objects
{
    public class WoWPlayerMe : WoWPlayer
    {
        internal WoWPlayerMe(IntPtr pointer, ulong guid, ObjectType objectType) : base(pointer, guid, objectType)
        {
            // RefreshSpells();
        }

        public WoWUnit Target { get; set; }

        public void LookAt(WoWUnit unit)
        {
            Functions.ClickToMove(Pointer, ClickToMoveType.FaceTarget, Position, unit.Guid);
        }

        public Position Corpse => new Position(MemoryManager.InternalRead<XYZ>((IntPtr)0x00BD0A50 + 0x8));

        public void SetFacing(float angle)
        {
            const float pi2 = (float)(Math.PI * 2);
            if (angle < 0.0f)
                angle += pi2;
            if (angle > pi2)
                angle -= pi2;
            Functions.SetFacing(Pointer, Functions.PerformanceCounter(), angle);
        }

        public void MoveTo(Position position)
        {
            Functions.ClickToMove(Pointer, ClickToMoveType.Move, position);
        }

        public void MoveToStop()
        {
            Functions.ClickToMove(Pointer, ClickToMoveType.Move, Position);
        }

        public void Loot(WoWUnit target)
        {
            target.Interact();
        }

        #region Lua

        public ulong Copper => (Convert.ToUInt64(Functions.DoStringWithResult("{0} = GetMoney()")[0]));

        // https://wowpedia.fandom.com/wiki/UiMapID
        public uint GetCurrentMapAreaID => (Convert.ToUInt32(Functions.DoStringWithResult("{0} = GetCurrentMapAreaID()")[0]));

        #endregion Lua
    }
}