using System;
using System.Collections.Generic;
using System.Linq;
using Legends.Data;

namespace Legends.Simulation
{
    public class DragonEggRelaySimulator : IEventSimulator
    {
        private class AthleteRunState
        {
            public string Id;
            public int Dexterity, Constitution, Focus, Strength, Luck;
            public int Progress;
            public float Time;
            public bool WobbleActive;
            public bool Finished;
        }

        public EventResult Simulate(EventConfigData config, List<AthleteState> athletes, int seed)
        {
            var rng = new Random(seed);
            var sorted = athletes.OrderBy(a => a.Id).ToList();

            var states = sorted.Select(a => new AthleteRunState
            {
                Id           = a.Id,
                Dexterity    = a.Stats.Dexterity,
                Constitution = a.Stats.Constitution,
                Focus        = a.Stats.Focus,
                Strength     = a.Stats.Strength,
                Luck         = a.Stats.Luck,
                Progress     = 0,
                Time         = 0f,
                WobbleActive = false,
                Finished     = false,
            }).ToList();

            var result = new EventResult
            {
                EventId    = "dragon_egg_relay",
                Seed       = seed,
                Placements = new List<string>(),
                Timeline   = new List<ReplayEvent>(),
                Frames     = new List<ReplayFrame>(),
                StatKeys   = new List<string>(),
                StatValues = new List<string>(),
            };

            RunMainLoop(states, rng, config, result);
            GenerateFrames(states, config, result);

            return result;
        }

        private void RunMainLoop(List<AthleteRunState> states, Random rng,
                                  EventConfigData config, EventResult result)
        {
            while (states.Any(s => !s.Finished))
            {
                foreach (var athlete in states)
                {
                    if (athlete.Finished) continue;
                    RunSegment(athlete, rng, config, result);
                }
            }

            result.Placements = result.Placements
                .OrderBy(id => states.First(s => s.Id == id).Time)
                .ToList();
        }

        private void RunSegment(AthleteRunState a, Random rng,
                                 EventConfigData config, EventResult result)
        {
            float t = a.Time;
            bool didStumble = false;

            // 1. Stumble check
            double stumbleChance = 0.05 + (10 - a.Constitution) * 0.02;
            if (rng.NextDouble() < stumbleChance)
            {
                a.Time += config.StumbleTimePenalty;
                AddEvent(result, t, a.Id, EventTypes.Stumble);
                didStumble = true;
            }

            if (!didStumble)
            {
                // 2. Egg wobble check (only if not already wobbling)
                if (!a.WobbleActive)
                {
                    double wobbleChance = 0.04 + (10 - a.Dexterity) * 0.015;
                    if (rng.NextDouble() < wobbleChance)
                    {
                        a.WobbleActive = true;
                        AddEvent(result, t, a.Id, EventTypes.EggWobble);
                    }
                }

                // 3. Surge check
                double surgeChance = 0.08 + a.Focus * 0.02;
                if (rng.NextDouble() < surgeChance)
                {
                    a.Time = Math.Max(a.Time - config.SurgeTimeBonus, 0f);
                    AddEvent(result, t, a.Id, EventTypes.Surge);
                }
            }

            // 4. Progress (always happens)
            float segTime = config.BaseSegmentTime - (a.Strength - 5) * 0.1f;
            segTime = Math.Max(segTime, 0.3f);
            a.Time += segTime;
            a.Progress++;
            AddEventWithParam(result, t, a.Id, EventTypes.Progress, "segment", a.Progress);

            // 5. Egg drop check (only if wobble active)
            if (a.WobbleActive)
            {
                double dropChance = 0.15 + (10 - a.Luck) * 0.02;
                if (rng.NextDouble() < dropChance)
                {
                    a.WobbleActive = false;
                    a.Time += config.EggDropTimePenalty;
                    AddEvent(result, t, a.Id, EventTypes.EggDrop);
                }
                else
                {
                    a.WobbleActive = false;
                    AddEvent(result, t, a.Id, EventTypes.EggRecovery);
                }
            }

            // 6. Finish check
            if (a.Progress >= config.NumSegments)
            {
                a.Finished = true;
                result.Placements.Add(a.Id);
                AddEvent(result, a.Time, a.Id, EventTypes.Finish);
            }
        }

        private void GenerateFrames(List<AthleteRunState> states, EventConfigData config, EventResult result)
        {
            float maxTime = states.Max(s => s.Time);
            float interval = 0.1f;

            var timelineByAthlete = result.Timeline
                .GroupBy(e => e.AthleteId)
                .ToDictionary(g => g.Key, g => g.OrderBy(e => e.Timestamp).ToList());

            for (float t = 0f; t <= maxTime + interval; t += interval)
            {
                int slotIndex = 0;
                foreach (var athlete in states)
                {
                    float progress = GetProgressAt(athlete.Id, t, timelineByAthlete, config.NumSegments);
                    float xPos    = progress * config.SegmentLength;
                    float zOffset = slotIndex * 1.2f;

                    result.Frames.Add(new ReplayFrame
                    {
                        Timestamp = t,
                        AthleteId = athlete.Id,
                        Position  = new float[] { xPos, 0f, zOffset },
                    });

                    slotIndex++;
                }
            }
        }

        private float GetProgressAt(string athleteId, float t,
            Dictionary<string, List<ReplayEvent>> timeline, int numSegments)
        {
            if (!timeline.TryGetValue(athleteId, out var events))
                return 0f;

            int segmentsCompleted = events
                .Where(e => e.EventType == EventTypes.Progress && e.Timestamp <= t)
                .Count();

            return Math.Min(segmentsCompleted, numSegments);
        }

        private void AddEvent(EventResult result, float timestamp, string athleteId, string eventType)
        {
            result.Timeline.Add(new ReplayEvent
            {
                Timestamp   = timestamp,
                AthleteId   = athleteId,
                EventType   = eventType,
                ParamKeys   = new List<string>(),
                ParamValues = new List<float>(),
            });
        }

        private void AddEventWithParam(EventResult result, float timestamp, string athleteId,
                                        string eventType, string key, float value)
        {
            result.Timeline.Add(new ReplayEvent
            {
                Timestamp   = timestamp,
                AthleteId   = athleteId,
                EventType   = eventType,
                ParamKeys   = new List<string> { key },
                ParamValues = new List<float> { value },
            });
        }
    }
}
