using BloogBot.Game.Enums;
using BloogBot.Game.Objects;
using BloogBot.Game.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloogBot.Game
{
    public class QuestCollection
    {
        public QuestCollection()
        {
            /*
            Manager.Events.Register("QUEST_QUERY_COMPLETE", (string ev, List<string> args) => { LastCompletedQuestsQuery = DateTime.Now; });
            */
        }

        private DateTime LastCompletedQuestsQuery { get; set; }

        public IEnumerable<QuestLogEntry> QuestLog
        {
            get
            {
                var descriptorArray = MemoryManager.InternalRead<uint>(new IntPtr((uint)ObjectManager.Me.Pointer + 0x8));
                for (int i = 0; i < 25; i++)
                {
                    var qlPtr = new IntPtr(descriptorArray + (int)ePlayerFields.PLAYER_QUEST_LOG_1_1 * 0x4 + (i * 0x14));
                    if (MemoryManager.InternalRead<uint>(qlPtr) > 0)
                        yield return MemoryManager.InternalRead<QuestLogEntry>(qlPtr);
                }
            }
        }

        // Remember to run QueryQuestsCompleted() before using this.
        public IEnumerable<int> CompletedQuestIds
        {
            get
            {
                // We probably haven't asked for an update in a while...
                if (DateTime.Now.Subtract(LastCompletedQuestsQuery).TotalMinutes > 2)
                {
                    QueryQuestsCompleted();
                    yield break;
                }

                uint CompletedQuests = 0x00ACFDF4;
                var currentQuest = MemoryManager.InternalRead<uint>((IntPtr)CompletedQuests);
                while ((currentQuest & 1) == 0 && currentQuest > 0)
                {
                    yield return MemoryManager.ReadInt(new IntPtr(currentQuest + 2 * 4));
                    currentQuest = MemoryManager.InternalRead<uint>(new IntPtr(currentQuest + 4));
                }
            }
        }

        public WoWQuest this[int id]
        {
            get { return new WoWQuest(id); }
        }

        public void QueryQuestsCompleted()
        {
            Functions.DoString("QueryQuestsCompleted()");
            // Manager.ExecutionQueue.AddExececution(() => { WoWScript.ExecuteNoResults("QueryQuestsCompleted()"); });
        }
    }
}