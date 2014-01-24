using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac_Serveur
{
    class Round
    {

        string letter;
        bool isfinish;

        Game game;
          
        List<Score> listscore = new List<Score>();
        List<PlayerWords> allWords = new List<PlayerWords>();
        
        struct PlayerWords
        {
            public Player player;
            public List<string> words;
        }
        
        public Round(string _letter)
        {
            this.letter = _letter;

            isfinish = false;
        }

        public string GetLetter()
        {
            return this.letter;
        }

        public List<Score> GetScore()
        {
            return this.listscore;
        }

        public void SetScore(Player p, List<string> _results)
        {
            PlayerWords pw;
            pw.player = p;
            pw.words = _results;

            AllWords.Add(pw);
        }

        public void Count()
        {
            int score;
            foreach(PlayerWords cplayer in allWords)
            {
                score = 0;

                List<string> twoPointList = new List<string>();
                List<string> onePointList = new List<string>();

                //On n'exclu de la liste de mots tous les mots similaires des autres joueurs 
                twoPointList = cplayer.words;
                foreach (PlayerWords oplayer in allWords)
                {         
                    if (oplayer.player != cplayer.player)
                        twoPointList = twoPointList.Except(oplayer.words).ToList();
                }

                //On créer une liste "inversée" afin de calculer les mots valant 1 point
                onePointList = cplayer.words.Except(twoPointList).ToList();
                
                //On pacours les listes et vérifient que celles ci commencent bien par la bonne lettre
                foreach (string word in twoPointList)
                {
                    if (word.StartsWith(letter))
                       score = score + 2;
                }

                foreach (string word in onePointList)
                {
                    if (word.StartsWith(letter))
                       score = score + 1;
                }
                
                //on enregistre le score final
                listscore.Add(new Score(cplayer.player.Name, score));
            }

            if (isfinish != true)
                isfinish = true;
        }
    }
}

