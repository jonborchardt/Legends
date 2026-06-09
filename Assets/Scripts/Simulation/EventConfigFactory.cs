namespace Legends.Simulation
{
    // Creates EventConfig instances entirely in code — no .asset file required.
    public static class EventConfigFactory
    {
        public static EventConfig CreateDragonEggRelay()
        {
            var config = UnityEngine.ScriptableObject.CreateInstance<EventConfig>();
            config.EventId            = "dragon_egg_relay";
            config.NumSegments        = 10;
            config.SegmentLength      = 5f;
            config.BaseSegmentTime    = 1.5f;
            config.StumbleTimePenalty = 1.2f;
            config.EggDropTimePenalty = 2.5f;
            config.SurgeTimeBonus     = 0.6f;
            return config;
        }
    }
}
