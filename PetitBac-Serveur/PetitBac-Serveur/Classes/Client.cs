
using System;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace PetitBac_Serveur
{
    //Public delegates	
    public delegate void ConnectDelegate(object sender, EventArgs e);
    public delegate void DisconnectDelegate(object sender, EventArgs e);
    public delegate void MessageDelegate(object sender, MessageEventArgs e);

    //Custom EventArgs
    public class MessageEventArgs : EventArgs
    {
        private string msg;
        public string Message
        {
            get
            {
                return this.msg;
            }
            set
            {
                this.msg = value;
            }
        }
    }

    public class Client
    {
        //Events Defination
        public event ConnectDelegate Connected;
        public event DisconnectDelegate Disconnected;
        public event MessageDelegate MessageReceived;
        //Some Variables
        private bool firstTime = true;
        private bool connected = false;
        private byte[] recByte = new byte[1024];
        private StringBuilder myBuilder = new StringBuilder();
        private TcpClient myClient;
        private string name, clientID;

        public Client(TcpClient myClient)
        {
            this.myClient = myClient;
        }
        //Connect method used to connect to Client
        public void Connect()
        {

            //Assign a new Guid
            this.clientID = Guid.NewGuid().ToString();
            //Start Receiving the Messages
            AsyncCallback GetStreamMsgCallback = new AsyncCallback(GetStreamMsg);
            myClient.GetStream().BeginRead(recByte, 0, 1024, GetStreamMsgCallback, null);
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string ID
        {
            get
            {
                return this.clientID;
            }
        }

        public void GetStreamMsg(IAsyncResult ar)
        {
            int intCount;
            try
            {
                //Lock the Client Stream
                lock (myClient.GetStream())
                {
                    //Receive the Bytes
                    intCount = myClient.GetStream().EndRead(ar);
                }
                if (intCount < 1)
                {
                    //If a value less than 1 received that means that 
                    //client disconnected
                    myClient.Close();
                    //raise the Disconnected Event
                    if (Disconnected != null)
                    {
                        EventArgs e = new EventArgs();
                        Disconnected(this, e);
                    }
                }
                //Send the received message from processing
                BuildText(recByte, 0, intCount);
                if (!firstTime)
                {
                    //if its not the first time then restart the listen process
                    lock (myClient.GetStream())
                    {
                        AsyncCallback GetStreamMsgCallback = new AsyncCallback(GetStreamMsg);
                        myClient.GetStream().BeginRead(recByte, 0, 1024, GetStreamMsgCallback, null);
                    }
                }
            }
            catch (Exception e)
            {
                string ex = e.Message;
                //myClient.Close();
                //if (Disconnected != null)
                //{
                //    EventArgs e = new EventArgs();
                //    Disconnected(this, e);
                //}
            }
        }

        public void Disconnect()
        {
            this.connected = false;
            myClient.Close();
        }
        //Method that takes the Usernam and does some processing
        public void CheckName(string name)
        {
            if (name.Length > 20)
            {
                Send("Désolé@name est trop long, veuillez entrer un pseudo de moins de 20 caractères!!");
                Disconnect();
                return;
            }
            else if (name.IndexOf("@") >= 0)
            {
                Send("Désolé@Invalid Character in name!!");
                Disconnect();
                return;
            }
            else if (!ClientList.AddClient(name, this.clientID))
            {
                //Check if the name is duplicate
                Send("exist");
                Disconnect();
                return;
            }
            else
            {
                //If name is not duplicate then the client is connected
                this.connected = true;
                this.name = name.Split(':')[2];
                //Build the names list and send it to the client
                StringBuilder userList = new StringBuilder();
                userList.Append(this.clientID);
                Hashtable clientTable = ClientList.GetList;
                foreach (DictionaryEntry d in clientTable)
                {
                    //Seperate the names by a '@'
                    userList.Append("@");
                    userList.Append(d.Value.ToString());
                }
                //Start the llistening
                lock (myClient.GetStream())
                {
                    AsyncCallback GetStreamMsgCallback = new AsyncCallback(GetStreamMsg);
                    myClient.GetStream().BeginRead(recByte, 0, 1024, GetStreamMsgCallback, null);
                }
                //Send the Userlist
                Send(userList.ToString());
                //Raise the Connected Event
                if (Connected != null)
                {
                    EventArgs e = new EventArgs();
                    Connected(this, e);
                }
            }
        }

        //Method to process the Messages
        public void BuildText(byte[] dataByte, int offset, int count)
        {

            for (int i = 0; i < count; i++)
            {
                //Check is a line terminator is encountered
                if (dataByte[i] == 13)
                {

                    if (firstTime)
                    {
                        //If first time then call the Checkname method
                        CheckName(myBuilder.ToString().Trim());
                        firstTime = false;
                    }
                    else if (MessageReceived != null && connected)
                    {
                        //Else raise the MessageReceived Event 
                        //and pass the message along
                        MessageEventArgs e = new MessageEventArgs();
                        e.Message = (myBuilder.ToString()).Trim();
                        MessageReceived(this, e);
                    }
                    //Clear the StringBuilder
                    myBuilder = new System.Text.StringBuilder();
                }
                else
                {
                    //Append the Byte to the StringBuilder
                    myBuilder.Append(Convert.ToChar(dataByte[i]));
                }
            }
        }

        //Method to send the message
        public void Send(string msg)
        {
            lock (myClient.GetStream())
            {
                System.IO.StreamWriter myWriter = new System.IO.StreamWriter(myClient.GetStream());
                myWriter.Write(msg);
                myWriter.Flush();
            }
        }
    }

    //Class to maintain the Userlist
    public class ClientList
    {
        private static Hashtable clientTable = new Hashtable();

        //Method to add a new user
        public static bool AddClient(string name, string id)
        {
            lock (clientTable)
            {
                //If name exists return false
                if (clientTable.ContainsValue(name))
                {
                    return false;
                }
                else
                {
                    //Or add the name to the list and return true
                    clientTable.Add(id, name);
                    return true;
                }
            }
        }

        //Method to remove the user
        public static bool RemoveClient(string name, string id)
        {
            lock (clientTable)
            {
                if (clientTable.ContainsValue(name))
                {
                    clientTable.Remove(id);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        //Property to get the Users list
        public static Hashtable GetList
        {
            get
            {
                return clientTable;
            }
        }

    }
}
