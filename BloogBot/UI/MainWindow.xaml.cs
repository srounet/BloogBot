using BloogBot.AI;
using BloogBot.Game;
using BloogBot.Game.Enums;
using BloogBot.Game.Objects;
using BloogBot.Game.Structs;
using System;
using System.Linq;
using System.Threading;
using System.Windows;

namespace BloogBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {
        private readonly Bot bot = new Bot();
        private WoWUnit currentUnit = null;

        public MainWindow()
        {
            InitializeComponent();
            Thread.Sleep(250);
            ObjectManager.Initialize();
            Thread.Sleep(250);
            ObjectManager.StartEnumeration();
            Thread.Sleep(250);
            WoWDB.Initialize();

            Functions.PatchLuaProtect();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            foreach (WoWUnit unit in ObjectManager.Units)
            {
                Console.WriteLine(String.Format("{0}: {1}", unit.Name, unit.FactionId));
            }
            WoWUnit target = ObjectManager.FindClosestTarget();

            if (target != null)
            {
                target.Target();
                currentUnit = target;
                // ObjectManager.Me.IsFacing(target.Position);
                // Console.WriteLine("ObjectManager.Me.IsFacing(target.Position): {0}", ObjectManager.Me.IsFacing(target.Position));
            }

            if (ObjectManager.IsLoggedIn)
            {
                ObjectManager.Quests.QueryQuestsCompleted();
                foreach (QuestLogEntry q in ObjectManager.Quests.QuestLog)
                {
                    q.AsWoWQuest();
                    //Console.WriteLine(q.AsWoWQuest().CachedEntry.Name);
                }
            }

            // ObjectManager.Me.LookAt(target.Position);

            // Console.WriteLine(String.Format("Health: {0}", ObjectManager.Units.First().Health));
            // ThreadSynchronizer.RunOnMainThread(() => ObjectManager.Pulse());

            // var objectCount = ObjectManager.Objects.Count;
        }

        private void MoveTo_Click(object sender, RoutedEventArgs e)
        {
            bot.botStates.Clear();
            Console.WriteLine("ObjectManager.Aggressors.Count(): {0}", ObjectManager.Aggressors.Count());

            if (currentUnit != null)
            {
                Console.WriteLine("currentUnit.IsUnit(): {0}", currentUnit.IsUnit);
                Console.WriteLine("ObjectManager.Me.CanAttack(currentUnit): {0}", ObjectManager.Me.CanAttack());
                /*
                Console.WriteLine("ObjectManager.Me.Facing: {0}", ObjectManager.Me.Facing);
                Console.WriteLine("currentUnit.IsFacingMelee(): {0}", currentUnit.IsFacingMelee());
                Console.WriteLine("currentUnit.FaceMelee(): {0}", currentUnit.FaceMelee());
                Console.WriteLine("currentUnit.IsMoving(): {0}", currentUnit.IsMoving());
                Console.WriteLine("ObjectManager.Me.IsMoving(): {0}", ObjectManager.Me.IsMoving());
                Console.WriteLine("ObjectManager.Me.Rage: {0}", ObjectManager.Me.Rage);
                */
                // ObjectManager.Me.Face(currentUnit.Position);
                // ObjectManager.Me.LookAt(currentUnit);
                // Console.WriteLine("Is in combat?: {0}", currentUnit.IsInCombat);
                // Console.WriteLine(ObjectManager.Me.IsAutoAttacking);
            }
            Console.WriteLine("ME X: {0}", ObjectManager.Me.Position.X);
            Console.WriteLine("ME Y: {0}", ObjectManager.Me.Position.Y);
            Console.WriteLine("ME Z: {0}", ObjectManager.Me.Position.Z);

            Console.WriteLine("ME Corpse X: {0}", ObjectManager.Me.Corpse.X);
            Console.WriteLine("ME Corpse Y: {0}", ObjectManager.Me.Corpse.Y);
            Console.WriteLine("ME Corpse Z: {0}", ObjectManager.Me.Corpse.Z);

            Console.WriteLine("ME IsGhost: {0}", ObjectManager.Me.IsGhost);

            //
            Functions.RepopMe();
            //Functions.RetrieveCorpse();
            /*
            WoWSpell HeroicStrike = new WoWSpell(78);
            Console.WriteLine("HeroicStrike.EnoughPowerToCastSpell(): {0}", HeroicStrike.EnoughPowerToCastSpell());
            */

            // https://github.com/miceiken/IceFlake/blob/f8faa4417b6882da7ccfe7f491d1f7693199c5d5/IceFlake/Client/Objects/WoWSpell.cs

            /*
            DBTableManager.InitDBTables();
            Console.WriteLine(DBTableManager.db_tables[Game.Enums.ClientDB.Spell]);
            SpellRec spell = new SpellRec();
            DBTableManager.db_tables[Game.Enums.ClientDB.Spell].GetLocalizedRow(6603, spell);
            */

            /*
            Position targetPosition = ObjectManager.CurrentTarget.Position;
            Console.WriteLine(String.Format("Target GUID: {0}", ObjectManager.Me.TargetGuid));
            Console.WriteLine(String.Format(
                "Target: {0} ({1}) {2}",
                ObjectManager.CurrentTarget.Name, ObjectManager.CurrentTarget.Level,
                targetPosition.X
            ));
            */
            // ObjectManager.Me.MoveTo(targetPosition);
            //bot.PushState(new GrindState(bot.botStates));
            // WoWUnit target = ObjectManager.Units.First();
            // var nextWaypoint = Navigation.GetNextWaypoint((uint)ObjectManager.MapID, ObjectManager.Me.Position, target.Position, false);
            // Console.WriteLine(nextWaypoint.X);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            bot.Start();
            WoWDB.refreshKnownSpells();
            bot.PushState(new GrindState(bot.botStates));
        }
    }
}