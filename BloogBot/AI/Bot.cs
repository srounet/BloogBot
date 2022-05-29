using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloogBot.AI
{
    internal class Bot
    {
        public readonly Stack<IBotState> botStates = new Stack<IBotState>();

        private bool running;

        internal void PushState(IBotState state) => botStates.Push(state);

        internal void PopState()
        {
            if (botStates.Count > 0)
                botStates.Pop();
        }

        internal void Stop()
        {
            Console.WriteLine("\n--- STOPPING BOT ---\n");
            running = false;
            while (botStates.Count > 0)
                botStates.Pop();
        }

        internal void Start()
        {
            Console.WriteLine("\n--- STARTING BOT ---\n");
            running = true;
            StartInternal();
        }

        private async void StartInternal()
        {
            while (running)
            {
                try
                {
                    if (botStates.Count == 0)
                        Console.WriteLine("Bot currently has no state.");
                    else
                    {
                        botStates.Peek()?.Update();
                    }

                    await Task.Delay(25);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error occured inside Bot's main loop: {e}");
                }
            }
        }
    }
}