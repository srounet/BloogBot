using BloogBot.Game;
using BloogBot.Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BloogBot.AI
{
    internal class RetrieveCorpseState : IBotState
    {
        private static readonly Random random = new Random();

        private readonly Stack<IBotState> botStates;
        private readonly WoWPlayerMe player;

        public RetrieveCorpseState(Stack<IBotState> botStates)
        {
            Console.WriteLine("Entering RetrieveCorpseState");
            this.botStates = botStates;
            player = ObjectManager.Me;
            player.MoveToStop();
        }

        public void Update()
        {
            // Player Dead need to repop
            if (player.IsDead && !player.IsGhost)
            {
                // Click Repop
                Console.WriteLine("Repop");
                Functions.RepopMe();
                Thread.Sleep(1000);
            }

            if (!player.IsDead && !player.IsGhost)
            {
                botStates.Pop();
            }

            var distanceToCorpse = player.Position.DistanceTo(player.Corpse);

            if (distanceToCorpse < 30)
            {
                player.MoveToStop();
                // Click Retrieve Corpse
                Functions.RetrieveCorpse();
                Thread.Sleep(1000);
                botStates.Pop();
                // Rest
                botStates.Push(new RestState(botStates)); // untested
                return;
            }

            var nextWaypoint = Navigation.GetNextWaypoint((uint)ObjectManager.MapID, player.Position, player.Corpse, false);
            player.MoveTo(nextWaypoint);
        }
    }
}