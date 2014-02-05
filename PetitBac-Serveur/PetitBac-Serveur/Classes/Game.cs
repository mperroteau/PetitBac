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
        int countround;

        Player leader;
        List<Player> listplayer;

        List<string> letters = null;
        List<Round> listround;

        string dico = "ABCDEFGHJKLMNOPQRSTUVWXYZ";

        public Game(String _name, Player _leader, List<Player> _listplayer)
        {
            this.name = _name;
            this.leader = _leader;
            this.listplayer = _listplayer;
            this.countround = 1;
            
            Round firstround = new Round(this,countround, GetRandomLetter());

            //Data.Instance.ListGame().Add(this);
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

        private string GetRandomLetter()
        {
            Random rand = new Random();
            int max = dico.Length;
            int index = rand.Next(0, dico.Length);
            
            return dico[index].ToString();
        }
    }
}
