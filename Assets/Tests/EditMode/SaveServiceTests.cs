using NUnit.Framework;
using Legends.Data;
using Legends.Services;
using System.Collections.Generic;

[TestFixture]
public class SaveServiceTests
{
    PlayerPrefsSaveService _svc;

    [SetUp]
    public void SetUp()
    {
        _svc = new PlayerPrefsSaveService();
        _svc.Delete(); // clean slate before each test
    }

    [TearDown]
    public void TearDown()
    {
        _svc.Delete(); // clean up after each test
    }

    [Test]
    public void Exists_ReturnsFalse_BeforeFirstSave()
    {
        Assert.IsFalse(_svc.Exists());
    }

    [Test]
    public void Exists_ReturnsTrue_AfterSave()
    {
        _svc.Save(MakeState());
        Assert.IsTrue(_svc.Exists());
    }

    [Test]
    public void Load_ReturnsNull_WhenNoSaveExists()
    {
        var result = _svc.Load();
        Assert.IsNull(result);
    }

    [Test]
    public void Save_ThenLoad_PreservesTeamName()
    {
        var state = MakeState("TestTeam");
        _svc.Save(state);
        var loaded = _svc.Load();
        Assert.AreEqual("TestTeam", loaded.Team.TeamName);
    }

    [Test]
    public void Save_ThenLoad_PreservesAthleteCount()
    {
        var state = MakeState();
        _svc.Save(state);
        var loaded = _svc.Load();
        Assert.AreEqual(2, loaded.Team.Roster.Count);
    }

    [Test]
    public void Delete_CausesExists_ToReturnFalse()
    {
        _svc.Save(MakeState());
        _svc.Delete();
        Assert.IsFalse(_svc.Exists());
    }

    GameState MakeState(string teamName = "The Wanderers") => new GameState
    {
        SaveVersion = "1.0",
        Team = new TeamState
        {
            TeamName = teamName,
            Roster = new List<AthleteState>
            {
                new AthleteState { Id = "a1", Name = "Kira", Stats = new AthleteStats() },
                new AthleteState { Id = "a2", Name = "Orin", Stats = new AthleteStats() },
            }
        },
        CompletedEvents = new List<EventResult>()
    };
}
