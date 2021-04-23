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
using System.Windows.Shapes;

namespace Fulgidi_Chat_Client
{
    /// <summary>
    /// Logica di interazione per Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        AsyncSocketClient client;
        public Chat(AsyncSocketClient c)
        {
            InitializeComponent();
            this.client = c;
            c.OnNewMessage += client_OnNewMessage;
        }

        private void client_OnNewMessage(object sender, EventArgs e)
        {
            lstChat.ItemsSource = client.Msgs;
            lstChat.Items.Refresh();
        }

        private void BtnInvia_Click(object sender, RoutedEventArgs e)
        {
            client.SendMessage(txtMsg.Text);
        }

        private void txtMsg_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) 
                return;

            client.SendMessage(txtMsg.Text);
        }
    }
}
