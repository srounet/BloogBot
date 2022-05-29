using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BloogBot.Game.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct QuestLogEntry
    {
        public int ID;
        public QuestState State;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public short[] Objectives;
        public int Time;

        public WoWQuest AsWoWQuest()
        {
            return new WoWQuest(ID);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct QuestCacheRecord
    {
        public int Id;
        public int Method;
        public int Level;
        public int RequiredLevel;
        public int AreaIdOrSortId;
        public int InfoId;
        public int SuggestedPlayers;
        public int FriendlyFactionID;
        public int FriendlyFactionAmount;
        public int HostileFactionID;
        public int HostileFactionAmount;
        public int NextQuestId;
        public int XPId;
        public int RewardMoney;
        public int RewardMoneyInsteadOfXp;
        public int RewardSpellId;
        public int EffectOnPlayer;
        public int RewardHonor;
        public float RewardHonorBonus;
        public int StartingItemId;
        public int Flags;
        public fixed int RewardItem[4];
        public fixed int RewardItemCount[4];
        public fixed int RewardChoiceItem[6];
        public fixed int RewardChoiceItemCount[6];
        public int PointMapID;
        public float PointX;
        public float PointY;
        public int PointOptional;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)] public string Name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3000)] public string ObjectiveText;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3000)] public string Description;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)] public string EndText;
        public fixed int ObjectiveId[4];
        public fixed int ObjectiveRequiredCount[4];
        public fixed int CollectItemId[6];
        public fixed int CollectItemCount[6];
        public fixed int IntermediateItemId[4];
        public fixed int IntermediateItemCount[4];

        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        //public fixed byte OverrideObjectiveText[4][256]; // 4 * 256
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string OverrideObjectiveText1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string OverrideObjectiveText2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string OverrideObjectiveText3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string OverrideObjectiveText4;
        public int RewardTitleId;
        public int RequiredPlayersKilled;
        public int RewardTalentPoints;
        public int RewardArenaPoints;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)] public string CompletionText;
        public fixed int RewardReputationFaction[5];
        public fixed int FactionRewardID[5];
        public fixed int RewardReputationOverride[5];
        public int Unk17;
    }

    public enum QuestState
    {
        None = 0,
        Complete = 1,
        Fail = 2,
    }

    public struct QuestCache
    {
        public int BonusArenaPoints;
        public int BonusTalents;
        public uint CharTitleId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)] public string Description;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)] public string EndText;
        public uint Id;
        public uint Method;
        public uint MinLevel;
        public uint NextQuestInChain;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)] public string Objectives;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string OverrideObjectiveText1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string OverrideObjectiveText2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string OverrideObjectiveText3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string OverrideObjectiveText4;
        public int PlayersSlain;

        public uint PointMapId;
        public float PointX;
        public float PointY;
        public uint QuestFlags;
        public int QuestLevel;
        public uint RepObjectiveFaction1;
        public uint RepObjectiveFaction2;
        public int RepObjectiveValue1;
        public int RepObjectiveValue2;

        public int ReqCreatureOrGOId1;

        public int ReqCreatureOrGOId2;

        public int ReqCreatureOrGOId3;

        public int ReqCreatureOrGOId4;
        public int ReqCreatureOrGoCount1;
        public int ReqCreatureOrGoCount2;
        public int ReqCreatureOrGoCount3;
        public int ReqCreatureOrGoCount4;
        public int ReqItemCount1;

        public int ReqItemCount2;

        public int ReqItemCount3;

        public int ReqItemCount4;

        public int ReqItemCount5;

        public int ReqItemCount6;
        public int ReqItemId1;
        public int ReqItemId2;
        public int ReqItemId3;
        public int ReqItemId4;
        public int ReqItemId5;
        public int ReqItemId6;
        public int ReqSourceId1;
        public int ReqSourceId2;
        public int ReqSourceId3;
        public int ReqSourceId4;
        public int ReqSourceIdMaxCount1;
        public int ReqSourceIdMaxCount2;
        public int ReqSourceIdMaxCount3;
        public int ReqSourceIdMaxCount4;
        public uint RewardChoiceItemCount1;
        public uint RewardChoiceItemCount2;
        public uint RewardChoiceItemCount3;
        public uint RewardChoiceItemCount4;
        public uint RewardChoiceItemCount5;
        public uint RewardChoiceItemCount6;
        public uint RewardChoiceItemId1;
        public uint RewardChoiceItemId2;
        public uint RewardChoiceItemId3;
        public uint RewardChoiceItemId4;
        public uint RewardChoiceItemId5;
        public uint RewardChoiceItemId6;
        public uint RewardHonorAddition;
        public float RewardHonorMultiplier;
        public uint RewardItemCount1;
        public uint RewardItemCount2;
        public uint RewardItemCount3;
        public uint RewardItemCount4;
        public uint RewardItemId1;
        public uint RewardItemId2;
        public uint RewardItemId3;
        public uint RewardItemId4;
        public uint RewardMoneyMaxLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)] public uint[] RewardReputationFactions;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)] public int[] RewardReputationFactionsValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)] public uint[] RewardReputationValue;
        public uint RewardSpell;
        public uint RewardSpellCast;
        public int RewardXpId;
        public uint SourceItemId;
        public int SuggestedPlayers;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)] public string Title;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)] public string ToDoText;
        public uint Type;
        public int Unk2;
        public int Unk3;
        public int ZoneOrSort;
    }
}