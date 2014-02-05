using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac
{
    public class Player
    {

        string name { get;  set; }
        Game currentGame {get; set;}

        public Player(string _n)
        {
            this.name = _n;
            this.currentGame = null;

            AddUser();
        }

        public string GetName()
        {
            return this.name;
        }

        public bool ifCurrentGame()
        {
            if (this.currentGame == null)
                    return false;
            else
                return true;
        }

        private void AddUser()
        {
            /*
             Ajoute l'utilisateur dans le serveur socket
             */
        }

        private void DeleteUser()
        {
            /*
             Retire l'utilisateur dans le serveur socket
             */
        }
    }
}
