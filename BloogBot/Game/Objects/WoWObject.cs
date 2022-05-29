using BloogBot.Game.Enums;
using BloogBot.Game.Objects;
using BloogBot.Game.Structs;
using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace BloogBot.Game
{
    // https://www.ownedcore.com/forums/world-of-warcraft/world-of-warcraft-bots-programs/wow-memory-editing/268259-problems-using-whitemagic-trying-delegate-function.html
    public class WoWObject
    {
        private int WoWObject_DescriptorOffset = 0x8;

        private const float halfRangedShootCone = 1.48f;

        private const float halfMeleeShootCone = 0.95f;        //1.045f;

        public readonly IntPtr Pointer;
        public readonly ulong Guid;
        public readonly ObjectType ObjectType;

        #region delegates

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void GetPositionDelegate(IntPtr objectPtr, ref XYZ pos);

        private GetPositionDelegate getPositionFunction;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Ansi)]
        private delegate IntPtr GetNameDelegate(IntPtr objectPtr);

        private GetNameDelegate GetNameFunction;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void InteractDelegate(IntPtr objectPtr);

        private InteractDelegate InteractFunction;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate float GetFacingDelegate(IntPtr objectPtr);

        private GetFacingDelegate GetFacingFunction;

        #endregion delegates

        public enum VTable
        {
            GetPosition = 12,
            GetFacing = 14,
            Interact = 44,
            GetName = 54,

            // CGUnit_C_VTABLE 0x00A34D38 (R:0x00634D38 D:0x00A34D38)
            // CGUnit_C__GetPosition 0x006E71D0 (R:0x002E71D0 D:0x006E71D0) VTable index: 12
            // CGUnit_C__GetFacing 0x006E7220 (R:0x002E7220 D:0x006E7220) VTable index: 14
            // CGUnit_C__GetModel 0x00717F00 (R:0x00317F00 D:0x00717F00) VTable index: 24
            // CGUnit_C__OnRightClick 0x00731710 (R:0x00331710 D:0x00731710) VTable index: 44
            // CGUnit_C__GetObjectName 0x006E71A0 (R:0x002E71A0 D:0x006E71A0) VTable index: 54
        }

        internal WoWObject(IntPtr pointer, ulong guid, ObjectType objectType)
        {
            Pointer = pointer;
            Guid = guid;
            ObjectType = objectType;
        }

        protected IntPtr GetDescriptorPtr() => MemoryManager.ReadIntPtr(IntPtr.Add(Pointer, WoWObject_DescriptorOffset));

        public bool IsUnit => ObjectType == ObjectType.Unit;

        public bool IsPlayer => ObjectType == ObjectType.Player;

        public bool IsItem => ObjectType == ObjectType.Item;

        #region GetFacing

        public virtual float Facing => GetFacing();

        public float GetFacing()
        {
            IntPtr ptr = Marshal.ReadIntPtr(Marshal.ReadIntPtr(Pointer), (int)VTable.GetFacing * 4);
            GetFacingFunction = MemoryManager.GetRegisterDelegate<GetFacingDelegate>(ptr);
            return ThreadSynchronizer.RunOnMainThread(() => GetFacingFunction(Pointer));
        }

        #endregion GetFacing

        #region Interact

        public void Interact()
        {
            Console.WriteLine("Interact");
            IntPtr ptr = Marshal.ReadIntPtr(Marshal.ReadIntPtr(Pointer), (int)VTable.Interact * 4);
            InteractFunction = MemoryManager.GetRegisterDelegate<InteractDelegate>(ptr);
            ThreadSynchronizer.RunOnMainThread(() => InteractFunction(Pointer));
        }

        #endregion Interact

        #region GetPosition

        public Position Position => GetPosition();

        [HandleProcessCorruptedStateExceptions]
        private Position GetPosition()
        {
            try
            {
                IntPtr ptr = Marshal.ReadIntPtr(Marshal.ReadIntPtr(Pointer), (int)VTable.GetPosition * 4);
                getPositionFunction = MemoryManager.GetRegisterDelegate<GetPositionDelegate>(ptr);
                var xyz = new XYZ();
                getPositionFunction(Pointer, ref xyz);

                return new Position(xyz);
            }
            catch (AccessViolationException)
            {
                Console.WriteLine("Access violation on WoWObject.Position. Swallowing.");
                return new Position(0, 0, 0);
            }
        }

        #endregion GetPosition

        #region GetName

        public string Name => GetName();

        [HandleProcessCorruptedStateExceptions]
        public string GetName()
        {
            try
            {
                IntPtr ptr = Marshal.ReadIntPtr(Marshal.ReadIntPtr(Pointer), (int)VTable.GetName * 4);
                GetNameFunction = MemoryManager.GetRegisterDelegate<GetNameDelegate>(ptr);
                byte[] bytes = Encoding.Default.GetBytes(Marshal.PtrToStringAnsi(GetNameFunction(Pointer)));
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                return "";
            }
        }

        #endregion GetName

        #region facing

        private float NormalizeRadian(float radian)
        {
            while (radian < 0)
            {
                radian = (float)(radian + 2 * Math.PI);
            }
            while (radian >= 2 * Math.PI)
            {
                radian = (float)(radian - 2 * Math.PI);
            }
            return radian;
        }

        private float CalculateNeededFacing(Position start, Position end)
        {
            return (float)Math.Atan2((end.Y - start.Y), (end.X - start.X));
        }

        private float GetSideFaceRotation(float halfShootCone)
        {
            float angle = NormalizeRadian(CalculateNeededFacing(ObjectManager.Me.Position, Position));
            float faceTo0 = NormalizeRadian(angle - ObjectManager.Me.GetFacing());
            float faceDiff = faceTo0;
            bool objectOnRightSide = false;
            float sideFaceDiff = 0;

            if (faceTo0 >= Math.PI)
            {
                faceDiff = (float)(2 * Math.PI - faceTo0);
                objectOnRightSide = true;
            }

            if (faceDiff > halfShootCone)
            {
                sideFaceDiff = faceDiff - halfShootCone;

                if (!objectOnRightSide)
                {
                    return sideFaceDiff + ObjectManager.Me.GetFacing();
                }
                else
                {
                    return ObjectManager.Me.GetFacing() - sideFaceDiff;
                }
            }
            else
            {
                return ObjectManager.Me.GetFacing();
            }
        }

        private float GetSideFaceAngle()
        {
            float angle = CalculateNeededFacing(ObjectManager.Me.Position, Position);
            float faceTo0 = NormalizeRadian(angle - ObjectManager.Me.GetFacing());

            if (faceTo0 > Math.PI)
            {
                faceTo0 = (float)-(2 * Math.PI - faceTo0);
            }
            return faceTo0;
        }

        private bool IsFacing(float angle)
        {
            return ObjectManager.Me.GetFacing() == GetSideFaceRotation(angle);
        }

        public bool IsFacingMelee => IsFacing(halfMeleeShootCone);

        public bool IsFacingRanged => IsFacing(halfRangedShootCone);

        private void SetFacing(float angle)
        {
            if (angle < 0.0f)
                angle += (float)(Math.PI * 2);
            if (angle > Math.PI * 2)
                angle -= (float)(Math.PI * 2);

            Functions.SetFacing(ObjectManager.Me.Pointer, Functions.PerformanceCounter(), angle);
        }

        private bool Face(float angle)
        {
            if (!IsFacing(angle))
            {
                float sfr = GetSideFaceRotation(angle);
                sfr = sfr < Math.PI ? sfr - angle / 100 : sfr + angle / 100;

                SetFacing(sfr);
                return true;
            }
            return false;
        }

        public bool FaceMelee()
        { return Face(halfMeleeShootCone); }

        public bool FaceRanged()
        { return Face(halfRangedShootCone); }

        #endregion facing
    }
}