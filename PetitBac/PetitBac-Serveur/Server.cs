using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitBac_Serveur
{
    public class StateObject
    {
        public Socket worksocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        //public static ManualResetEvent allDone = ManualResetEvent(false);

        public AsynchronousSocketListener() {
        }

        public static void StartListening()
        {
            byte[] bytes = new Byte[1024];

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipadresse = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipadresse, 11000);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    //allDone.Reset();

                    Console.WriteLine("Waiting for a connection ...");
                    //listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    //allDone.WaitOne();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();           
        }

        public static void AcceptedCallback(IAsyncResult ar)
        {
            //Get the socket that handles the client request

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            //Read data from the client socket

            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                //There might be more data, so store the data received so far

                //state.sb.Append(Encoding.Unicode.GetString(state.buffer,0,bytes

            }


        }
    }
}
