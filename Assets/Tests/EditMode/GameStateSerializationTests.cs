using NUnit.Framework;
using Newtonsoft.Json;
using Legends.Data;
using System.Collections.Generic;

[TestFixture]
public class GameStateSerializationTests
{
    [Test]
    public void EmptyGameState_SerializesAndDeserializes_WithoutError()
    {
        var state = new GameState { SaveVersion = "1.0" };
        string json = JsonConvert.SerializeObject(state);
        var result = JsonConvert.DeserializeObject<GameState>(json);
        Assert.AreEqual("1.0", result.SaveVersion);
    }

    [Test]
    public void FullGameState_PreservesAllFields_AcrossRoundTrip()
    {
        var original = new GameState
        {
            SaveVersion = "1.0",
            Team = new TeamState
            {
                TeamName = "The Wanderers",
                Roster = new List<AthleteState>
                {
                    new AthleteState { Id = "a1", Name = "Kira",
                        Stats = new AthleteStats { Dexterity=6, Constitution=5, Focus=7, Strength=5, Luck=4 } },
                    new AthleteState { Id = "a2", Name = "Orin",
                        Stats = new AthleteStats { Dexterity=5, Constitution=8, Focus=4, Strength=7, Luck=3 } },
                }
            },
            CompletedEvents = new List<EventResult>()
        };

        string json = JsonConvert.SerializeObject(original);
        var restored = JsonConvert.DeserializeObject<GameState>(json);

        Assert.AreEqual(original.Team.TeamName, restored.Team.TeamName);
        Assert.AreEqual(2, restored.Team.Roster.Count);
        Assert.AreEqual("Kira", restored.Team.Roster[0].Name);
        Assert.AreEqual(6, restored.Team.Roster[0].Stats.Dexterity);
        Assert.AreEqual(7, restored.Team.Roster[0].Stats.Focus);
        Assert.AreEqual("Orin", restored.Team.Roster[1].Name);
    }

    [Test]
    public void AthleteStats_AllFiveFields_SurviveRoundTrip()
    {
        var stats = new AthleteStats { Dexterity=1, Constitution=2, Focus=3, Strength=4, Luck=5 };
        string json = JsonConvert.SerializeObject(stats);
        var restored = JsonConvert.DeserializeObject<AthleteStats>(json);

        Assert.AreEqual(1, restored.Dexterity);
        Assert.AreEqual(2, restored.Constitution);
        Assert.AreEqual(3, restored.Focus);
        Assert.AreEqual(4, restored.Strength);
        Assert.AreEqual(5, restored.Luck);
    }
}
