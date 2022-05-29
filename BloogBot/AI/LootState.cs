using BloogBot.Game.Objects;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BloogBot.AI
{
    public class LootState : IBotState
    {
        private static readonly Random random = new Random();

        private readonly Stack<IBotState> botStates;
        private readonly WoWPlayerMe player;
        private readonly WoWUnit target;

        public LootState(Stack<IBotState> botStates, WoWUnit target)
        {
            Console.WriteLine("Entering LootState");
            this.botStates = botStates;
            player = ObjectManager.Me;
            this.target = target;
        }

        public void Update()
        {
            player.MoveToStop();
            if (target.Health == 0)
            {
                this.target.Interact();
                Thread.Sleep(100);
                botStates.Pop();
                botStates.Push(new RestState(botStates));
                //botStates.Pop();
            }
        }
    }
}