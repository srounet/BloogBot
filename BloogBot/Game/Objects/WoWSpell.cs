using BloogBot.Game.Enums;
using BloogBot.Game.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BloogBot.Game.Objects
{
    public class WoWSpell
    {
        #region addresses

        private static uint _EnoughPowerToCastSpellPtr = 0x008017E0;

        #endregion addresses

        #region delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool EnoughPowerToCastSpellDelegate(IntPtr instance, IntPtr rowPtr);

        private EnoughPowerToCastSpellDelegate EnoughPowerToCastSpellFunction = MemoryManager.GetRegisterDelegate<EnoughPowerToCastSpellDelegate>((IntPtr)_EnoughPowerToCastSpellPtr);

        #endregion delegates

        public WoWSpell(uint id)
        {
            Id = id;
            try
            {
                SpellRecord = WoWDB.Tables[ClientDB.Spell].GetLocalizedRow((int)id).GetStruct<SpellRec>();
            }
            catch
            {
                SpellRecord = default(SpellRec);
                Id = 0;
            }
        }

        private SpellRec SpellRecord { get; set; }
        public uint Id { get; private set; }

        public bool IsLearnt()
        {
            foreach (WoWSpell knownSpell in WoWDB.KnownSpells)
            {
                if (Id == knownSpell.Id)
                    return true;
            }
            return false;
        }

        public string Name
        {
            get { return SpellRecord.SpellName; }
        }

        public float Cooldown
        {
            get
            {
                return Functions.GetSpellCoolDown(Id);
            }
        }

        public bool IsReady
        {
            get { return Cooldown <= 0f; }
        }

        public bool CanUse
        {
            get { return IsLearnt() && IsReady; }
        }

        public void Cast()
        {
            Functions.DoString(String.Format("CastSpellByID({0})", Id));
        }

        public bool EnoughPowerToCastSpell()
        {
            IntPtr rowPtr = Marshal.AllocHGlobal(4 * 4 * 256);
            Marshal.StructureToPtr(SpellRecord, rowPtr, false);
            bool canCast = EnoughPowerToCastSpellFunction(ObjectManager.Me.Pointer, rowPtr);
            Marshal.FreeHGlobal(rowPtr);
            return canCast;
        }
    }
}