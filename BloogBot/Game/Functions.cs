using BloogBot.Game.Enums;
using BloogBot.Game.Objects;
using BloogBot.Game.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace BloogBot.Game
{
    static public class Functions
    {
        // https://www.ownedcore.com/forums/world-of-warcraft/world-of-warcraft-bots-programs/wow-memory-editing/298984-3-3-5a-12340-offsets.html

        #region static addresses

        private static uint _ClntObjMgrGetActivePlayer = 0x004d3790;
        private static uint _EnumVisibleObjects = 0x004D4B30;
        private static uint _GetObjectPtr = 0x004D4DB0;  // ClntObjMgrObjectPtr
        private static uint _ClickToMovePtr = 0x00727400;  // CGPlayer_C__ClickToMove
        private static uint _ClickToMoveStopPtr = 0x0072b3a0;  // CGPlayer_C__ClickToMoveStop
        private static uint _LuaDoString = 0x00819210; // FrameScript__Execute
        private static uint _GetLocalizedText = 0x007225E0; // FrameScript__GetLocalizedText
        private static uint _SetTarget = 0x00524BF0; // CGGameUI__Target
        private static uint _SetFacing = 0x0072EA50;
        private static uint _PerformanceCounter = 0x0086AE20;
        private static uint _GetSpellCooldown = 0x00809000; // Spell_C__GetSpellCooldown_Proxy
        private static uint _UnitReaction = 0x007251C0; // CGUnit_C__UnitReaction
        private static uint _RetrieveCorpse = 0x0051B800; // Script_RetrieveCorpse
        private static uint _RepopMe = 0x0051AA90; // Script_RepopMe
        private static uint _GetInfoBlockById = 0x0067DE90; // DbQuestCache_GetInfoBlockById

        #endregion static addresses

        #region GetInfoBlockById

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr GetQuestInfoBlockByIdDelegate(IntPtr instance, int a2, ref int a3, int a4 = 0, int a5 = 0, int a6 = 0);

        private static GetQuestInfoBlockByIdDelegate GetQuestInfoBlockByIdFunction = MemoryManager.GetRegisterDelegate<GetQuestInfoBlockByIdDelegate>((IntPtr)_GetInfoBlockById);

        public static QuestCacheRecord GetQuestRecordFromId(int id)
        {
            uint WdbQuestCache = 0x00C5DA48;
            IntPtr recPtr = GetQuestInfoBlockByIdFunction((IntPtr)WdbQuestCache, id, ref id);
            return MemoryManager.InternalRead<QuestCacheRecord>(recPtr);
        }

        #endregion GetInfoBlockById

        #region RepopMe

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RepopMeDelegate(IntPtr pointer);

        private static RepopMeDelegate RepopMeFunction = MemoryManager.GetRegisterDelegate<RepopMeDelegate>((IntPtr)_RepopMe);

        public static void RepopMe()
        {
            RepopMeFunction(ObjectManager.Me.Pointer);
        }

        #endregion RepopMe

        #region RetrieveCorpse

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RetrieveCorpseDelegate();

        private static RetrieveCorpseDelegate RetrieveCorpseFunction = MemoryManager.GetRegisterDelegate<RetrieveCorpseDelegate>((IntPtr)_RetrieveCorpse);

        public static void RetrieveCorpse()
        {
            RetrieveCorpseFunction();
        }

        #endregion RetrieveCorpse

        #region UnitReaction

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int GetUnitReactionDelegate(IntPtr thisObj, IntPtr unitToCompare);

        private static GetUnitReactionDelegate GetUnitReactionFunction = MemoryManager.GetRegisterDelegate<GetUnitReactionDelegate>((IntPtr)_UnitReaction);

        public static UnitReaction GetUnitReaction(IntPtr UnitPointer, IntPtr PlayerPointer)
        {
            return (UnitReaction)GetUnitReactionFunction(UnitPointer, PlayerPointer);
        }

        #endregion UnitReaction

        #region SpellCooldown

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool GetSpellCooldownDelegate(uint spellId, bool isPet, ref int duration, ref int start, ref bool isEnabled, ref int unk0);

        private static GetSpellCooldownDelegate GetSpellCooldownFunction = MemoryManager.GetRegisterDelegate<GetSpellCooldownDelegate>((IntPtr)_GetSpellCooldown);

        public static float GetSpellCoolDown(uint id)
        {
            int start = 0;
            int duration = 0;
            bool isReady = false;
            int unk0 = 0;

            GetSpellCooldownFunction(id, false, ref duration, ref start, ref isReady, ref unk0);

            int result = start + duration - (int)PerformanceCounter();
            return isReady ? (result > 0 ? result / 1000f : 0f) : float.MaxValue;
        }

        #endregion SpellCooldown

        #region PerformanceCounter

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint PerformanceCounterDelegate();

        private static PerformanceCounterDelegate PerformanceCounterFunction = MemoryManager.GetRegisterDelegate<PerformanceCounterDelegate>((IntPtr)_PerformanceCounter);

        public static uint PerformanceCounter() => PerformanceCounterFunction();

        #endregion PerformanceCounter

        #region SetFacing

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void SetFacingDelegate(IntPtr thisObj, uint time, float angle);

        private static readonly SetFacingDelegate SetFacingFunction = MemoryManager.GetRegisterDelegate<SetFacingDelegate>((IntPtr)_SetFacing);

        public static void SetFacing(IntPtr Pointer, uint time, float angle) => SetFacingFunction(Pointer, time, angle);

        #endregion SetFacing

        #region SetTarget

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SetUITargetDelegate(ulong guidPointer);

        public static readonly SetUITargetDelegate SetTargetFunction = MemoryManager.GetRegisterDelegate<SetUITargetDelegate>((IntPtr)_SetTarget);

        public static int SetTarget(ulong guidPointer) => ThreadSynchronizer.RunOnMainThread(() => SetTargetFunction(guidPointer));

        #endregion SetTarget

        #region GetPlayerGuid

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ulong GetPlayerGuidDelegate();

        private static readonly GetPlayerGuidDelegate GetPlayerGuidFunction = MemoryManager.GetRegisterDelegate<GetPlayerGuidDelegate>((IntPtr)_ClntObjMgrGetActivePlayer);

        static internal ulong GetPlayerGuid() => ThreadSynchronizer.RunOnMainThread(() => GetPlayerGuidFunction());

        #endregion GetPlayerGuid

        #region EnumVisibleObjects

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint EnumVisibleObjectsDelegate(IntPtr callback, int filter);

        private static readonly EnumVisibleObjectsDelegate EnumVisibleObjectsFunction = MemoryManager.GetRegisterDelegate<EnumVisibleObjectsDelegate>((IntPtr)_EnumVisibleObjects);

        static internal uint EnumVisibleObjects(IntPtr callback, int filter) => ThreadSynchronizer.RunOnMainThread(() => EnumVisibleObjectsFunction(callback, filter));

        #endregion EnumVisibleObjects

        #region GetObjectPtr

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr GetObjectPtrDelegate(ulong guid);

        private static readonly GetObjectPtrDelegate GetObjectPtrFunction = MemoryManager.GetRegisterDelegate<GetObjectPtrDelegate>((IntPtr)_GetObjectPtr);

        static public IntPtr GetObjectPtr(ulong guid) => GetObjectPtrFunction(guid);

        #endregion GetObjectPtr

        #region ClickToMove

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void ClickToMoveDelegate(IntPtr playerPtr, ClickToMoveType clickType, ref ulong interactGuidPtr, ref XYZ positionPtr, float precision);

        private static readonly ClickToMoveDelegate ClickToMoveFunction = MemoryManager.GetRegisterDelegate<ClickToMoveDelegate>((IntPtr)_ClickToMovePtr);

        internal static void ClickToMove(IntPtr playerPtr, ClickToMoveType clickType, Position position)
        {
            ulong interactGuidPtr = 0;
            var xyz = new XYZ(position.X, position.Y, position.Z);
            ClickToMoveFunction(playerPtr, clickType, ref interactGuidPtr, ref xyz, 2);
        }

        internal static void ClickToMove(IntPtr playerPtr, ClickToMoveType clickType, Position position, ulong interactGuidPtr)
        {
            var xyz = new XYZ(position.X, position.Y, position.Z);
            ClickToMoveFunction(playerPtr, clickType, ref interactGuidPtr, ref xyz, 2);
        }

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void ClickToMoveStopDelegate(IntPtr playerPtr);

        private static readonly ClickToMoveStopDelegate ClickToMoveStopFunction = MemoryManager.GetRegisterDelegate<ClickToMoveStopDelegate>((IntPtr)_ClickToMoveStopPtr);

        // does not seems to work as expected, won't stop movement.
        internal static void ClickToMoveStop(IntPtr playerPtr) => ClickToMoveStopFunction(playerPtr);

        #endregion ClickToMove

        #region PatchLuaProtect

        public static void PatchLuaProtect()
        {
            MemoryManager.WriteBytes((IntPtr)0x5191C0, new byte[] { 0xB8, 0x01, 0x0, 0x0, 0x0 });
            MemoryManager.WriteByte((IntPtr)0x5191C5, 0xC3);
            MemoryManager.WriteByte((IntPtr)0x5191C6, 0x90);
            MemoryManager.WriteByte((IntPtr)0x5191C7, 0x90);
            MemoryManager.WriteByte((IntPtr)0x5191C8, 0x90);
            MemoryManager.WriteByte((IntPtr)0x5191C9, 0x90);
        }

        #endregion PatchLuaProtect

        #region LuaDoString

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void Lua_DoStringDelegate(string command, string fileName, uint hwEvent); // hwEvent should always be passed as zero

        private static Lua_DoStringDelegate DoStringFunction = MemoryManager.GetRegisterDelegate<Lua_DoStringDelegate>((IntPtr)_LuaDoString);

        static public void DoString(string command) => ThreadSynchronizer.RunOnMainThread(() => DoStringFunction(command, "plugin.lua", 0));

        #endregion LuaDoString

        #region GetLocalizedText

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetLocalizedTextDelegate(IntPtr pointer, string returnString, int negativeone);

        public static GetLocalizedTextDelegate GetLocalizedTextFunction = MemoryManager.GetRegisterDelegate<GetLocalizedTextDelegate>((IntPtr)_GetLocalizedText);

        static public string[] GetLocalizedText(string[] varNames)
        {
            var results = new List<string>();

            foreach (var varName in varNames)
            {
                uint address = ThreadSynchronizer.RunOnMainThread(() => GetLocalizedTextFunction(ObjectManager.Me.Pointer, varName, -1));
                results.Add(MemoryManager.ReadString((IntPtr)address, 5120));
            }

            return results.ToArray();
        }

        #endregion GetLocalizedText

        #region DoStringWithResult

        private static string GetRandomLuaVarName()
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(8).ToArray());
        }

        static public string[] DoStringWithResult(string code)
        {
            var luaVarNames = new List<string>();
            for (var i = 0; i < 11; i++)
            {
                var currentPlaceHolder = "{" + i + "}";
                if (!code.Contains(currentPlaceHolder)) break;
                var randomName = GetRandomLuaVarName();
                code = code.Replace(currentPlaceHolder, randomName);
                luaVarNames.Add(randomName);
            }

            DoString(code);

            var results = GetLocalizedText(luaVarNames.ToArray());

            return results.ToArray();
        }

        #endregion DoStringWithResult
    }
}