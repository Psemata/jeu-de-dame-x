using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeDameX.Model
{
    class Player
    {
        public String Name { get; set; }
        public int Team { get; set; }
        public int PowerGauge { get; set; }
        public Player(String name, int team)
        {
            Name = name;
            Team = team;
            PowerGauge = 0;
        }
    }
}
