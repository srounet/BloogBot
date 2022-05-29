using BloogBot.Game.Enums;
using BloogBot.Game.Objects;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BloogBot.Game
{
    // https://github.com/miceiken/IceFlake/blob/f8faa4417b6882da7ccfe7f491d1f7693199c5d5/IceFlake/Client/WoWDB.cs
    public static class WoWDB
    {
        public static readonly Dictionary<ClientDB, DbTable> Tables = new Dictionary<ClientDB, DbTable>();
        public static List<WoWSpell> KnownSpells = new List<WoWSpell>();

        #region addresses

        internal static uint _RegisterBase = 0x006337D0;
        internal static uint _GetRow = 0x004BB1C0;
        internal static uint _GetLocalizedRow = 0x004CFD20;
        internal static uint _SpellCount = 0x00BE8D9C;
        internal static uint _SpellBook = 0x00BE5D88;

        #endregion addresses

        static internal void Initialize()
        {
            for (var tableBase = (IntPtr)_RegisterBase; MemoryManager.ReadByte(tableBase) != 0xC3; tableBase += 0x11)
            {
                var index = MemoryManager.ReadInt(tableBase + 1);
                var tablePtr = new IntPtr(MemoryManager.ReadInt(tableBase + 0xB) + 0x18);
                Tables.Add((ClientDB)index, new DbTable(tablePtr));
            }
        }

        public static void refreshKnownSpells()
        {
            var knownspells = new List<WoWSpell>();
            for (int i = 0; i < MemoryManager.ReadInt((IntPtr)_SpellCount); i++)
            {
                var spellId = MemoryManager.ReadInt((IntPtr)(_SpellBook + (i * 4)));
                knownspells.Add(new WoWSpell((uint)spellId));
            }
            KnownSpells.Clear();
            KnownSpells.AddRange(knownspells);
        }

        #region Nested type: DbTable

        public class DbTable
        {
            internal readonly IntPtr Address;

            public uint MaxIndex { get; private set; }
            public uint MinIndex { get; private set; }

            public DbTable(IntPtr address)
            {
                Address = address;
                var h = MemoryManager.InternalRead<DbHeader>(Address);
                MaxIndex = h.MaxIndex;
                MinIndex = h.MinIndex;
            }

            #region ClientDb_GetLocalizedRow

            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            private delegate int ClientDb_GetLocalizedRow(IntPtr instance, int index, IntPtr rowPtr);

            private ClientDb_GetLocalizedRow _getLocalizedRow = MemoryManager.GetRegisterDelegate<ClientDb_GetLocalizedRow>((IntPtr)_GetLocalizedRow);

            public Row GetLocalizedRow(int index)
            {
                IntPtr rowPtr = Marshal.AllocHGlobal(4 * 4 * 256);
                int tmp = _getLocalizedRow(new IntPtr(Address.ToInt32() - 0x18), index, rowPtr);
                if (tmp != 0)
                {
                    Console.WriteLine(rowPtr);
                    return new Row(rowPtr, true);
                }
                Marshal.FreeHGlobal(rowPtr);
                return null;
            }

            #endregion ClientDb_GetLocalizedRow

            #region ClientDb_GetRow

            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            private delegate int ClientDb_GetRow(IntPtr instance, int idx);

            private ClientDb_GetRow _getRow = MemoryManager.GetRegisterDelegate<ClientDb_GetRow>((IntPtr)_GetRow);

            public Row GetRow(int index) => GetRowFromDelegate(index);

            private Row GetRowFromDelegate(int index)
            {
                var ret = new IntPtr(_getRow(new IntPtr(Address.ToInt32()), index));
                return ret == IntPtr.Zero ? null : new Row(ret, false);
            }

            #endregion ClientDb_GetRow

            #region Nested type: DbHeader

            [StructLayout(LayoutKind.Sequential)]
            private struct DbHeader
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public readonly uint[] Junk;

                public readonly uint MaxIndex;
                public readonly uint MinIndex;
            }

            #endregion Nested type: DbHeader

            #region Nested type: Row

            public class Row : IDisposable
            {
                private readonly bool _isManagedMem;
                private IntPtr _rowPtr;
                private object structedObject;

                private Row(IntPtr rowPtr)
                {
                    _rowPtr = rowPtr;
                }

                internal Row(IntPtr rowPtr, bool isManagedMem) : this(rowPtr)
                {
                    _isManagedMem = isManagedMem;
                }

                #region IDisposable Members

                public void Dispose()
                {
                    if (_isManagedMem)
                    {
                        Marshal.FreeHGlobal(_rowPtr);
                    }

                    _rowPtr = IntPtr.Zero;
                    GC.SuppressFinalize(this);
                }

                #endregion IDisposable Members

                public T GetField<T>(uint index) where T : struct
                {
                    try
                    {
                        if (typeof(T) == typeof(string))
                        {
                            // Sometimes.... generics ****ing suck
                            object s =
                                Marshal.PtrToStringAnsi(
                                    MemoryManager.ReadIntPtr(new IntPtr((uint)_rowPtr + (index * 4))));
                            return (T)s;
                        }

                        return MemoryManager.InternalRead<T>(new IntPtr((uint)_rowPtr + (index * 4)));
                    }
                    catch
                    {
                        return default(T);
                    }
                }

                //public void SetField(uint index, int value)
                //{
                //    byte[] bs = BitConverter.GetBytes(value);
                //    Win32.WriteBytes((IntPtr)(_rowPtr.ToUInt32() + (index * 4)), bs);
                //}

                public T GetStruct<T>() where T : struct
                {
                    try
                    {
                        if (structedObject == null)
                        {
                            IntPtr addr = _rowPtr;
                            structedObject = (T)Marshal.PtrToStructure(addr, typeof(T));
                            Marshal.FreeHGlobal(addr);
                        }
                        return (T)structedObject;
                    }
                    catch
                    {
                        return default(T);
                    }
                }
            }

            #endregion Nested type: Row
        }

        #endregion Nested type: DbTable
    }
}