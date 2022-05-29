using BloogBot.Game.Enums;
using System;

namespace BloogBot.Game.Objects
{
    public class WoWItem : WoWObject
    {
        internal WoWItem(IntPtr pointer, ulong guid, ObjectType objectType) : base(pointer, guid, objectType)
        {
        }
    }
}