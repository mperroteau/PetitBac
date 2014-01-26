using PetitBac.NetSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO; 

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
                client = new TcpClient("localhost", 9999);         
            }
            catch (Exception e)
            {
                string ex = e.Message;
            }
        }


        public TcpClient GetClient()
        {
            return client;
        }

        //Method to send a message to the server
        public void Send(string data)
        {
            try
            {
                //Get a StreamWriter 
                System.IO.StreamWriter writer = null;
                writer = new System.IO.StreamWriter(client.GetStream());
                writer.WriteLine(data);
                //Flush the stream
                writer.Flush();
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
            while (true)
            {
                int byteCount;
                try
                {
                    //Get the number of Bytes received
                    byteCount = (client.GetStream()).EndRead(ar);
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
                    //Unless its the first time start Asynchronous Read
                    //Again

                    if (firstTime)
                    {
                        AsyncCallback GetMsgCallback = new AsyncCallback(GetMsgServer);
                        (client.GetStream()).BeginRead(recByte, 0, 1024, GetMsgCallback, this);
                        firstTime = false;
                    }
                    
                }
                catch (Exception ed)
                {
                    Disconnect();
                    //MessageBox.Show("Exception Occured :" + ed.ToString());
                }
            }
        }

        public void CallbackMethod(IAsyncResult ar)
        {
            try
            {
                // Retrieve the delegate.
                AsyncResult result = (AsyncResult)ar;
                //AsyncMethodCaller caller = (AsyncMethodCaller)result.AsyncDelegate;
                AsyncCallback caller = (AsyncCallback)result.AsyncDelegate;


                // Retrieve the format string that was passed as state 
                // information.
                string formatString = (string)ar.AsyncState;

                // Define a variable to receive the value of the out parameter.
                // If the parameter were ref rather than out then it would have to
                // be a class-level field so it could also be passed to BeginInvoke.
                int threadId = 0;

                // Call EndInvoke to retrieve the results.
                //string returnValue = caller.EndInvoke(out threadId, ar);
                //string returnValue = caller.EndInvoke(out threadId, ar);




                // Use the format string to format the output message.
                //Console.WriteLine(formatString, threadId, returnValue);
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
        }

        //public void CallBack(IAsyncResult result)
        public void CallBack()
        {         
           NetworkStream clientStream = client.GetStream();
           StreamReader sr = new StreamReader(clientStream);
           while (true)
           {
               if (clientStream.DataAvailable)
               {
                   string data = "";
                   int i;
                   while (sr.Peek() != -1)
                   {
                       try
                       {
                           //data = sr.Read().ToString();
                           //data =
                           //Console.WriteLine("Debug before : " + data);
                           data += sr.Read().ToString();
                           data = sr.ReadLine();
                           
                           //lastRequest = data;
                           //byte[] b = System.Text.Encoding.ASCII.GetBytes(bdata);
                           //data = System.Text.Encoding.UTF8.GetString(b);
                           
                       }

                           
                       catch (Exception e)
                       {
                           string ex = e.Message;
                       }
                       
                   }
                   //string r1 = sr.ReadLine();
                   //string r = sr.ReadToEnd();
                   
               }
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
