using System.Collections.Generic;
using Legends.Data;

namespace Legends.Simulation
{
    public interface IEventSimulator
    {
        EventResult Simulate(EventConfigData config, List<AthleteState> athletes, int seed);
    }
}
