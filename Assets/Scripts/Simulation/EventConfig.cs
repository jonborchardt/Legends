using UnityEngine;

namespace Legends.Simulation
{
    [CreateAssetMenu(fileName = "EventConfig", menuName = "Legends/EventConfig")]
    public class EventConfig : ScriptableObject
    {
        public string EventId            = "dragon_egg_relay";
        public int    NumSegments        = 10;
        public float  SegmentLength      = 5f;
        public float  BaseSegmentTime    = 1.5f;
        public float  StumbleTimePenalty = 1.2f;
        public float  EggDropTimePenalty = 2.5f;
        public float  SurgeTimeBonus     = 0.6f;

        public EventConfigData ToData() => new EventConfigData
        {
            EventId            = EventId,
            NumSegments        = NumSegments,
            SegmentLength      = SegmentLength,
            BaseSegmentTime    = BaseSegmentTime,
            StumbleTimePenalty = StumbleTimePenalty,
            EggDropTimePenalty = EggDropTimePenalty,
            SurgeTimeBonus     = SurgeTimeBonus,
        };
    }
}
