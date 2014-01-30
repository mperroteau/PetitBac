using PetitBac_Serveur.NetSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetitBac_Serveur.Pages
{
    /// <summary>
    /// Interaction logic for Server.xaml
    /// </summary>
    public partial class Server : UserControl
    {
        private NetServer server = new NetServer();

        private delegate void Safe(string n);
        private Safe SafeCall;

        EventHandler<NetSockConnectionRequestEventArgs> ConnectionRequested;
        EventHandler<NetSockDataArrivalEventArgs> DataArrived;
        EventHandler<NetSocketDisconnectedEventArgs> Disconnected;


        private Thread serverThread;
        private TcpListener serverListener;
        //private Hashtable clientTable;

        public Server()
        {
            InitializeComponent();

            //clientTable = new Hashtable();
            //Start a thread on the startListen method
            serverThread = new Thread(new ThreadStart(startListen));
            serverThread.Start();
            //this.server.Connected += new EventHandler<NetSocketConnectedEventArgs>(server_Connected);
            //this.server.ConnectionRequested += new EventHandler<NetSockConnectionRequestEventArgs>(server_ConnectionRequested);
            //this.server.DataArrived += new EventHandler<NetSockDataArrivalEventArgs>(server_DataArrived);
            //this.server.Disconnected += new EventHandler<NetSocketDisconnectedEventArgs>(server_Disconnected);
            //this.server.ErrorReceived += new EventHandler<NetSockErrorReceivedEventArgs>(server_ErrorReceived);
            //this.server.StateChanged += new EventHandler<NetSockStateChangedEventArgs>(server_StateChanged);

            //ConnectionRequested = new EventHandler<NetSockConnectionRequestEventArgs>(local_ConnectionRequested);
            //DataArrived = new EventHandler<NetSockDataArrivalEventArgs>(local_DataArrived);
            //Disconnected = new EventHandler<NetSocketDisconnectedEventArgs>(local_Disconnected);

            //this.SafeCall = new Safe(Log_Local);
            //this.Load += new EventHandler(server_Load);
            //this.server.Listen(9999);



            
        }

        public void startListen()
        {
            try
            {
                //Start the tcpListner
                serverListener = new TcpListener(9999);
                serverListener.Start();
                AddLog("Serveur lancé sur le port 9999");
                do
                {
                    //Create a new class when a new Chat Client connects
                    Player newPlayer = new Player(serverListener.AcceptTcpClient());
                    //Attach the Delegates
                    newPlayer.Disconnected += new DisconnectDelegate(OnDisconnected);
                    newPlayer.Connected += new ConnectDelegate(this.OnConnected);
                    newPlayer.MessageReceived += new MessageDelegate(OnMessageReceived);
                    //Connect to the clients
                    newPlayer.Connect();
                }
                while (true);
            }
            catch (Exception e)
            {
                AddLog(e.Message);
                //serverListener.Stop();
            }
        }

        public void OnConnected(object sender, EventArgs e)
        {
            //Get the client that raised the vent
            Player currentplayer = (Player)sender;
            //Add the client to the Hashtable
            //Ajouter le client a la liste
            Data.Instance.AddPlayer(currentplayer);
            currentplayer.Send("Player:All:"+Data.Instance.GetStringPlayers());
            //clientTable.Add(currentplayer.ID, currentplayer);            
            //AddLog("Client connecté:" + temp.UserName);
            AddLog("Nouveau joueur : " + currentplayer.Name);


            //loop through each client and announce the 
            //client connected

            foreach (Player tempPlayer in Data.Instance.GetListPlayer())
            {
                tempPlayer.Send("Player:New:"+currentplayer.Name);
            }
        }

        public void OnDisconnected(object sender, EventArgs e)
        {
            //Get the Client that raised the Event
            Player currentplayer = (Player)sender;
            //If the Client exists in the Hashtable

            //if (clientTable.ContainsKey(currentplayer.ID))
            if (Data.Instance.ListPlayers().Contains(currentplayer))
            {
                string uname = currentplayer.Name;
                AddLog("Déconnection :" + currentplayer.Name);
                //Remove the client from the hashtable
                Data.Instance.RemovePlayer(currentplayer);
                //Remove the client from the ClientList class
                ClientList.RemoveClient(currentplayer.Name, currentplayer.ID);
                Client tempClient;
                
                //Announce to all the existing clients

                //foreach (DictionaryEntry d in clientTable)
                //{
                //    tempClient = (Client)d.Value;
                //    tempClient.Send(tempClient.ID + "@Disconnected@" + currentplayer.Name);
                //}
            }
        }

        public void OnMessageReceived(object sender, MessageEventArgs e)
        {
            //Message sender client
            Player currentplayer = (Player)sender;
            string commande = e.Message;

            //Si les messages ont mergé, divise les deux messages
            if (currentplayer.GetLastMessage() != null)
            {

                if (e.Message.StartsWith(currentplayer.GetLastMessage()))
                    commande = e.Message.Replace(currentplayer.GetLastMessage(), null);

            }
            currentplayer.SetLastMessage(e.Message);

            TextAnalysis(currentplayer, commande, sender, e);
            
            e = new MessageEventArgs();
            //AddLog(currentplayer.Name + " :" + e.Message);
            

            //Player tempClient;
            //Send the message to each client

            //foreach (Player p in Data.Instance.ListPlayer())
            //{
            //    //tempClient = (Player)d.Value;
            //    p.Send(currentplayer.Name + " :" + e.Message);
            //}

            //C'est le client qui va demander au serveur
        }

        private void TextAnalysis(Player currentplayer, string commande, object sender, EventArgs e)
        {
            int counter = 0;

            
            if (commande.Split(new Char[] { ':' })[0] == "Game")
            {
                #region old
                if (commande.Split(new Char[] { ':' })[1] == "New")
                {
                    this.AddLog("Le joueur " + currentplayer.Name + " a crée un nouveau jeu : " + commande.Split(new Char[] { ':' })[2]);
                    //Game newgame = new Game(commande.Split(new Char[] { ':' })[2], currentplayer);
                    this.AddLog("Debug : " + commande);
                    this.Dispatcher.BeginInvoke(this.DataArrived, sender, e);
                    //currentplayer.Send("Game:All:" + Data.Instance.ListPlayers());
                    //e.Message = null;

                    return;
                }

                if (commande.Split(new Char[] { ':' })[1] == "GetAll")
                {
                    string sendcontent = "Game:All:";
                    foreach (Game g in Data.Instance.ListGame())
                    {
                        counter++;
                        if (counter != Data.Instance.ListGame().Count)
                            sendcontent += g.GetName() + "|";
                        else
                            sendcontent += g.GetName();
                    }
                    currentplayer.Send(sendcontent);
                    this.AddLog("Debug Send : " + sendcontent);
                }
                #endregion

                if (commande.Split(new Char[] { ':' })[1] == "Start")
                {
                    this.AddLog("Le joueur " + currentplayer.Name + " a lancé la partie");
                    string listcat = commande.Split(new Char[] { ':' })[2];
                    List<Player> playerlist = new List<Player>();
                    foreach (Player p in Data.Instance.GetListPlayer()){
                        playerlist.Add(p);
                    }
                    foreach (Player r in playerlist){
                        r.Send("Game:Start"+currentplayer.Name);
                    }
                    Game newgame = new Game(commande.Split(new Char[] { ':' })[2], currentplayer, playerlist);
                    this.Dispatcher.BeginInvoke(this.DataArrived, sender, e);

                    return;
                }
                if (commande.Split(new Char[] { ':' })[0] == "Player")
                {
                    if (commande.Split(new Char[] { ':' })[1] == "NewMessage"){
                        foreach (Player p in Data.Instance.GetListPlayer())
                        {
                            p.Send("Player:NewMessage:" + currentplayer + "" + commande.Split(new Char[] { ':' })[2]);
                        }
                    }
                
                }
            }
            
        }


        //Method to add the string to the server log
        public void AddLog(string msg)
        {
             this.Log_Local(msg);
        }


        private void Log_Local(string n)
        {
            //this.listBox1.Items.Add(n);
            //this.ActionServer.Text = this.ActionServer.Text + "\n" + n.ToString();
            if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
            {
                Application.Current.Dispatcher.Invoke(new Action(() => this.ActionServer.Items.Add(n)));
            }
            else
            {
                this.ActionServer.Items.Add(n);
            }
            //this.ActionServer.Items.Add(n);C:\Users\Marine\Documents\GitHub\PetitBac\PetitBac-Serveur\PetitBac-Serveur\Classes\Round.cs
            
        }

        
    }
}
