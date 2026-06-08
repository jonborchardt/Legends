using NUnit.Framework;
using Legends.Simulation;
using Legends.Data;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class DeterminismTests
{
    DragonEggRelaySimulator _sim;
    EventConfigData _config;
    List<AthleteState> _athletes;

    [SetUp]
    public void SetUp()
    {
        _sim     = new DragonEggRelaySimulator();
        _config  = MakeConfig();
        _athletes = FourBalancedAthletes();
    }

    [Test]
    public void SameSeed_ProducesIdenticalPlacements()
    {
        var r1 = _sim.Simulate(_config, _athletes, 42);
        var r2 = _sim.Simulate(_config, _athletes, 42);
        Assert.IsTrue(r1.Placements.SequenceEqual(r2.Placements));
    }

    [Test]
    public void SameSeed_ProducesIdenticalTimeline()
    {
        var r1 = _sim.Simulate(_config, _athletes, 99);
        var r2 = _sim.Simulate(_config, _athletes, 99);
        Assert.AreEqual(r1.Timeline.Count, r2.Timeline.Count);
        for (int i = 0; i < r1.Timeline.Count; i++)
        {
            Assert.AreEqual(r1.Timeline[i].EventType, r2.Timeline[i].EventType);
            Assert.AreEqual(r1.Timeline[i].AthleteId,  r2.Timeline[i].AthleteId);
            Assert.AreEqual(r1.Timeline[i].Timestamp,  r2.Timeline[i].Timestamp, 0.0001f);
        }
    }

    [Test]
    public void SameSeed_ProducesIdenticalFramePositions()
    {
        var r1 = _sim.Simulate(_config, _athletes, 7);
        var r2 = _sim.Simulate(_config, _athletes, 7);
        Assert.AreEqual(r1.Frames.Count, r2.Frames.Count);
        for (int i = 0; i < r1.Frames.Count; i++)
        {
            Assert.AreEqual(r1.Frames[i].Position[0], r2.Frames[i].Position[0], 0.0001f);
            Assert.AreEqual(r1.Frames[i].Position[2], r2.Frames[i].Position[2], 0.0001f);
        }
    }

    [Test]
    public void DifferentSeeds_ProduceDifferentWinners_MostOfTheTime()
    {
        int differentCount = 0;
        for (int i = 0; i < 20; i++)
        {
            var r1 = _sim.Simulate(_config, _athletes, i);
            var r2 = _sim.Simulate(_config, _athletes, i + 1000);
            if (r1.Placements[0] != r2.Placements[0])
                differentCount++;
        }
        Assert.GreaterOrEqual(differentCount, 8, "Expected at least 8/20 pairs to have different winners");
    }

    EventConfigData MakeConfig() => new EventConfigData
    {
        NumSegments = 10, SegmentLength = 5f, BaseSegmentTime = 1.5f,
        StumbleTimePenalty = 1.2f, EggDropTimePenalty = 2.5f, SurgeTimeBonus = 0.6f,
    };

    List<AthleteState> FourBalancedAthletes() => new List<AthleteState>
    {
        new AthleteState { Id="a1", Name="A", Stats=new AthleteStats{Dexterity=5,Constitution=5,Focus=5,Strength=5,Luck=5} },
        new AthleteState { Id="a2", Name="B", Stats=new AthleteStats{Dexterity=5,Constitution=5,Focus=5,Strength=5,Luck=5} },
        new AthleteState { Id="a3", Name="C", Stats=new AthleteStats{Dexterity=5,Constitution=5,Focus=5,Strength=5,Luck=5} },
        new AthleteState { Id="a4", Name="D", Stats=new AthleteStats{Dexterity=5,Constitution=5,Focus=5,Strength=5,Luck=5} },
    };
}
