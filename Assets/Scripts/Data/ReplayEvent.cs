using System;
using System.Collections.Generic;

namespace Legends.Data
{
    [Serializable]
    public class ReplayEvent
    {
        public float Timestamp;
        public string AthleteId;
        public string EventType;
        // Parallel lists instead of Dictionary<> for JsonUtility compatibility.
        public List<string> ParamKeys;
        public List<float> ParamValues;
    }
}
