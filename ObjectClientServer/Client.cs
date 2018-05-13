using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ObjectClientServer
{
    class Client
    {
        private TextBlock mainConsole;

        public Client(TextBlock mainConsole)
        {
            this.mainConsole = mainConsole;
        }

        public void Main()
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                mainConsole.Text += "Connecting...../n";

                tcpclnt.Connect(HostNameToIP("term2"), 8001);
                // use the ipaddress as in the server program

                mainConsole.Text += "Connected /n";
                mainConsole.Text += "Enter the string to be transmitted : /n";

                String str = "Test message";
                Stream stm = tcpclnt.GetStream();

                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(str);
                mainConsole.Text += "Transmitting...../n";

                stm.Write(ba, 0, ba.Length);

                byte[] bb = new byte[100];
                int k = stm.Read(bb, 0, 100);

                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(bb[i]));

                tcpclnt.Close();
            }

            catch (Exception e)
            {
                mainConsole.Text += "Error..... " + e.StackTrace+"/n";
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
