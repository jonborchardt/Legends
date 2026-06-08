using NUnit.Framework;
using Legends.Simulation;
using Legends.Data;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class CoverageAndStatTests
{
    DragonEggRelaySimulator _sim;
    EventConfigData _config;

    [SetUp]
    public void SetUp()
    {
        _sim    = new DragonEggRelaySimulator();
        _config = MakeConfig();
    }

    [Test]
    public void AllSevenEventTypes_AppearAcross_100_Runs()
    {
        var allEventTypes = new System.Collections.Generic.HashSet<string>();
        for (int seed = 0; seed < 100; seed++)
        {
            var result = _sim.Simulate(_config, FourBalancedAthletes(), seed);
            foreach (var evt in result.Timeline)
                allEventTypes.Add(evt.EventType);
        }

        string[] required =
        {
            EventTypes.Progress,
            EventTypes.Surge,
            EventTypes.Stumble,
            EventTypes.EggWobble,
            EventTypes.EggRecovery,
            EventTypes.EggDrop,
            EventTypes.Finish,
        };
        foreach (var type in required)
            Assert.IsTrue(allEventTypes.Contains(type), $"Missing event type: {type}");
    }

    [Test]
    public void EggDrop_OccursInAtLeast_10_Of_100_Runs()
    {
        int dropRuns = 0;
        for (int seed = 0; seed < 100; seed++)
        {
            var result = _sim.Simulate(_config, FourBalancedAthletes(), seed);
            if (result.Timeline.Any(e => e.EventType == EventTypes.EggDrop))
                dropRuns++;
        }
        Assert.GreaterOrEqual(dropRuns, 10, $"Only {dropRuns}/100 runs had an egg drop");
    }

    [Test]
    public void HighStrength_AthletesFinishFaster_In_35_Of_50_Runs()
    {
        var strong = new List<AthleteState>
        {
            new AthleteState { Id="s1", Name="Strong",
                Stats=new AthleteStats{Dexterity=5,Constitution=5,Focus=5,Strength=10,Luck=5} },
            new AthleteState { Id="w1", Name="Weak",
                Stats=new AthleteStats{Dexterity=5,Constitution=5,Focus=5,Strength=1,Luck=5} },
        };

        int strongWins = 0;
        for (int seed = 0; seed < 50; seed++)
        {
            var result = _sim.Simulate(_config, strong, seed);
            if (result.Placements[0] == "s1")
                strongWins++;
        }
        Assert.GreaterOrEqual(strongWins, 35, $"Strong athlete won only {strongWins}/50 races");
    }

    [Test]
    public void HighLuck_ReducesEggDropRate_ComparedToLowLuck()
    {
        var lucky   = MakeAthletes(luck: 10);
        var unlucky = MakeAthletes(luck: 1);

        int luckyDrops = 0, unluckyDrops = 0;
        for (int seed = 0; seed < 50; seed++)
        {
            luckyDrops   += _sim.Simulate(_config, lucky,   seed).Timeline.Count(e => e.EventType == EventTypes.EggDrop);
            unluckyDrops += _sim.Simulate(_config, unlucky, seed).Timeline.Count(e => e.EventType == EventTypes.EggDrop);
        }

        Assert.Greater(unluckyDrops, luckyDrops,
            $"Unlucky drops ({unluckyDrops}) should exceed lucky drops ({luckyDrops})");
    }

    [Test]
    public void HighConstitution_ReducesStumbleRate_ComparedToLowConstitution()
    {
        var sturdy = MakeAthletes(constitution: 10);
        var frail  = MakeAthletes(constitution: 1);

        int sturdyStumbles = 0, frailStumbles = 0;
        for (int seed = 0; seed < 50; seed++)
        {
            sturdyStumbles += _sim.Simulate(_config, sturdy, seed).Timeline.Count(e => e.EventType == EventTypes.Stumble);
            frailStumbles  += _sim.Simulate(_config, frail,  seed).Timeline.Count(e => e.EventType == EventTypes.Stumble);
        }

        Assert.Greater(frailStumbles, sturdyStumbles,
            $"Frail stumbles ({frailStumbles}) should exceed sturdy stumbles ({sturdyStumbles})");
    }

    EventConfigData MakeConfig() => new EventConfigData
    {
        NumSegments=10, SegmentLength=5f, BaseSegmentTime=1.5f,
        StumbleTimePenalty=1.2f, EggDropTimePenalty=2.5f, SurgeTimeBonus=0.6f,
    };

    List<AthleteState> FourBalancedAthletes() => MakeAthletes();

    List<AthleteState> MakeAthletes(int dex=5, int constitution=5, int focus=5, int strength=5, int luck=5)
        => new List<AthleteState>
        {
            new AthleteState{Id="a1",Name="A",Stats=new AthleteStats{Dexterity=dex,Constitution=constitution,Focus=focus,Strength=strength,Luck=luck}},
            new AthleteState{Id="a2",Name="B",Stats=new AthleteStats{Dexterity=dex,Constitution=constitution,Focus=focus,Strength=strength,Luck=luck}},
            new AthleteState{Id="a3",Name="C",Stats=new AthleteStats{Dexterity=dex,Constitution=constitution,Focus=focus,Strength=strength,Luck=luck}},
            new AthleteState{Id="a4",Name="D",Stats=new AthleteStats{Dexterity=dex,Constitution=constitution,Focus=focus,Strength=strength,Luck=luck}},
        };
}
