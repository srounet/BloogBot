using BloogBot.Game.Enums;
using System;
using System.Runtime.InteropServices;

namespace BloogBot.Game.Objects
{
    public class WoWUnit : WoWObject
    {
        private int WoWUnit_HealthOffset = 0x60;// 0x18;
        private int WoWUnit_TargetGuidOffset = 0x48;

        #region addresses

        private static uint _CanAttack = 0x00729740; //
        private static uint _HasAura = 0x007282A0; // CGUnit_C__HasAuraBySpellId

        #endregion addresses

        #region delegates

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate bool CanAttackDelegate(IntPtr objectPtr, IntPtr otherPtr);

        private CanAttackDelegate CanAttackFunction = MemoryManager.GetRegisterDelegate<CanAttackDelegate>(_CanAttack);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate bool HasAuraDelegate(IntPtr thisObj, int spellId);

        private static HasAuraDelegate HasAuraFunction = MemoryManager.GetRegisterDelegate<HasAuraDelegate>(_HasAura);

        #endregion delegates

        internal WoWUnit(IntPtr pointer, ulong guid, ObjectType objectType) : base(pointer, guid, objectType)
        {
        }

        // https://www.ownedcore.com/forums/world-of-warcraft/world-of-warcraft-bots-programs/wow-memory-editing/281613-help-net-injection-calling-cgobject_c-virtual-functions.html
        // https://www.ownedcore.com/forums/world-of-warcraft/world-of-warcraft-bots-programs/wow-memory-editing/298984-3-3-5a-12340-offsets.html

        public ulong TargetGuid => MemoryManager.ReadUlong(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_TARGET);

        /**
         * Target this Unit
        */

        public void Target()
        {
            Functions.SetTarget(Guid);
        }

        public void MoveTo()
        {
        }

        public int CastingId() => MemoryManager.ReadInt(Pointer + 0xA6C);

        public DynamicFlags DynamicFlags => (DynamicFlags)MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_DYNAMIC_FLAGS);
        public UnitFlags UnitFlags => (UnitFlags)MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_FLAGS);
        public MovementFlags MovementFlags => (MovementFlags)MemoryManager.ReadInt((IntPtr)MemoryManager.ReadInt(Pointer + 0xD8) + 0x44);
        public UnitReaction UnitReaction => Functions.GetUnitReaction(Pointer, ObjectManager.Me.Pointer);

        public bool IsMoving => MovementFlags.HasFlag(MovementFlags.MOVEFLAG_FORWARD);

        public bool IsAutoAttacking => MemoryManager.ReadInt(Pointer + 0xA20) != 0;

        public bool IsInCombat => UnitFlags.HasFlag(UnitFlags.UNIT_FLAG_IN_COMBAT);

        public bool CanBeLooted => Health == 0 && DynamicFlags.HasFlag(DynamicFlags.UNIT_DYNFLAG_LOOTABLE);

        public bool IsCasting => CastingId() != 0;

        public bool IsDead => Health <= 0 || DynamicFlags.HasFlag(DynamicFlags.UNIT_DYNFLAG_DEAD);

        public bool CanAttack()
        {
            if (Pointer == IntPtr.Zero)
                return false;

            if (!IsPlayer && IsDead)
                return false;

            return CanAttackFunction(ObjectManager.Me.Pointer, Pointer);
        }

        public bool HasAura(int spellId)
        {
            return HasAuraFunction(Pointer, spellId);
        }

        public int Level => MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_LEVEL);

        public int Health => MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_HEALTH);
        public int MaxHealth => MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_MAXHEALTH);
        public int HealthPercent => (int)(Health / (float)MaxHealth * 100);

        public int Mana => MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_POWER1);
        public int MaxMana => MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_MAXPOWER1);
        public int ManaPercent => (int)(Mana / (float)MaxMana * 100);

        public int Rage => MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_POWER2) / 10;
        public int MaxRage => MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_MAXPOWER2) / 10;

        public int Energy => MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_POWER4);
        public int MaxEnergy => MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_MAXPOWER4);

        /* https://www.ownedcore.com/forums/world-of-warcraft/world-of-warcraft-emulator-servers/83643-list-faction-ids.html
         * Orgrimmar - #85 | Stormwind - #11 | Thunder Bluff - #105 | Undercity - #68 | Silvermoon - #1602
        */
        public int FactionId => MemoryManager.ReadInt(GetDescriptorPtr() + (int)eUnitFields.UNIT_FIELD_FACTIONTEMPLATE);
    }
}