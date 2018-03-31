using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyClasses;

namespace RechercheFiles
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {            
            List<RecivedFiles> l = new List<RecivedFiles>();
            MyClasses.BiBFilesXML bib = new MyClasses.BiBFilesXML();
            l = bib.rechercheFichiers(listeFilesTextBox.Text);
            listedefichiers.Items.Clear();
            foreach (MyClasses.RecivedFiles rf in l)
                listedefichiers.Items.Add(rf);
        }
    }
}
