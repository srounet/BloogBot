using BloogBot.Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloogBot.AI
{
    internal class MoveToTargetState : IBotState
    {
        private readonly Stack<IBotState> botStates;
        private readonly WoWPlayerMe player;
        private readonly WoWUnit target;

        internal MoveToTargetState(Stack<IBotState> botStates, WoWUnit target)
        {
            Console.WriteLine("Entering MoveToTargetState");
            this.botStates = botStates;
            player = ObjectManager.Me;
            this.target = target;
        }

        public void Update()
        {
            var distanceToTarget = player.Position.DistanceTo(target.Position);

            if (distanceToTarget < 5)
            {
                player.MoveToStop();
                botStates.Pop();
                botStates.Push(new CombatState(botStates, target));
                return;
            }

            var nextWaypoint = Navigation.GetNextWaypoint((uint)ObjectManager.MapID, player.Position, target.Position, false);
            player.MoveTo(nextWaypoint);
        }
    }
}