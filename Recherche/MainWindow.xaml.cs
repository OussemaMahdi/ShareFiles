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
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Recherche
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
            BiBFilesXML bib = new BiBFilesXML();
            l = bib.rechercheFichiers(listeFilesTextBox.Text);
            listedefichiers.Items.Clear();
            foreach (RecivedFiles rf in l)
                listedefichiers.Items.Add(rf);
        }
        private void listedefichiers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listedefichiers.SelectedItem != null)
            {
                RecivedFiles item = (RecivedFiles)listedefichiers.SelectedItem;
                BiBFilesXML bib=new BiBFilesXML();
                if(!bib.getFilesToDownload().Contains(item))
                {
                    if (System.IO.File.Exists(item.getPathOfSave()))
                    {
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.StartInfo.FileName = item.getPathOfSave();
                        proc.StartInfo.UseShellExecute = true;
                        proc.Start();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("this file is deleted or changed path", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                        bib.deleteFile(item);
                    }
                }
                else
                {
                MessageBox.Show("this file is not downloaded","Alert",MessageBoxButton.OK,MessageBoxImage.Information);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(listeFilesTextBox);
        }

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private const int HOTKEY_ID = 9005;
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }
        protected override void OnClosed(EventArgs e)
        {
            try
            {
                _source.RemoveHook(HwndHook);
                _source = null;
                UnregisterHotKey();
                base.OnClosed(e);
            }
            catch { base.OnClosed(e); }
        }
        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint VK_A = 0x1B;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, 0x0000, VK_A))
            {
                // handle error
            }
        }
        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;                        
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            this.Close();
        }
    }
}
