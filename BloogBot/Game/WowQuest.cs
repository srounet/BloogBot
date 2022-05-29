using BloogBot.Game.Objects;
using BloogBot.Game.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloogBot.Game
{
    public class WoWQuest
    {
        public WoWQuest(int id)
        {
            ID = id;
            if (CachedEntry.Id == 0)
                CachedEntry = Functions.GetQuestRecordFromId(id);
        }

        public WoWQuest(QuestLogEntry ql)
            : this(ql.ID)
        {
            QuestLogEntry = ql;
        }

        public WoWQuest(QuestCacheRecord qc)
            : this(qc.Id)
        {
            CachedEntry = qc;
        }

        public int ID { get; private set; }

        public QuestLogEntry QuestLogEntry { get; private set; }

        public QuestCacheRecord CachedEntry { get; private set; }

        public bool PlayerIsOnQuest
        {
            get { return ObjectManager.Quests.QuestLog.Count(x => x.ID == ID) > 0; }
        }

        public bool PlayerHasCompletedQuest
        {
            get { return ObjectManager.Quests.CompletedQuestIds.Count(x => x == ID) > 0; }
        }

        public bool PlayerIsSuitableForQuest
        {
            get { return ObjectManager.Me.Level >= CachedEntry.RequiredLevel; }
        }
    }
}