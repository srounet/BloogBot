using BloogBot.Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloogBot.AI
{
    public class GrindState : IBotState
    {
        private static readonly Random random = new Random();

        private readonly Stack<IBotState> botStates;
        private readonly WoWPlayerMe player;

        public GrindState(Stack<IBotState> botStates)
        {
            Console.WriteLine("Entering GrindState");
            this.botStates = botStates;
            player = ObjectManager.Me;
        }

        public void Update()
        {
            // Player is dead, retrieve corpse
            if (player.IsDead || player.IsGhost)
            {
                botStates.Push(new RetrieveCorpseState(botStates));
            }
            else
            {
                WoWUnit enemyTarget = ObjectManager.FindClosestTarget();

                if (enemyTarget != null)
                {
                    enemyTarget.Target();
                    botStates.Push(new MoveToTargetState(botStates, enemyTarget));
                }
            }
        }
    }
}