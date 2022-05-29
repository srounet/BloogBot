using BloogBot.Game;
using BloogBot.Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BloogBot.AI
{
    internal class CombatState : IBotState
    {
        private readonly Stack<IBotState> botStates;
        private readonly WoWPlayerMe player;
        private readonly WoWUnit target;

        #region spells

        private readonly WoWSpell AutoAttack = new WoWSpell(6603);
        private readonly WoWSpell BloodFury = new WoWSpell(20572);
        private readonly WoWSpell Charge = new WoWSpell(100);
        private readonly WoWSpell HeroicStrike = new WoWSpell(78);
        private readonly WoWSpell BattleShout = new WoWSpell(6673);

        #endregion spells

        internal CombatState(Stack<IBotState> botStates, WoWUnit target)
        {
            Console.WriteLine("Entering CombatState");
            this.botStates = botStates;
            player = ObjectManager.Me;
            this.target = target;

            /*
            player.LookAt(target);
            player.MoveToStop();
            */

            AutoAttack.Cast();
            TryUseAbility(BloodFury);
            TryUseAbility(BattleShout);
        }

        public void Update()
        {
            TryUseAbility(HeroicStrike);

            if (target.IsDead)
            {
                botStates.Pop();
                if (ObjectManager.Aggressors.Count() > 0)
                {
                    Console.WriteLine("Got other agressors, queing combat");
                    foreach (WoWUnit aggressor in ObjectManager.Aggressors)
                    {
                        botStates.Push(new CombatState(botStates, aggressor));
                    }
                }
                // Check if any other unit is attacking us
                // queue Combat State to this unit

                botStates.Push(new LootState(botStates, target));
            }

            var distanceToTarget = player.Position.DistanceTo(target.Position);

            if (distanceToTarget > 5)
            {
                player.MoveTo(target.Position);
            }

            if (!player.IsMoving && !target.IsFacingMelee)
                target.FaceMelee();

            if (ObjectManager.Me.TargetGuid != target.Guid)
            {
                target.Target();
            }

            if (!player.IsAutoAttacking)
            {
                target.Target();
                AutoAttack.Cast();
                // Functions.DoString("CastSpellByID(6603)");
                Thread.Sleep(25);
            }

            // Player is dead, retrieve corpse
            if (player.IsDead || player.IsGhost)
            {
                botStates.Pop();
                botStates.Push(new RetrieveCorpseState(botStates));
            }
        }

        public void TryUseAbility(WoWSpell spell)
        {
            if (spell.CanUse && !player.IsCasting && spell.EnoughPowerToCastSpell())
            {
                Console.WriteLine("Casting: {0}", spell.Name);
                spell.Cast();
            }
        }
    }
}