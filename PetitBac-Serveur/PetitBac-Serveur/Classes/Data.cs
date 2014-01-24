using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac_Serveur
{
    public sealed class Data
    {
        public static Data Instance { get; private set; }

        private List<Player> serverPlayers = new List<Player>();
        private List<Game> serverGames = new List<Game>();

        public List<Player> ListPlayers()
        {
            return this.serverPlayers;
        }
        public List<Game> ListGame()
        {
            return this.serverGames;
        }

        public void AddPlayer(Player p)
        {
            serverPlayers.Add(p);
        }

        public void RemovePlayer(Player p)
        {
            serverPlayers.Remove(p);
        }

        static Data() { Instance = new Data(); }
 
        
    }
}
