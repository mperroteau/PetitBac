using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac
{
    class Game
    {
        string name;

        Player leader;
        List<Player> listplayer;

        int minPlayer;
        int maxPlayer;

        int roundnumber;
        int roundtimer;

        List<Round> listround;

        public Game()
        {
        }

        public string GetName()
        {
            return name;
        }
    }
}
