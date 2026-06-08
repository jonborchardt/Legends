using System;
using System.Collections.Generic;

namespace Legends.Data
{
    [Serializable]
    public class GameState
    {
        public string SaveVersion = "1.0";
        public TeamState Team;
        public List<EventResult> CompletedEvents = new();
    }
}
