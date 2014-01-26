using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac
{
    public class Game
    {
        string name;

        Player leader;
        List<Player> listplayer;

        int minPlayer;
        int maxPlayer;

        int roundnumber;
        int roundtimer;

        List<Round> listround;
        private string game;

        public Game()
        {
        }

        public Game(string _name)
        {
            // TODO: Complete member initialization
            this.name = _name;
        }

        public string GetName()
        {
            return name;
        }
    }
}
