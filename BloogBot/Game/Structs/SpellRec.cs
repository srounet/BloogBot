using System.Runtime.InteropServices;

namespace BloogBot.Game.Structs
{
    #region SpellRec

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SpellRec
    {
        public uint Id;
        public uint Category;
        public uint Dispel;
        public int Mechanic;

        public uint Attributes; //group of flags
        public uint AttributesEx;
        public uint AttributesEx2;
        public uint AttributesEx3;
        public uint AttributesEx4;
        public uint AttributesEx5;
        public uint AttributesEx6;
        public uint AttributesEx7;

        public uint Stances;
        public uint unk_320_2;
        public uint StancesNot;
        public uint unk_320_3;
        public uint Targets;
        public uint TargetCreatureType;
        public uint RequiresSpellFocus;
        public uint FacingCasterFlags;
        public uint CasterAuraState;
        public uint TargetAuraState;
        public uint CasterAuraStateNot;
        public uint TargetAuraStateNot;
        public uint casterAuraSpell;
        public uint targetAuraSpell;
        public uint excludeCasterAuraSpell;
        public uint excludeTargetAuraSpell;
        public uint CastingTimeIndex;
        public uint RecoveryTime;
        public uint CategoryRecoveryTime;
        public uint InterruptFlags;
        public uint AuraInterruptFlags;
        public uint ChannelInterruptFlags;
        public uint procFlags;
        public uint procChance;
        public uint procCharges;
        public uint maxLevel;
        public uint baseLevel;
        public uint spellLevel;
        public uint DurationIndex;
        public int powerType;
        public uint manaCost;
        public uint manaCostPerlevel;
        public uint manaPerSecond;
        public uint manaPerSecondPerLevel;
        public uint rangeIndex;
        public float speed;
        public uint modalNextSpell;
        public uint StackAmount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)] public uint[] Totem;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I4)] public int[] Reagent;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U4)] public uint[] ReagentCount;
        public int EquippedItemClass;
        public int EquippedItemSubClassMask;
        public int EquippedItemInventoryTypeMask;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)] public int[] Effect;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I4)]
        public int[]
            EffectDieSides;

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I4)]
        //public int[] EffectBaseDice;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R4)]
        //public float[] EffectDicePerLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R4)]
        public float[]
            EffectRealPointsPerLevel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I4)]
        public int[]
            EffectBasePoints;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[]
            EffectMechanic;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[]
            EffectImplicitTargetA;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[]
            EffectImplicitTargetB;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[]
            EffectRadiusIndex;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[]
            EffectApplyAuraName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[]
            EffectAmplitude;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R4)]
        public float[]
            EffectMultipleValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[]
            EffectChainTarget;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[]
            EffectItemType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I4)]
        public int[]
            EffectMiscValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I4)]
        public int[]
            EffectMiscValueB;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        public uint[]
            EffectTriggerSpell;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R4)]
        public float[]
            EffectPointsPerComboPoint;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.Struct)]
        public Flag96[]
            EffectSpellClassMask;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)] public uint[] SpellVisual;
        public uint SpellIconID;
        public uint activeIconID;
        public uint spellPriority;

        [MarshalAs(UnmanagedType.LPStr)] public string SpellName;
        [MarshalAs(UnmanagedType.LPStr)] public string Rank;
        [MarshalAs(UnmanagedType.LPStr)] public string Description;
        [MarshalAs(UnmanagedType.LPStr)] public string ToolTip;

        public uint ManaCostPercentage;
        public uint StartRecoveryCategory;
        public uint StartRecoveryTime;
        public uint MaxTargetLevel;
        public uint SpellFamilyName;
        public Flag96 SpellFamilyFlags;
        public uint MaxAffectedTargets;
        public uint DmgClass;
        public uint PreventionType;
        public uint StanceBarOrder;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R4)]
        public float[]
            DmgMultiplier;

        public uint MinFactionId;
        public uint MinReputation;
        public uint RequiredAuraVision;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
        public uint[]
            TotemCategory;

        public int AreaGroupId;
        public int SchoolMask;
        public uint runeCostID;
        public uint spellMissileID;
        public uint PowerDisplayId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R4)] public float[] unk_320_4;
        public uint spellDescriptionVariableID;
        public uint SpellDifficultyId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Flag96
    {
        public uint flag1;
        public uint flag2;
        public uint flag3;
    }

    #endregion SpellRec
}