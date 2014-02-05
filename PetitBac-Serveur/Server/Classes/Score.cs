using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac_Serveur
{
    public class Score
    {
        string name;
        string round;
        int score;

        public Score(string _name, int _score)
        {
            this.name = _name;
            this.score = _score;
        }

        public int GetScore()
        {
            return this.score;
        }

        public string GetName()
        {
            return this.name;
        }

    }
}