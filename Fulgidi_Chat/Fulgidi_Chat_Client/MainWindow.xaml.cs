using SocketAsync;
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

namespace Fulgidi_Chat_Client
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AsyncSocketClient client;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //creo il client
            client = new AsyncSocketClient();
            client.SetServerIPAddress(txtIP.Text);
            client.SetServerPort(txtPorta.Text);

            //connessione al server
            client.ConnectToServer();

            //invio messaggio con credenziali
            client.SendMessage(txtNick.Text);

            //apertura finestra di chat
            Window chat = new Chat(client);
            chat.Show();
            this.Close();
        }
    }
}
