﻿using FirstFloor.ModernUI.Presentation;
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
    /// Interaction logic for Parties.xaml
    /// </summary>
    public partial class Parties : UserControl
    {
        private byte[] recByte = new byte[1024];

        public Parties()
        {
            InitializeComponent();
            //Faire une requête au serveur
            /*
             Requête au serveur qui retourne une liste de parties
             */

            getGames();
            try
            {
                foreach (Game g in Client.Instance.ClientGames)
                    this.AllGames.Items.Add(g.GetName());
                    //this.Games_view.Links.Add(new Link
                    //{
                    //    DisplayName = g.GetName(),
                    //    Source = new Uri("/Settings/InterfacciaGrafica.xaml", UriKind.Relative)
                    //});
            }
            catch (Exception e)
            {
                string ex = e.Message;
            }
            
            //Lister les parties avec le créateur de la partie, le joueurs, le créateur de la partie, les catégories, le nombre de tours
            //Temps maximal pour la partie
            //Comment repérer les parties ? 
            //Lancement d'un serveur ? 
        }

        public void getGames()
        {
            //retourne une série de données
            Client.Instance.Send("Game:GetAll");
            //Client.Instance.CallBack();
            AsyncCallback GetMsgCallback = new AsyncCallback(Client.Instance.GetMsgServer);
            //AsyncCallback callBack = new AsyncCallback(Client.Instance.CallBack);
           // Client.Instance.CallBack();
            (Client.Instance.GetClient().GetStream()).BeginRead(recByte, 0, 1024, GetMsgCallback, this);
            //(Client.Instance.GetClient().GetStream()).BeginRead(recByte, 0, 1024, callBack, this);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.AllGames.Items.Clear();
                foreach (Game g in Client.Instance.ClientGames)
                    this.AllGames.Items.Add(g.GetName());
                
                //this.Games_view.Links.Add(new Link
                //{
                //    DisplayName = g.GetName(),
                //    Source = new Uri("/Settings/InterfacciaGrafica.xaml", UriKind.Relative)
                //});
            }
            catch (Exception ex)
            {
                string ex2 = ex.Message;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Client.Instance.Send("Game:Join:"+this.AllGames.SelectedIndex.ToString());
        }
    }
}
