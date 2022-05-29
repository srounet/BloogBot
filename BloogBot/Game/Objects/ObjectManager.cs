using BloogBot.Game.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BloogBot.Game.Objects
{
    public static class ObjectManager
    {
        private const int OBJECT_TYPE_OFFSET = 0x14;

        #region delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int EnumerateVisibleObjectsCallback(ulong guid, int filter);

        private static EnumerateVisibleObjectsCallback callback;
        private static IntPtr callbackPtr;

        #endregion delegates

        private static ulong playerGuid;
        static public WoWUnit CurrentTarget => Units.FirstOrDefault(u => Me.TargetGuid == u.Guid);
        internal static IList<WoWObject> Objects = new List<WoWObject>();
        internal static IList<WoWObject> ObjectsBuffer = new List<WoWObject>();

        #region world objects

        public static WoWPlayerMe Me { get; private set; }
        public static IEnumerable<WoWUnit> Units => Objects.OfType<WoWUnit>().Where(o => o.ObjectType == ObjectType.Unit).ToList();
        public static IEnumerable<WoWPlayer> Players => Objects.OfType<WoWPlayer>();
        public static IEnumerable<WoWItem> Items => Objects.OfType<WoWItem>();
        public static IEnumerable<WoWGameObject> GameObjects => Objects.OfType<WoWGameObject>();

        //         public static QuestCollection Quests { get; private set; }
        public static QuestCollection Quests = new QuestCollection();

        #endregion world objects

        static internal void Initialize()
        {
            callback = Callback;
            callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
        }

        #region Functions

        static public bool IsLoggedIn => Functions.GetPlayerGuid() > 0;

        static public int ZoneID => MemoryManager.ReadInt((IntPtr)0x00BD080C);

        // 0	Eastern Kingdoms
        // 1	Kalimdor
        // 530	Outland
        // 571	Northrend
        static public int MapID => MemoryManager.ReadInt((IntPtr)0x00AB63BC);

        static public WoWUnit FindClosestTarget()
        {
            return ObjectManager
                    .Units
                    .Where(u => u != null && u.Name != null && u.Position != null)
                    .Where(u => u.Health > 0)
                    .Where(u => u.FactionId != 85 && u.FactionId != 29 && u.FactionId != 31 && u.FactionId != 35 && u.FactionId != 126 && u.FactionId != 877)
                    .Where(u => u.CanAttack())
                    .OrderBy(u => Navigation.DistanceViaPath((uint)ObjectManager.MapID, ObjectManager.Me.Position, u.Position))
                    .FirstOrDefault();
        }

        static public IEnumerable<WoWUnit> Aggressors =>
                Units
                    .Where(u => u.Health > 0)
                    .Where(u =>
                        u.TargetGuid == Me?.Guid)
                    .Where(u =>
                        u.UnitReaction == UnitReaction.Hostile ||
                        u.UnitReaction == UnitReaction.Unfriendly ||
                        u.UnitReaction == UnitReaction.Neutral)
                    .Where(u => u.IsInCombat);

        #endregion Functions

        #region enumerateObjects

        static internal async void StartEnumeration()
        {
            while (true)
            {
                try
                {
                    EnumerateVisibleObjects();
                    await Task.Delay(500);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void EnumerateVisibleObjects()
        {
            ThreadSynchronizer.RunOnMainThread(() =>
            {
                if (IsLoggedIn)
                {
                    playerGuid = Functions.GetPlayerGuid();
                    ObjectsBuffer.Clear();
                    if (IsLoggedIn)
                        Functions.EnumVisibleObjects(callbackPtr, 0);
                    Objects = new List<WoWObject>(ObjectsBuffer);
                }
            });
        }

        private static int Callback(ulong guid, int filter)
        {
            var pointer = Functions.GetObjectPtr(guid);
            var objectType = (ObjectType)MemoryManager.ReadInt(IntPtr.Add(pointer, OBJECT_TYPE_OFFSET));

            switch (objectType)
            {
                case ObjectType.GameObject:
                    ObjectsBuffer.Add(new WoWGameObject(pointer, guid, objectType));
                    break;

                case ObjectType.Container:
                case ObjectType.Item:
                    ObjectsBuffer.Add(new WoWItem(pointer, guid, objectType));
                    break;

                case ObjectType.Player:
                    if (guid == playerGuid)
                    {
                        var player = new WoWPlayerMe(pointer, guid, objectType);
                        Me = player;
                        ObjectsBuffer.Add(player);
                    }
                    else
                        ObjectsBuffer.Add(new WoWPlayer(pointer, guid, objectType));
                    break;

                case ObjectType.Unit:
                    ObjectsBuffer.Add(new WoWUnit(pointer, guid, objectType));
                    break;
            }

            return 1;
        }

        #endregion enumerateObjects
    }
}