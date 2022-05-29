using BloogBot.Game.Enums;
using System;

namespace BloogBot.Game.Objects
{
    public class WoWPlayer : WoWUnit
    {
        internal WoWPlayer(IntPtr pointer, ulong guid, ObjectType objectType) : base(pointer, guid, objectType)
        {
        }

        public bool IsGhost => (MemoryManager.ReadInt(GetDescriptorPtr() + (int)ePlayerFields.PLAYER_FLAGS) & (1 << 4)) > 0;
        // (GetDescriptor<uint>(WoWPlayerFields.PLAYER_FLAGS) & (1 << 4)) > 0;
    }

    // public bool IsEating => HasBuff("Food");

    // public bool IsDrinking => HasBuff("Drink");
}