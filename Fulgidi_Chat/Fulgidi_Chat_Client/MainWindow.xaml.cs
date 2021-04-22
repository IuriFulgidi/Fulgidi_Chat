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
            string ip = txtIP.Text;
            string porta = txtPorta.Text;
            string nick = txtNick.Text;

            //controllo dei dati
            if (ip == "" || porta == "" || nick == "")
            {
                MessageBox.Show("Fornire tutti i dati");
                return;
            }

            //creo il client
            client = new AsyncSocketClient();

            if(!client.SetServerIPAddress(ip))
            {
                MessageBox.Show("indirizzo ip non valido");
                return;
            }
            if (!client.SetServerPort(porta))
            {
                MessageBox.Show("numero di porta non valido");
                return;
            }

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
