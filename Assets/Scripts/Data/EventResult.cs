using System;
using System.Collections.Generic;

namespace Legends.Data
{
    [Serializable]
    public class EventResult
    {
        public string EventId;
        public int Seed;
        public List<string> Placements;      // AthleteIds in finish order
        public List<ReplayEvent> Timeline;
        public List<ReplayFrame> Frames;
        public List<string> StatKeys;        // parallel arrays for per-athlete stats
        public List<string> StatValues;
    }
}
