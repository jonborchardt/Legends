using NUnit.Framework;
using Newtonsoft.Json;
using Legends.Data;
using System.Collections.Generic;

[TestFixture]
public class ReplayDataTests
{
    [Test]
    public void ReplayFrame_Position_SurvivesRoundTrip()
    {
        var frame = new ReplayFrame
        {
            Timestamp = 1.5f,
            AthleteId = "a1",
            Position = new float[] { 1.5f, 2.0f, -3.0f },
        };

        string json = JsonConvert.SerializeObject(frame);
        var restored = JsonConvert.DeserializeObject<ReplayFrame>(json);

        Assert.AreEqual(1.5f, restored.Timestamp, 0.0001f);
        Assert.AreEqual("a1", restored.AthleteId);
        Assert.AreEqual(3, restored.Position.Length);
        Assert.AreEqual(1.5f, restored.Position[0], 0.0001f);
        Assert.AreEqual(2.0f, restored.Position[1], 0.0001f);
        Assert.AreEqual(-3.0f, restored.Position[2], 0.0001f);
    }

    [Test]
    public void ReplayEvent_SurvivesRoundTrip()
    {
        var evt = new ReplayEvent
        {
            Timestamp = 3.2f,
            AthleteId = "a2",
            EventType = EventTypes.EggDrop,
            ParamKeys = new List<string> { "segment" },
            ParamValues = new List<float> { 5f }
        };

        string json = JsonConvert.SerializeObject(evt);
        var restored = JsonConvert.DeserializeObject<ReplayEvent>(json);

        Assert.AreEqual(EventTypes.EggDrop, restored.EventType);
        Assert.AreEqual(3.2f, restored.Timestamp, 0.0001f);
        Assert.AreEqual(1, restored.ParamKeys.Count);
        Assert.AreEqual("segment", restored.ParamKeys[0]);
        Assert.AreEqual(5f, restored.ParamValues[0], 0.0001f);
    }

    [Test]
    public void EventResult_WithTimeline_SurvivesRoundTrip()
    {
        var result = new EventResult
        {
            EventId = "dragon_egg_relay",
            Seed = 42,
            Placements = new List<string> { "a1", "a2", "a3" },
            Timeline = new List<ReplayEvent>
            {
                new ReplayEvent { Timestamp=1f, AthleteId="a1", EventType=EventTypes.Surge },
                new ReplayEvent { Timestamp=2f, AthleteId="a2", EventType=EventTypes.Stumble },
            },
            Frames = new List<ReplayFrame>(),
            StatKeys = new List<string>(),
            StatValues = new List<string>()
        };

        string json = JsonConvert.SerializeObject(result);
        var restored = JsonConvert.DeserializeObject<EventResult>(json);

        Assert.AreEqual(42, restored.Seed);
        Assert.AreEqual(3, restored.Placements.Count);
        Assert.AreEqual("a1", restored.Placements[0]);
        Assert.AreEqual(2, restored.Timeline.Count);
        Assert.AreEqual(EventTypes.Surge, restored.Timeline[0].EventType);
        Assert.AreEqual(EventTypes.Stumble, restored.Timeline[1].EventType);
    }
}
