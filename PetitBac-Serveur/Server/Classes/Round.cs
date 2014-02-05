using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac_Serveur
{
    public class Round
    {

        string letter;
        int number;
        bool isfinish;

        Game game = new Game();
          
        List<Score> listscore = new List<Score>();
        List<PlayerWords> allWords = new List<PlayerWords>();
        
        struct PlayerWords
        {
            public Player player;
            public List<string> words;
        }
        
        public Round(Game _game,int _number, string _letter)
        {
            this.game = _game;
            this.number = _number;
            this.letter = _letter;
            isfinish = false;
            foreach(Player p in game.GetPlayers())
            {
                p.Send("Game:NewRound:"+this.number+":"+this.letter);
            }
            
            
        }

        public Round()
        {
            // TODO: Complete member initialization
        }

        public string GetLetter()
        {
            return this.letter;
        }

        public List<Score> GetScore()
        {
            return this.listscore;
        }

        public string GetResult()
        {
            string scores = null;
            foreach (PlayerWords pw in allWords)
            {
                foreach (Score s in listscore)
                {
                    if (s.GetName().Equals(pw.player.Name))
                    {
                        scores += s.GetName() + ";";
                        scores += s.GetScore() + ";";
                    }
                }
                foreach (string w in pw.words)
                    scores += w + "@";
                scores += "|";
            }
            //foreach (Score s in listscore)
            //{
            //    scores += s.GetName() + ";";
            //    scores += s.GetScore() + ";";
            //    foreach (PlayerWords pw in allWords)
            //    {
            //        foreach (string w in pw.words)
            //            scores += w + "@";
            //    }
            //    scores += "|";
            //}

            return scores;
        }

        public int GetNumber()
        {
            return this.number;
        }

        public void SetScore(Player p, List<string> _results)
        {
            PlayerWords pw = new PlayerWords();
            pw.player = p;
            pw.words = _results;

            allWords.Add(pw);
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
                    if (word.StartsWith(letter, true, new System.Globalization.CultureInfo("fr-FR")))
                       score = score + 2;
                }

                foreach (string word in onePointList)
                {
                    if (word.StartsWith(letter, true, new System.Globalization.CultureInfo("fr-FR")))
                       score = score + 1;
                }
                
                //on enregistre le score final
                listscore.Add(new Score(cplayer.player.Name, score));
            }

            //if (allWords.Count.Equals(game.GetPlayers().Count))
            //{
            //    if (isfinish != true)
            //        isfinish = true;
            //}
        }

        public void AddPlayerWord(Player player, string words)
        {
            PlayerWords currentpw = new PlayerWords();
            currentpw.player = player;
            currentpw.words = new List<string>();
            foreach (string w in words.Split('|'))
            {
                currentpw.words.Add(w);
            }
            allWords.Add(currentpw);
        }
    }
}

