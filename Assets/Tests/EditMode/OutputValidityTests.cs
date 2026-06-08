using NUnit.Framework;
using Legends.Simulation;
using Legends.Data;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class OutputValidityTests
{
    DragonEggRelaySimulator _sim;
    EventConfigData _config;
    List<AthleteState> _athletes;

    [SetUp]
    public void SetUp()
    {
        _sim      = new DragonEggRelaySimulator();
        _config   = MakeConfig();
        _athletes = FourBalancedAthletes();
    }

    [Test]
    public void Placements_ContainsAllAthleteIds_ExactlyOnce()
    {
        var result = _sim.Simulate(_config, _athletes, 1);
        Assert.AreEqual(4, result.Placements.Count);
        var ids    = _athletes.Select(a => a.Id).OrderBy(x => x).ToList();
        var placed = result.Placements.OrderBy(x => x).ToList();
        Assert.IsTrue(ids.SequenceEqual(placed));
    }

    [Test]
    public void Timeline_HasExactlyOneFinishEvent_PerAthlete()
    {
        var result       = _sim.Simulate(_config, _athletes, 2);
        var finishEvents = result.Timeline.Where(e => e.EventType == EventTypes.Finish).ToList();
        Assert.AreEqual(4, finishEvents.Count);
        var athleteIds  = finishEvents.Select(e => e.AthleteId).OrderBy(x => x).ToList();
        var expectedIds = _athletes.Select(a => a.Id).OrderBy(x => x).ToList();
        Assert.IsTrue(athleteIds.SequenceEqual(expectedIds));
    }

    [Test]
    public void Frames_AreNonEmpty()
    {
        var result = _sim.Simulate(_config, _athletes, 3);
        Assert.IsTrue(result.Frames.Count > 0);
    }

    [Test]
    public void Frames_AreSortedByTimestamp()
    {
        var result = _sim.Simulate(_config, _athletes, 4);
        for (int i = 1; i < result.Frames.Count; i++)
            Assert.GreaterOrEqual(result.Frames[i].Timestamp, result.Frames[i - 1].Timestamp);
    }

    [Test]
    public void Frames_CoverEntireEventDuration()
    {
        var result     = _sim.Simulate(_config, _athletes, 5);
        float lastFinish = result.Timeline.Where(e => e.EventType == EventTypes.Finish).Max(e => e.Timestamp);
        float lastFrame  = result.Frames.Max(f => f.Timestamp);
        Assert.GreaterOrEqual(lastFrame, lastFinish);
    }

    [Test]
    public void EventResult_RecordsSeedCorrectly()
    {
        var result = _sim.Simulate(_config, _athletes, 12345);
        Assert.AreEqual(12345, result.Seed);
    }

    [Test]
    public void EventResult_EventId_IsDragonEggRelay()
    {
        var result = _sim.Simulate(_config, _athletes, 0);
        Assert.AreEqual("dragon_egg_relay", result.EventId);
    }

    [Test]
    public void Timeline_ContainsProgressEvents_ForEachSegment_PerAthlete()
    {
        var result = _sim.Simulate(_config, _athletes, 6);
        foreach (var athlete in _athletes)
        {
            int progressCount = result.Timeline
                .Count(e => e.EventType == EventTypes.Progress && e.AthleteId == athlete.Id);
            Assert.AreEqual(_config.NumSegments, progressCount,
                $"Athlete {athlete.Id} should have {_config.NumSegments} progress events");
        }
    }

    EventConfigData MakeConfig() => new EventConfigData
    {
        NumSegments = 10, SegmentLength = 5f, BaseSegmentTime = 1.5f,
        StumbleTimePenalty = 1.2f, EggDropTimePenalty = 2.5f, SurgeTimeBonus = 0.6f,
    };

    List<AthleteState> FourBalancedAthletes() => new List<AthleteState>
    {
        new AthleteState{Id="a1",Name="A",Stats=new AthleteStats{Dexterity=5,Constitution=5,Focus=5,Strength=5,Luck=5}},
        new AthleteState{Id="a2",Name="B",Stats=new AthleteStats{Dexterity=5,Constitution=5,Focus=5,Strength=5,Luck=5}},
        new AthleteState{Id="a3",Name="C",Stats=new AthleteStats{Dexterity=5,Constitution=5,Focus=5,Strength=5,Luck=5}},
        new AthleteState{Id="a4",Name="D",Stats=new AthleteStats{Dexterity=5,Constitution=5,Focus=5,Strength=5,Luck=5}},
    };
}
