using PetitBac.NetSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
        private StringBuilder myBuilder;
        private string userID, name;

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
                    //Disconnect();

                    //MessageBox.Show("Disconnected!!");
                    return;
                }
                //Send the Server message for parsing
                BuildText(recByte, 0, byteCount);
                //Unless its the first time start Asynchronous Read
                //Again
                if (!firstTime)
                {
                    AsyncCallback GetMsgCallback = new AsyncCallback(GetMsgServer);
                    (client.GetStream()).BeginRead(recByte, 0, 1024, GetMsgCallback, this);
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
                myBuilder.Append(Convert.ToChar(dataByte[i]));
            }
            char[] spliters = { '@' };
            //Check if this is the first message received
            if (firstTime)
            {
                //Split the string received at the occurance of '@'
                string[] tempString = myBuilder.ToString().Split(spliters);
                //If the Server sent 'sorry' that means there was some error
                //so we just disconnect the client
                if (tempString[0] == "sorry")
                {
                    object[] temp = { tempString[1] };
                    //this.Invoke(new displayMessage(DisplayText), temp);
                    Disconnect();
                }
                else
                {
                    //Store the Client Guid 
                    this.userID = tempString[0];
                    //Loop through array of UserNames
                    for (int i = 1; i < tempString.Length; i++)
                    {
                        object[] temp = { tempString[i] };
                        //Invoke the AddUser method
                        //Since we are working on another thread rather than the primary 
                        //thread we have to use the Invoke method
                        //to call the method that will update the listbox
                        //this.Invoke(new displayMessage(AddUser), temp);
                    }
                    //Reset the flag
                    firstTime = false;
                    //Start the listening process again 
                    AsyncCallback GetMsgCallback = new AsyncCallback(GetMsgServer);
                    (client.GetStream()).BeginRead(recByte, 0, 1024, GetMsgCallback, this);
                }

            }
            else
            {
                //Generally all other messages get passed here
                //Check if the Message starts with the ClientID
                //In which case we come to know that its a Server Command
                if (myBuilder.ToString().IndexOf(this.userID) >= 0)
                {
                    string[] tempString = myBuilder.ToString().Split(spliters);
                    //If its connected command then add the user to the ListBox
                    if (tempString[1] == "Connected")
                    {
                        object[] temp = { tempString[2] };
                        //this.Invoke(new displayMessage(AddUser), temp);
                    }
                    else if (tempString[1] == "Disconnected")
                    {
                        //If its disconnected command then remove the 
                        //username from the list box
                        object[] temp = { tempString[2] };
                        //this.Invoke(new displayMessage(RemoveUser), temp);
                    }
                }
                else
                {
                    //For regular messages append a Line terminator
                    myBuilder.Append("\r\n");
                    object[] temp = { myBuilder.ToString() };
                    //Invoke the DisplayText method
                    //this.Invoke(new displayMessage(DisplayText), temp);
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
