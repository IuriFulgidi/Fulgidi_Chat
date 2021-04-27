using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketAsync
{
    public class AsyncSocketClient
    {
        IPAddress mServerIpAddress;
        int mServerPort;
        TcpClient mClient;
        public List<string> Msgs = new List<string>();

        public event EventHandler OnNewMessage;
        protected virtual void OnNewMessageHandler(EventArgs e)
        {
            EventHandler handler = OnNewMessage;
            handler?.Invoke(this, e);
        }

        public IPAddress ServerIpAddress
        {
            get {return mServerIpAddress;}
        }

        public int ServerPort
        {
            get{ return mServerPort;}
        }

        public bool SetServerIPAddress(string str_ipaddr)
        {
            IPAddress ipaddr = null;
            if (!IPAddress.TryParse(str_ipaddr, out ipaddr))
            {
                Console.WriteLine("Ip non valido"); 
                return false;
            }
            mServerIpAddress = ipaddr;
            return true;

        }

        public bool SetServerPort(string str_port)
        {
            int port = -1;
            if (!int.TryParse(str_port, out port))
            {
                Console.WriteLine("Porta non valida");
                return false;
            }
            if (port<0 || port >65535)
            {
                Console.WriteLine("La porta de essere tra 0 e 65535");
                return false;
            }
            mServerPort= port;
            return true;

        }

        public async Task ConnectToServer()
        {
            if(mClient == null)
                mClient = new TcpClient();

            try
            {
                await mClient.ConnectAsync(mServerIpAddress, mServerPort);
                Console.WriteLine($"Connesso correttamente a {mServerIpAddress} / {mServerPort}");
                ReceiveMessages();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private async Task ReceiveMessages()
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = mClient.GetStream();
                reader = new StreamReader(stream);
                char[] buff = new char[512];

                //ricezione effettiva
                while (true)
                {
                    //numero di bytes in ricezione
                    int nBytes = await reader.ReadAsync(buff, 0, buff.Length);

                    //in caso di disconnessione
                    if (nBytes == 0)
                    {
                        Msgs.Add("Disconnesso dal server");
                        EventArgs ev = new EventArgs();
                        OnNewMessageHandler(ev);
                        break;
                    }

                    //creazione stringa del messaggio
                    string recvMessage = new string(buff, 0, nBytes);

                    //si aggiunge il messaggio alla lista
                    Msgs.Add(recvMessage);

                    //evento per la gestione del messaggio
                    EventArgs e = new EventArgs();
                    OnNewMessageHandler(e);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendMessage(string msg)
        {
            try
            {
                if (string.IsNullOrEmpty(msg))
                    return;

                byte[] buff = Encoding.ASCII.GetBytes(msg);
                mClient.GetStream().WriteAsync(buff, 0, buff.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore:" + ex.Message);
            }
        }
        public AsyncSocketClient()
        {
            mServerIpAddress = null;
            mServerPort = -1;
            mClient = null;
        }
    }
}
