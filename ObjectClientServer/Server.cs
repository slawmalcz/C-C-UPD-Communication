using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ObjectClientServer
{
    class Server
    {
        private TextBlock mainConsole;

        public Server(TextBlock mainConsole)
        {
            this.mainConsole = mainConsole;
        }

        public void Main()
        {
            try
            {
                IPAddress ipAd = HostNameToIP("DESKTOP-0HHCC1N");
                // use local m/c IP address, and 
                // use the same in the client

                /* Initializes the Listener */
                TcpListener myList = new TcpListener(ipAd, 25565);
                mainConsole.Text += "Starting .../n";
                /* Start Listeneting at the specified port */
                myList.Start();

                mainConsole.Text += "The server is running at port 8001.../n";
                mainConsole.Text += "The local End point is  :" +
                                  myList.LocalEndpoint + "/n";
                mainConsole.Text += "Waiting for a connection...../n";

                Socket s = myList.AcceptSocket();
                mainConsole.Text += "Connection accepted from " + s.RemoteEndPoint + "/n";

                byte[] b = new byte[100];
                int k = s.Receive(b);
                mainConsole.Text += "Recieved.../n";
                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(b[i]));

                ASCIIEncoding asen = new ASCIIEncoding();
                s.Send(asen.GetBytes("The string was recieved by the server."));
                mainConsole.Text += "\nSent Acknowledgement/n";
                /* clean up */
                s.Close();
                myList.Stop();

            }
            catch (Exception e)
            {
                mainConsole.Text += "Error..... " + e.StackTrace + "/n";
            }
        }

        private IPAddress HostNameToIP(string host)
        {
            IPHostEntry hostEntry;

            hostEntry = Dns.GetHostEntry(host);

            //you might get more than one ip for a hostname since 
            //DNS supports more than one record

            if (hostEntry.AddressList.Length > 0)
            {
                var ip = hostEntry.AddressList[0];
                return ip;
            }
            else
            {
                throw new Exception("Cannot convert hostname to IP");
            }
        }
    }
}
