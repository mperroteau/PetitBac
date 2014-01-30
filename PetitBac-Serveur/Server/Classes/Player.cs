using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac_Serveur
{
    public class Player : Client
    {
        //String ip { get; set; }
        Game currentGame { get; set; }
        string lastmessage = null;


        public Player(TcpClient _t)
            : base(_t)
        {
            //this.name = _n;
            this.currentGame = null;
            //Data.Instance.AddPlayer(this);

        }

        public string GetLastMessage()
        {
            return this.lastmessage;
        }

        public void SetLastMessage(string _lastmessage)
        {
            this.lastmessage = _lastmessage;
        }

        public bool ifCurrentGame()
        {
            if (this.currentGame == null)
                return false;
            else
                return true;
        }

        
        
    }

    
}
