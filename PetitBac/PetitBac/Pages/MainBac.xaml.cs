using PetitBac.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

namespace PetitBac.Pages
{
    /// <summary>
    /// Interaction logic for MainBac.xaml
    /// </summary>
    public partial class MainBac : UserControl
    {

        #region Variables

            private List<Item> GameItems = new List<Item>();
            private List<Player> ConnectedClient = new List<Player>();
            private TcpClient client;
            //Flag to check if this is the first communication with the server
            bool firstTime = true;
            bool isLeader = false;
            private byte[] recByte = new byte[1024];
            private StringBuilder myBuilder = new StringBuilder();
            //private bool connected = false;
            private string currentlogin;
            string lastlog;
            int currentround;

        #endregion

        public MainBac()
        {
                InitializeComponent();
                SetItems();
                SetUnlogInterface();
        }
        
        /// <summary>
        /// Methode qui enregistre tous les éléments du tableau "PetitBac" dans une liste
        /// Cela permet par la suite de gérer les éléments plus facilement
        /// </summary>
        public void SetItems(){
        /*Add all game items in the list*/
            GameItems.Add(new Item(l1c1.Name, 1, 1));
            GameItems.Add(new Item(l1c2.Name, 1, 2));
            GameItems.Add(new Item(l1c3.Name, 1, 3));
            GameItems.Add(new Item(l1c4.Name, 1, 4));
            GameItems.Add(new Item(l1c5.Name, 1, 5));

            GameItems.Add(new Item(l2c1.Name, 2, 1));
            GameItems.Add(new Item(l2c2.Name, 2, 2));
            GameItems.Add(new Item(l2c3.Name, 2, 3));
            GameItems.Add(new Item(l2c4.Name, 2, 4));
            GameItems.Add(new Item(l2c5.Name, 2, 5));

            GameItems.Add(new Item(l3c1.Name, 3, 1));
            GameItems.Add(new Item(l3c2.Name, 3, 2));
            GameItems.Add(new Item(l3c3.Name, 3, 3));
            GameItems.Add(new Item(l3c4.Name, 3, 4));
            GameItems.Add(new Item(l3c5.Name, 3, 5));

            GameItems.Add(new Item(l4c1.Name, 4, 1));
            GameItems.Add(new Item(l4c2.Name, 4, 2));
            GameItems.Add(new Item(l4c3.Name, 4, 3));
            GameItems.Add(new Item(l4c4.Name, 4, 4));
            GameItems.Add(new Item(l4c5.Name, 4, 5));

            GameItems.Add(new Item(l5c1.Name, 5, 1));
            GameItems.Add(new Item(l5c2.Name, 5, 2));
            GameItems.Add(new Item(l5c3.Name, 5, 3));
            GameItems.Add(new Item(l5c4.Name, 5, 4));
            GameItems.Add(new Item(l5c5.Name, 5, 5));
           
        }

        #region Fonctions interface

            public void SetFirstUnlogInterface()
            {
                logptions.IsEnabled = true;
                //unlog.IsEnabled = false;

                //Désactive tous les éléments de jeu
                game.IsEnabled = false;
            }

            public void SetUnlogInterface()
            {
                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => this.logptions.IsEnabled = true));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.tb_ipadress.IsEnabled = true));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.tb_port.IsEnabled = true));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.tb_login.IsEnabled = true));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.login.IsEnabled = true));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.unlog.IsEnabled = false));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.game.IsEnabled = false));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.tb_message.IsEnabled = false));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.tb_clients.Text = ""));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.tb_chat.Text = ""));
                }
                else
                {
                    //logptions.IsEnabled = true;
                    this.tb_ipadress.IsEnabled = true;
                    this.tb_port.IsEnabled = true;
                    this.tb_port.IsEnabled = true;
                    this.login.IsEnabled = true;
                    this.unlog.IsEnabled = false;

                    this.tb_message.IsEnabled = false;
                    this.tb_message.Text = "";
                    this.tb_chat.Text = "";
                    //Désactive tous les éléments de jeu
                    game.IsEnabled = false;
                }
               
            }

            public void SetLogInterface(){
                //afficher si le jeu est déjà en cours (un joueur ne peut pas jouer une partie en cours)
                //Désactiver start si un jeu est déjà en cours
                //logptions.IsEnabled = false;
                this.tb_ipadress.IsEnabled = false;
                this.tb_port.IsEnabled = false;
                this.tb_port.IsEnabled = false;
                this.login.IsEnabled = false;
                this.tb_message.IsEnabled = true;

                unlog.IsEnabled = true;
                game.IsEnabled = true;
                //Pour chaque élément du jeu, parcours 
                foreach (Item i in GameItems){
                   TextBox currenti = (TextBox)game.FindName(i.getName());
                   currenti.IsEnabled = false;
                }
                stop.IsEnabled = false;
            }       

            public void UpdateListClients()
            {
                //tb_clients.Text += p.GetName() + "&#10;";
                string templist = "";
                foreach (Player p in ConnectedClient)
                {
                    templist += p.GetName() + "\r\n";
                }

                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => this.tb_clients.Text=templist));
                }
                else
                {
                    this.tb_clients.Text=templist;
                }
            }

            public void UpdateMessages(string player, string message)
            {
                string old = "";
                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => old = tb_chat.Text));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.tb_chat.Text = old + "\r\n" + player + " : "+ message));
                }
                else
                {
                    old = tb_chat.Text;
                    this.tb_clients.Text = old + "\r\n" + player + " : " + message;
                }
            }

            /// <summary>
            /// TO DEBUG
            /// </summary>
            /// <param name="commande"></param>
            /// <param name="numberound"></param>
            /// <param name="option"></param>
            public void UpdateGame(string commande, int numberound, string option){
                try
                {
                    if (commande.Equals("Disable"))
                    {
                        foreach (Item i in GameItems)
                        {
                             if (i.line.Equals(numberound))
                            {
                                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                                {
                                    TextBox currenti = null;
                                    Application.Current.Dispatcher.Invoke(new Action(() => currenti=(TextBox)game.FindName(i.getName())));
                                    Application.Current.Dispatcher.Invoke(new Action(() => currenti.IsEnabled = false));
                                }
                                else
                                {
                                    TextBox currenti = (TextBox)game.FindName(i.getName());
                                    currenti.IsEnabled = false;
                                }
                             }
                        }

                        if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() => this.stop.IsEnabled = false));
                            //if (isLeader)
                                Application.Current.Dispatcher.Invoke(new Action(() => this.newround.IsEnabled = true));
                        }
                        else
                        {
                            this.stop.IsEnabled = false;
                            //if (isLeader)
                                this.newround.IsEnabled = true;
                        }
                        
                        
                    }
                    if (commande.Equals("Enable"))
                    {
                        if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                        {
                            TextBlock currenti = null;
                            Application.Current.Dispatcher.Invoke(new Action(() => currenti = (TextBlock)game.FindName("letter" + numberound)));
                            Application.Current.Dispatcher.Invoke(new Action(() => currenti.Text = option));
                        }
                        else
                        {
                            TextBlock currenti = (TextBlock)game.FindName("letter" + numberound);
                            currenti.Text = option;
                        }

                        foreach (Item i in GameItems)
                        {
                            if (i.line.Equals(numberound))
                            {
                                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                                {
                                    TextBox currenti = null;
                                    Application.Current.Dispatcher.Invoke(new Action(() => currenti=(TextBox)game.FindName(i.getName())));
                                    Application.Current.Dispatcher.Invoke(new Action(() => currenti.IsEnabled = true));
                                }
                                else
                                {
                                    TextBox currenti = (TextBox)game.FindName(i.getName());
                                    currenti.IsEnabled = true;
                                }
                            }
                        }

                        if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() => this.stop.IsEnabled = true));
                            Application.Current.Dispatcher.Invoke(new Action(() => this.newround.IsEnabled = false));
                        }
                        else
                        {
                            this.stop.IsEnabled = true;
                            this.newround.IsEnabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
            }

            public void UpdateResult(string result)
            {
                //user;score;mot@mot2@...|user...
                string screenresult = null;
                string username = null;
                string score = null;
                string words = null;
                //"\r\n"
                foreach (string useresult in result.Split('|'))
                {
                    if (useresult != "")
                    {
                        username = useresult.Split(';')[0];
                        score = useresult.Split(';')[1];
                        words = useresult.Split(';')[useresult.Split(';').Length-1];

                        screenresult += username + " (" + score + ")  :";

                        foreach (string word in words.Split('@'))
                        {
                            screenresult += "  |  " + word;
                        }

                        screenresult += "\r\n";
                    }
                }
                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => this.playerscores.Text = ""));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.playerscores.Text = screenresult));
                }
                else
                {
                    this.playerscores.Text = "";
                    this.playerscores.Text = screenresult;
                }
            }

            public void EndGameInterface(string result)
            {
                //user;score;mot@mot2@...|user...
                string screenresult = null;
                string username = null;
                string score = null;
                //"\r\n"
                foreach (string useresult in result.Split('|'))
                {
                    if (useresult != "")
                    {
                        username = useresult.Split(';')[0];
                        score = useresult.Split(';')[1];

                        screenresult += username + " (" + score + ")";


                        screenresult += "\r\n";
                    }
                }
                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => this.playerscores.Text = screenresult));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.newround.IsEnabled = false));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c1.Text = ""));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c2.Text = ""));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c3.Text = ""));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c4.Text = ""));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c5.Text = ""));

                    Application.Current.Dispatcher.Invoke(new Action(() => this.letter1.Text = ""));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.letter2.Text = ""));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.letter3.Text = ""));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.letter4.Text = ""));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.letter5.Text = ""));

                }
                else
                {
                    this.playerscores.Text = screenresult;
                    this.newround.IsEnabled = false;
                    this.letter1.Text = "";
                    this.letter2.Text = "";
                    this.letter3.Text = "";
                    this.letter4.Text = "";
                    this.letter5.Text = "";
                }

                foreach (Item i in GameItems)
                {
                    if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                    {
                        TextBox currenti = null;
                        Application.Current.Dispatcher.Invoke(new Action(() => currenti = (TextBox)game.FindName(i.getName())));
                        Application.Current.Dispatcher.Invoke(new Action(() => currenti.IsEnabled = false));
                        Application.Current.Dispatcher.Invoke(new Action(() => currenti.Text = ""));
                    }
                    else
                    {
                        TextBox currenti = (TextBox)game.FindName(i.getName());
                        currenti.IsEnabled = false;
                        currenti.Text = "";
                    }
                }

                EnableCategories();

            }

            public void DisableCategories()
            {
                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c1.IsEnabled = false));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c2.IsEnabled = false));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c3.IsEnabled = false));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c4.IsEnabled = false));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c5.IsEnabled = false));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.start.IsEnabled = false));
                }
                else
                {
                    this.c1.IsEnabled = false;
                    this.c2.IsEnabled = false;
                    this.c3.IsEnabled = false;
                    this.c4.IsEnabled = false;
                    this.c5.IsEnabled = false;
                    this.start.IsEnabled = false;
                }
            }

            public void EnableCategories()
            {
                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c1.IsEnabled = true));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c2.IsEnabled = true));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c3.IsEnabled = true));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c4.IsEnabled = true));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c5.IsEnabled = true));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.start.IsEnabled = true));
                }
                else
                {
                    this.c1.IsEnabled = true;
                    this.c2.IsEnabled = true;
                    this.c3.IsEnabled = true;
                    this.c4.IsEnabled = true;
                    this.c5.IsEnabled = true;
                    this.start.IsEnabled = true;
                }
            }

            public void Log(string message){

                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                    Application.Current.Dispatcher.Invoke(new Action(() => this.error_msg.Text = message));
                else
                    this.error_msg.Text = message;
            }

            public void SetCategories(string _c1, string _c2, string _c3, string _c4, string _c5)
            {
                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c1.Text = _c1));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c2.Text = _c2));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c3.Text = _c3));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c4.Text = _c4));
                    Application.Current.Dispatcher.Invoke(new Action(() => this.c5.Text = _c5));
                }
                else
                {
                    this.c1.Text = _c1;
                    this.c2.Text = _c2;
                    this.c3.Text = _c3;
                    this.c4.Text = _c4;
                    this.c5.Text = _c5;
                }
            }

        #endregion

        #region Actions interfaces

            public void Connect(object sender, RoutedEventArgs e)
            {
                //Lancer une requête au server, si le server retourne 
                //Une fois connecté
                try
                {
                    client = new TcpClient(tb_ipadress.Text, int.Parse(tb_port.Text));
                    //client = new TcpClient("localhost", 9999);


                    //string content = "Player:New:" + tb_login.Text;
                    //callbacklogin = false;
                    //AsyncCallback GetMsgCallback = new AsyncCallback(GetMsgServer);
                    //(client.GetStream()).BeginRead(recByte, 0, 1024, GetMsgCallback, null);

                    Send("Player:New:" + tb_login.Text);
                    currentlogin = tb_login.Text;
                    //Client.Instance.CallBack();
                    //Listen();
                    AsyncCallback GetMsgCallback = new AsyncCallback(GetMsgServer);
                    NetworkStream stream = client.GetStream();
                    //AsyncCallback callBack = new AsyncCallback(Client.Instance.CallBack);
                    stream.BeginRead(recByte, 0, 1024, GetMsgCallback, this);
                
               

                    //while (callbacklogin == false){
                    //    //on attend ... même si je sais pas comment
                    //}
                    //if (connected == true){
                        //string userlogin = tb_login.Text.ToString();
                        //new CurrentPlayer(userlogin);
                        error_msg.Text = "Connecté !";
                        error_msg.Foreground = new SolidColorBrush(Colors.Green);
                        SetLogInterface();
                    //}
                 }
                 catch (Exception ex){
                     error_msg.Text = ex.Message;
                     error_msg.Foreground = new SolidColorBrush(Colors.Red);
                 }
            }

            /// <summary>
            /// Méthode déclenchée lors du clic d'un utilisateur sur le bouton de lancement d'une partie
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Start(object sender, RoutedEventArgs e)
            {
                if (c1.Text != "" & c2.Text != "" & c3.Text != "" & c4.Text != "" & c5.Text != "")
                    Send("Game:Start:"+c1.Text+"|"+c2.Text+"|"+c3.Text+"|"+c4.Text+"|"+c5.Text);
                else
                {
                    error_msg.Text = "Veuillez saisir toutes les catégories avant de lancer le jeu";
                    error_msg.Foreground = new SolidColorBrush(Colors.Red);
                }
            }

            private void Stop(object sender, RoutedEventArgs e)
            {
                Send("Game:Stop");
            }

            private void NewRound(object sender, RoutedEventArgs e)
            {
                Send("Game:NewRound");
            }

            private void Disconnect(object sender, RoutedEventArgs e)
            {
                Disconnect();
            }

            /// <summary>
            /// Fonction d'envoie d'un message
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void tb_message_KeyDown(object sender, KeyEventArgs e){
                if (e.Key == Key.Enter){
                    Send("Player:NewMessage:" + tb_message.Text.Replace(":",""));
                    tb_message.Text = "";
                }
               
            }

        #endregion

        #region Communication Serveur

            /// <summary>
            /// Methode pour envoyer un message au serveur
            /// </summary>
            /// <param name="data"></param>
            public void Send(string data){
                try{
                    if (client != null){
                        //Get a StreamWriter 
                        System.IO.StreamWriter writer = null;
                        writer = new System.IO.StreamWriter(client.GetStream());
                        writer.WriteLine(data);
                        //Flush the stream
                        writer.Flush();
                    }
                }
                catch (Exception e){
                    error_msg.Text = e.Message;
                    error_msg.Foreground = new SolidColorBrush(Colors.Red);
                    return;
                }
            }

            /// <summary>
            /// Methode pour recevoir un message du serveur
            /// </summary>
            /// <param name="ar"></param>
            public void GetMsgServer(IAsyncResult ar)
            {
            
                try
                {
                    int byteCount;
                    //Get the number of Bytes received
                    //Re putin d'erreur de merde avec .EndRead(ar) ahhhhhhhhhhhhhhhhh!!!!!!
                    NetworkStream mystream = client.GetStream();
                    //BinaryReader br = new BinaryReader(client.GetStream());
                
                    //Gére la fin de la lecture asynchrone
                    byteCount = mystream.EndRead(ar);
                    //byteCount = br.ReadByte();
                    //BinaryWriter bw = new BinaryWriter(new NetworkStream(hostClient.Socket, false));

                    //If bytes received is less than 1 it means
                    //the server has disconnected
                    if (byteCount < 1)
                    {
                        //Close the socket
                        Disconnect();
                        //error_msg.Text = "Déconnecté";
                        //error_msg.Foreground = new SolidColorBrush(Colors.Red);
                        return;
                    }
                    //Send the Server message for parsing
                    BuildText(recByte, 0, byteCount);
                    //Unless its the first time start Asynchronous Read
                    //Again
                    if (!firstTime)
                    {
                        AsyncCallback GetMsgCallback = new AsyncCallback(GetMsgServer);
                        (this.client.GetStream()).BeginRead(recByte, 0, 1024, GetMsgCallback, this);
                    }
                }
                catch (Exception ed){
                    Disconnect();      
                }
            }
  
            /// <summary>
            /// Methode pour récéptionner les données recues
            /// </summary>
            /// <param name="dataByte"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            public void BuildText(byte[] dataByte, int offset, int count){

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
                //Check if this is the first message received
                if (firstTime)
                {
                    //Split the string received at the occurance of '@'
                    //If the Server sent 'sorry' that means there was some error
                    //so we just disconnect the client
                
                    TextAnalyse(myBuilder.ToString());

                        //Store the Client Guid 
                        //this.userID = tempString[0];
                        //Loop through array of UserNames
                        
                        //Reset the flag
                        firstTime = false;
                        //Start the listening process again 
                        AsyncCallback GetMsgCallback = new AsyncCallback(GetMsgServer);
                        (client.GetStream()).BeginRead(recByte, 0, 1024, GetMsgCallback, this);
                

                }
                else
                {
                    //Generally all other messages get passed here
                    //Check if the Message starts with the ClientID
                    //In which case we come to know that its a Server Command
                    //if (myBuilder.ToString().IndexOf(this.userID) >= 0)

                    TextAnalyse(myBuilder.ToString());
                }
                //Empty the StringBuilder
                myBuilder = new System.Text.StringBuilder();
            }

            /// <summary>
            /// Methode pour analyser les données en provenance du serveur
            /// </summary>
            /// <param name="data"></param>
            public void TextAnalyse(string data)
            {
                if (lastlog!=null && lastlog!="")
                    if (data.StartsWith(lastlog))
                    {
                        data = data.Replace(lastlog, null);
                    }
                lastlog = data;
                Log("Message recu : "+data);
                #region Player
                if (data.Split(':')[0] == "Player")
                {
                    if (data.Split(':')[1] == "All")
                    {
                        ConnectedClient.Clear();
                        string[] allplayers = data.Split(':')[2].Split('|');
                        foreach (string player in allplayers)
                        {
                            if (!player.Equals(""))
                            {
                                Player p = new Player(player);
                                ConnectedClient.Add(p);
                            }
                        }
                        UpdateListClients();
                    }
                    if (data.Split(':')[1] == "New")
                    {
                        string newplayer = data.Split(':')[2];
                        if (!newplayer.Equals(currentlogin))
                        {
                            ConnectedClient.Add(new Player(newplayer));
                            UpdateListClients();
                        }
                    }
                    if (data.Split(':')[1] == "Del")
                    {
                        string delplayer = data.Split(':')[2];
                        foreach (Player p in ConnectedClient)
                        {
                            if(p.GetName().Equals(delplayer))
                                ConnectedClient.Remove(p);
                        }
                        UpdateListClients();
                    }
                    //Attention, les logs restent en cache dans le string data, vider réguierement le cache si possible
                    if (data.Split(':')[1] == "NewMessage")
                    {
                        string message = data.Split(':')[3];
                        UpdateMessages(data.Split(':')[2], message);
                    }
                   
                }
                #endregion

                #region Game
                if (data.Split(':')[0] == "Game")
                {
                    if (data.Split(':')[1].StartsWith("Start"))
                    { 
                        //ajoute le leader
                        string leadername = data.Split(':')[2];
                        if (leadername.Equals(currentlogin))
                            isLeader = true;
                        else
                            isLeader = false;

                        Log("Nouvelle partie commencée, leader : "+leadername + " isLeader : "+isLeader);

                        string categories = data.Split(':')[3];

                        string c1 = categories.Split('|')[0];
                        string c2 = categories.Split('|')[1];
                        string c3 = categories.Split('|')[2];
                        string c4 = categories.Split('|')[3];
                        string c5 = categories.Split('|')[4];

                        SetCategories(c1, c2, c3, c4, c5);
                        
                    }

                    if (data.Split(':')[1] == "NewRound")
                    {
                        int round = int.Parse(data.Split(':')[2]);
                        string letter = data.Split(':')[3];
                        
                        DisableCategories();
                        UpdateGame("Enable", round, letter);

                        currentround = round;
                    }

                    if (data.Split(':')[1] == "Stop")
                    {
                        UpdateGame("Disable", currentround, null);
                        string result = null;
                        foreach(Item i in GameItems)
                        {
                            if (i.line.Equals(currentround))
                            {
                                if (!object.ReferenceEquals(System.Windows.Threading.Dispatcher.CurrentDispatcher, Application.Current.Dispatcher))
                                {   
                                    TextBox currenti = null;
                                    Application.Current.Dispatcher.Invoke(new Action(() => currenti = (TextBox)game.FindName(i.getName())));
                                    Application.Current.Dispatcher.Invoke(new Action(() => result += currenti.Text+"|"));
                                }
                                else
                                {
                                    TextBox currenti = (TextBox)game.FindName(i.getName());
                                    result += currenti.Text + "|";
                                }
                            }

                        }
                        Send("Game:RoundResult:"+result);
                    }

                    if (data.Split(':')[1] == "RoundResult")
                    {
                        UpdateResult(data.Split(':')[2]);
                    }

                    if (data.Split(':')[1] == "End")
                    {
                        EndGameInterface(data.Split(':')[2]);
                    }
                    
                }
                
                #endregion
            }

            /// <summary>
            /// Methode de déconnexion
            /// </summary>
            private void Disconnect(){
                try
                {
                    if (client != null)
                    {
                        client.Close();
                        Log("Déconnecté");
                        SetUnlogInterface();
                        client = null;
                    }
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }


            }

        #endregion

            

            

            

            

        
    }

}
