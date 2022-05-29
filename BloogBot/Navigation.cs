using BloogBot.Game;
using BloogBot.Game.Structs;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BloogBot
{
    public static unsafe class Navigation
    {
        #region delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate XYZ* CalculatePathDelegate(uint mapId, XYZ start, XYZ end, bool straightPath, out int length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FreePathArr(XYZ* pathArr);

        #endregion delegates

        public static CalculatePathDelegate calculatePath;
        public static FreePathArr freePathArr;

        static Navigation()
        {
            var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var mapsPath = $"{currentFolder}\\Navigation.dll";
            IntPtr navProcPtr = MemoryManager.LoadLibrary(mapsPath);

            var calculatePathPtr = MemoryManager.GetProcAddress(navProcPtr, "CalculatePath");
            calculatePath = MemoryManager.GetRegisterDelegate<CalculatePathDelegate>(calculatePathPtr);

            var freePathPtr = MemoryManager.GetProcAddress(navProcPtr, "FreePathArr");
            freePathArr = MemoryManager.GetRegisterDelegate<FreePathArr>(freePathPtr);
        }

        public static Position[] CalculatePath(uint mapId, Position start, Position end, bool straightPath)
        {
            var ret = calculatePath(mapId, start.ToXYZ(), end.ToXYZ(), straightPath, out int length);
            Position[] list = new Position[length];
            for (var i = 0; i < length; i++)
            {
                list[i] = new Position(ret[i]);
            }
            freePathArr(ret);
            return list;
        }

        static public Position GetNextWaypoint(uint mapId, Position start, Position end, bool straightPath)
        {
            var path = CalculatePath(mapId, start, end, straightPath);
            if (path.Length <= 1)
            {
                Console.WriteLine("Problem building path. Returning destination as next waypoint...");
                return end;
            }

            return path[1];
        }

        static public float DistanceViaPath(uint mapId, Position start, Position end)
        {
            var distance = 0f;
            var path = CalculatePath(mapId, start, end, false);
            for (var i = 0; i < path.Length - 1; i++)
                distance += path[i].DistanceTo(path[i + 1]);
            return distance;
        }
    }
}