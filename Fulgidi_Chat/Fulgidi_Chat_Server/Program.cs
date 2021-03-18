using Fulgidi_SocketAsync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fulgidi_Chat_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //creazione del server
            AsyncSocketServer server = new AsyncSocketServer();
            server.InizioAscolto();  
        }
    }
}
