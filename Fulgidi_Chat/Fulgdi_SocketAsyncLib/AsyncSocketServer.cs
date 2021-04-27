using SocketAsync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fulgidi_SocketAsync
{
    public class AsyncSocketServer
    {
        IPAddress mIP;
        int mPort;
        TcpListener mServer;
        bool continua;
        List<ChatClient> mClients;
        DateTime dataInizio;

        public AsyncSocketServer()
        {
            mClients = new List<ChatClient>();
        }

        // Mette in ascolto il server
        public async void InizioAscolto()
        {
            mIP = IPAddress.Any;
            mPort = 23000;

            Console.WriteLine($"Avvio il server. IP: {mIP} - Porta: {mPort}");
            //creare l'oggetto server
            mServer = new TcpListener(mIP, mPort);

            //avviare il server
            mServer.Start();
            continua = true;
            //mi metto in ascolto
            while (continua)
            {
                //aggiungo il client
                TcpClient client = await mServer.AcceptTcpClientAsync();
                RegistraClient(client);
                RiceviMessaggi(client);
            }
        }

        public async void RegistraClient(TcpClient client)
        {
            //si prende la data di inizio della chat
            if (mClients.Count() == 0)
                dataInizio = DateTime.Now;

            //ricezione nick del client
            NetworkStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);
                char[] buff = new char[512];

                //ricezione effettiva
                int nBytes = await reader.ReadAsync(buff, 0, buff.Length);
                string nick = new string(buff, 0, nBytes);

                //creazione del client e aggiunta alla lista
                ChatClient chatClient = new ChatClient(client,nick);
                mClients.Add(chatClient);

                //invio data al client
                SendToOne(chatClient.Client, dataInizio.ToShortDateString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            Console.WriteLine($"Client connessi: {mClients.Count()}, Client appena connesso:{ client.Client.RemoteEndPoint}");
            
        }

        private async void RiceviMessaggi(TcpClient client)
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);
                char[] buff = new char[512];

                //ricezione effettiva
                while (continua)
                {
                    Console.WriteLine("Pronto ad ascoltare...");
                    int nBytes = await reader.ReadAsync(buff, 0, buff.Length);
                    if (nBytes == 0)
                    {
                        RemoveClient(client);
                        Console.WriteLine("Client disconnesso.");
                        break;
                    }
                    string recvMessage = new string(buff,0,nBytes).ToLower();

                    ChatClient cc = mClients.Where(e => e.Client == client).FirstOrDefault();
                    string risp = $"({DateTime.Now.Hour}:{DateTime.Now.Minute}) {cc.Nick}: {recvMessage}";
                    SendToAll(risp);
                    Console.WriteLine($"Returned bytes: {nBytes}. Messaggio: {recvMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RemoveClient(TcpClient client)
        {
            //Con l'uso del LINQ
            //ChatClient cc = mClients.Where(e => e.Client == client).FirstOrDefault();

            foreach (ChatClient c in mClients)
            {
                if(c.Client==client)
                    mClients.Remove(c);
            }
        }

        public void SendToAll(string messaggio)
        {
            try
            {
                if (string.IsNullOrEmpty(messaggio))
                    return;

                byte[] buff = Encoding.ASCII.GetBytes(messaggio);

                foreach (ChatClient client in mClients)
                    client.Client.GetStream().WriteAsync(buff, 0, buff.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore:" + ex.Message);
            }
        }
        public void SendToOne(TcpClient client, string messaggio)
        {
            try
            {
                if (string.IsNullOrEmpty(messaggio))
                    return;

                byte[] buff = Encoding.ASCII.GetBytes(messaggio);
                client.GetStream().WriteAsync(buff, 0, buff.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore:" + ex.Message);
            }
        }
        public void CloseConnection()
        {
            try
            {
                foreach (ChatClient client in mClients)
                {
                    client.Client.Close();
                    RemoveClient(client.Client);
                }
                mServer.Stop();
                mServer = null;
                continua = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore:" + ex.Message);
            }
        }
    }
}
