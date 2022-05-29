using BloogBot.Game.Objects;
using System;
using System.Collections.Generic;

namespace BloogBot.AI
{
    internal class RestState : IBotState
    {
        private static readonly Random random = new Random();

        private readonly Stack<IBotState> botStates;
        private readonly WoWPlayerMe player;

        public RestState(Stack<IBotState> botStates)
        {
            Console.WriteLine("Entering RestState");
            this.botStates = botStates;
            player = ObjectManager.Me;
            player.MoveToStop();
        }

        public void Update()
        {
            if (player.HealthPercent > 60)
            {
                botStates.Pop();
            }
        }
    }
}