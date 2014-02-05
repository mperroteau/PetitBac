using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace PetitBac
{
    //Singleton servant à déterminer un NetClient pour l'ensemble du programme

    public sealed class Client
    {
        public static Client Instance { get; private set; }
        private TcpClient client;

        //Flag to check if this is the first communication with the server
        bool firstTime = true;
        private byte[] recByte = new byte[1024];

        //Identifiants du client
        private StringBuilder myBuilder = new StringBuilder();
        private string userID, name;

        public List<Game> ClientGames = new List<Game>();

        private Client()
        {
            try
            {
                //Connect to server
                //client = new TcpClient("localhost", 9999);         
            }
            catch (Exception e)
            {
                string ex = e.Message;
            }
        }


        public TcpClient GetClient()
        {
            if (client != null)
            {
                return client;
            }
            else
                return null;
        }

        //Method to send a message to the server
        public void Send(string data)
        {
            try
            {
                if (client != null)
                {
                    //Get a StreamWriter 
                    System.IO.StreamWriter writer = null;
                    writer = new System.IO.StreamWriter(client.GetStream());
                    writer.WriteLine(data);
                    //Flush the stream
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                string ex = e.Message;
                return;
            }
        }

        //Method to get a message from the server
        public void GetMsgServer(IAsyncResult ar)
        {
            
                int byteCount;
                try
                {
                    if (client != null)
                    {
                        //Get the number of Bytes received

                        
                        NetworkStream mystream = client.GetStream();
                        //ERREUR DE MERDE !!!!!!!! EnRead
                        byteCount = mystream.EndRead(ar);
                        //close networkstream
                        //ne fonctionne qu'une fois, a cause du mystream qui n'est pas fermé
                        //EndRead ne peut être utilisé q'une seule fois par instance, sinon voir le projet de base


                        //byteCount = (client.GetStream()).EndRead(ar);


                        //If bytes received is less than 1 it means
                        //the server has disconnected
                        if (byteCount < 1)
                        {
                            //Close the socket
                            Disconnect();
                            //MessageBox.Show("Disconnected!!");
                            return;
                        }
                        //Send the Server message for parsing
                        BuildText(recByte, 0, byteCount);

                        //break si un message valide est envoyé

                        //Unless its the first time start Asynchronous Read
                        //Again

                        //if (firstTime)
                        //{
                        //    if (client != null)
                        //    {
                                
                        //        AsyncCallback GetMsgCallback = new AsyncCallback(GetMsgServer);
                        //        (client.GetStream()).BeginRead(recByte, 0, 1024, GetMsgCallback, this);
                        //        firstTime = false;
                        //    }
                        //}
                    }
                    
                }
                catch (Exception ed)
                {
                    Disconnect();
                    //MessageBox.Show("Exception Occured :" + ed.ToString());
                }
            
        }


        //Method to Process Server Response
        public void BuildText(byte[] dataByte, int offset, int count)
        {
            //Loop till the number of bytes received
            for (int i = offset; i < (count); i++)
            {
                //If a New Line character is met then
                //skip the loop cycle
                if (dataByte[i] == 10)
                    continue;
                //Add the Byte to the StringBuilder in Char format
                //Exception null -> Déconnection
                myBuilder.Append(Convert.ToChar(dataByte[i]));
            }
            //char[] spliters = { '@' };
            //Check if this is the first message received
            //if (firstTime)
            //{
            //    string text = myBuilder.ToString();

            //        firstTime = false;
            //        //Start the listening process again 
            //        AsyncCallback GetMsgCallback = new AsyncCallback(GetMsgServer);
            //        (client.GetStream()).BeginRead(recByte, 0, 1024, GetMsgCallback, this);

            //}

            string data = myBuilder.ToString();

            if (data.Split(':')[0] == "Game")
            {
                if (data.Split(':')[1] == "All")
                {
                    ClientGames.Clear();
                    string[] allgames = data.Split(':')[2].Split('|');
                    //client passe a null
                    foreach (string game in allgames)
                    {
                        if (!game.Equals(""))
                        {
                            Game g = new Game(game);
                            ClientGames.Add(g);
                        }
                    }
                    //Faire un refresh de la vue ou un sleep sur un thread
                    
                }
            }
            
            //Empty the StringBuilder
            myBuilder = new System.Text.StringBuilder();
        }

        private void Disconnect()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }

        }

        static Client() { Instance = new Client(); }
    }

}
