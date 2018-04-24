using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CompNetworkProject
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] data = new byte[512];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
            UdpClient newsock = new UdpClient();
            int stateMachine = 0;

            switch (stateMachine)
            {
                /*
                 * Sending to server getter IP and waiting for accepting connection
                 */
                case 0:
                    newsock = InitializeSocket(out ipep);
                    String ipGeter;
                    ipGeter = Console.ReadLine();
                    data = Encoding.ASCII.GetBytes(ipGeter);
                    newsock.Send(data, data.Length, ipep);
                    data = newsock.Receive(ref ipep);
                    if (Encoding.ASCII.GetString(data) == "Accepted connection") stateMachine = 1;
                    break;
                /*
                 * Waitnig for server connection validate signal
                 */
                case 1:
                    if (Encoding.ASCII.GetString(data) == "Valid connection")
                    {
                        stateMachine = 2;
                    }
                    else
                    {
                        stateMachine = 0;
                    }
                    break;
                /*
                 * Sending data protocol
                 */
                case 2:
                    Boolean isDataEnd = false;
                    while (true)
                    {
                        /*
                         * here pass what you whant to send to data variable
                         * and if you have nothing more to send, set isDataEnd to true;
                         */
                        if (isDataEnd)
                        {
                            newsock.Send(data, 0, ipep);
                            break;
                        }
                        else
                        {
                            newsock.Send(data, data.Length, ipep);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Static functions for determine witch whom we want to trade data
        /// </summary>
        /// <returns>UdpClient - server UdpClient to send data and control exchange</returns>
        private static UdpClient InitializeSocket(out IPEndPoint ipep)
        {
            UdpClient newsock;
            String ipServer, portServer;
            Console.WriteLine("Podaj adres IP servera:");
            ipServer = Console.ReadLine();
            Console.WriteLine("Podaj adres portu servera:");
            portServer = Console.ReadLine();
            ipep = new IPEndPoint(IPAddress.Parse(ipServer), Int32.Parse(portServer));
            newsock = new UdpClient(ipep);
            return newsock;
        }
    }
}
