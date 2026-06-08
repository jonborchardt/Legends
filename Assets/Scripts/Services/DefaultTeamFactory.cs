using System;
using System.Collections.Generic;
using Legends.Data;

namespace Legends.Services
{
    public static class DefaultTeamFactory
    {
        public static TeamState Create()
        {
            return new TeamState
            {
                TeamName = "The Wanderers",
                Roster   = new List<AthleteState>
                {
                    Make("Kira",  dex:6, con:5, foc:7, str:5, lck:4),
                    Make("Orin",  dex:5, con:8, foc:4, str:7, lck:3),
                    Make("Sable", dex:7, con:4, foc:6, str:4, lck:8),
                    Make("Dax",   dex:4, con:6, foc:5, str:8, lck:5),
                }
            };
        }

        static AthleteState Make(string name, int dex, int con, int foc, int str, int lck)
        {
            return new AthleteState
            {
                Id    = Guid.NewGuid().ToString(),
                Name  = name,
                Stats = new AthleteStats
                {
                    Dexterity    = dex,
                    Constitution = con,
                    Focus        = foc,
                    Strength     = str,
                    Luck         = lck
                }
            };
        }
    }
}
