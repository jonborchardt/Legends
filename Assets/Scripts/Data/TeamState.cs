using System;
using System.Collections.Generic;

namespace Legends.Data
{
    [Serializable]
    public class TeamState
    {
        public string TeamName;
        public List<AthleteState> Roster;
    }
}
