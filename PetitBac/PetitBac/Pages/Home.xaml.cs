using PetitBac.Classes;
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


namespace PetitBac.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            string username = "utilisateur";
            //if (NavigationContext.QueryString.TryGetValue("parameter", out username))   //Ajouter la dll
            //{
            //    this.HelloLog.Text = "Bonjour " + username + " !";
            //}
            this.HelloLog.Text = "Bonjour " + username + " !";

            
            
        }
    }
}
