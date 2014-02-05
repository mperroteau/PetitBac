using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PetitBac.Pages
{
    /// <summary>
    /// Interaction logic for GameCreation.xaml
    /// </summary>
    public partial class GameCreation : UserControl
    {
        //private NetClient client = new NetClient();

        public GameCreation()
        {
            InitializeComponent();
        }

        private void SendGame(object sender, RoutedEventArgs e)
        {
            //SendText(sendBox.Text);

            //utiliser un singleton pour le client
            //if (Client.Instance.GetClient().State != SocketState.Connected)
            //{
            //    //this.Log("Send Cancelled");
            //    return;
            //}

            //byte[] name = Encoding.ASCII.GetBytes(GameName.Text);//Envoyer sous une forme particuliere fin que le serveur puisse reconnaitre les types d'éléments
            string sendname = GameName.Text;
            if (sendname.Length > 1)
            {
                Client.Instance.Send("Game:New:"+sendname);
                NavigationCommands.GoToPage.Execute("Pages/Games.xaml", null);
            }
        }

        
    }
}
