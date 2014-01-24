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


namespace PetitBac.Pages
{
    /// <summary>
    /// Interaction logic for CurrentGame.xaml
    /// </summary>
    public partial class CurrentGame : UserControl
    {
        public CurrentGame()
        {
            InitializeComponent();
            //créer un tableau de texte box
            TextBox test = new TextBox();
            test.Text = "coucou";

        }

       

    }
}
