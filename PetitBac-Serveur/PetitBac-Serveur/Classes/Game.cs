using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac_Serveur
{
    public class Game
    {
        string name;

        Player leader;
        List<Player> listplayer;

        List<string> letters = null;

        int minPlayer;
        int maxPlayer;

        int roundnumber;
        int roundtimer;

        List<Round> listround;

        public Game(String _name, Player _leader, int _minPlayer, int _maxPlayer, int _roundnumber, int _roundtimer)
        {
            this.name = _name;
            this.leader = _leader;

            this.minPlayer = _minPlayer;
            this.maxPlayer = _maxPlayer;

            this.roundnumber = _roundnumber;
            this.roundtimer = _roundtimer;

            this.listplayer = new List<Player>();
            this.listplayer.Add(leader);
        }

        public Game(String _name, Player _leader)
        {
            this.name = _name;
            this.leader = _leader;

            //this.minPlayer = _minPlayer;
            //this.maxPlayer = _maxPlayer;

            //this.roundnumber = _roundnumber;
            //this.roundtimer = _roundtimer;

            this.listplayer = new List<Player>();
            this.listplayer.Add(leader);
            Data.Instance.ListGame().Add(this);
        }

        public string GetName()
        {
            return this.name;
        }

        public List<Player> GetPlayers()
        {
            return this.listplayer;
        }

        public int GetFinalScore(Player p)
        {
            int finalscore = 0;

            foreach (Round r in this.listround)
            {
                foreach (Score s in r.GetScore())
                {
                    if (s.GetName().Equals(p.Name))
                        finalscore = finalscore + s.GetScore();
                }
            }
            return finalscore;
        }
    }
}
