using System;
using System.Collections.Generic;
using System.Linq;
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
using FirstFloor.ModernUI.Windows.Navigation;
using PetitBac.Classes;
using PetitBac.NetSocket;

namespace PetitBac.Pages
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        //private NetClient client = new NetClient();
        private byte[] recByte = new byte[1024];

        public Login()
        {
            InitializeComponent();
            //this.SafeCall = new Safe(Log_Local);
        }
        

        private void UserConnection(object sender, RoutedEventArgs e)
        {
            try
            {

                //System.Net.IPEndPoint end = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(IPServer.Text), 9999);
                //Client.Instance.GetClient().Connect(end);

                //Vérifier si le nom d'utilisateur existe, envoyer un Check avec le login, le serveur doit renvoyer true ou false
                if (UserLogin.Text != null)
                {
                    if (!PlayerExist())
                    {
                        //Envoyer un Send new User
                        //S'est connecté mais n'est pas officielement un joueur
                        //Peut envoyer une request en attendant la réponse ?
                        string content = "Player:New:" + UserLogin.Text;
                        Client.Instance.Send(content);
                        string userlogin = UserLogin.Text.ToString();
                        new CurrentPlayer(userlogin);

                        NavigationCommands.GoToPage.Execute("Pages/Home.xaml?parameter=\"" + userlogin + "\"", null);

                    }
                }
            }
            catch (Exception ex)
            {
                LogError.Text=ex.Message.ToString();
            }
        }

        bool PlayerExist()
        {
            try
            {
                //Start Reading
                //AsyncCallback GetMsgCallback = new AsyncCallback(client.GetMsgServer);
                //(Client.Instance.GetStream()).BeginRead(recByte, 0, 1024, GetMsgCallback, null);
                /*
                Appel la liste de joueurs et créer une liste
                */
                List<Player> listPlayer = new List<Player>();

                Player player1 = new Player("test");
                listPlayer.Add(player1);
                Player player2 = new Player("pierre");
                listPlayer.Add(player2);
                Player Player3 = new Player("marie");
                listPlayer.Add(Player3);

                foreach (Player p in listPlayer)
                {
                    if (p.GetName().Equals(UserLogin.Text.ToString()))
                    {
                        LogError.Text = "Pseudo déjà pris";
                        return true;
                    }
                    
                }
                return false;
            }
            catch (Exception e)
            {
                LogError.Text = e.Message.ToString();
                return true;
            }

        }

    }
}
