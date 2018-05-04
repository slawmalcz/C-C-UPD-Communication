using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CompNetworkProject
{
    class SendProtocol
    {
        private string pathToFile;
        private int numOfPackets;
        private string nameAndExtension;
        private Dictionary<int, byte[]> packages;

        public SendProtocol()
        {

            Console.WriteLine("Type path to file you want to send:");
            pathToFile = Console.ReadLine();
            nameAndExtension = Path.GetFileName(pathToFile) + '.' + Path.GetExtension(pathToFile);
            FileProcesor procesedFile = new FileProcesor(pathToFile);
            packages = procesedFile.GetPackages();
            numOfPackets = procesedFile.NumOfPacket;
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

        public void Main()
        {
            byte[] data = new byte[1024];
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
                 * Sending data information:
                 *  - name and extension
                 *  - number of package
                 * and whaiting for confirmation.
                 */
                case 2:
                    data = Encoding.ASCII.GetBytes(nameAndExtension);
                    newsock.Send(data, data.Length, ipep);
                    data = Encoding.ASCII.GetBytes(numOfPackets.ToString());
                    newsock.Send(data, data.Length, ipep);
                    data = newsock.Receive(ref ipep);
                    if (Encoding.ASCII.GetString(data) == "All good") stateMachine = 3;
                    break;

                /*
                 * Sending data protocol
                 */
                case 3:
                    int NumberSequencer;
                    for(int i=0;i<packages.Count;i++)
                    {
                        NumberSequencer = i;
                        data = Encoding.ASCII.GetBytes(NumberSequencer.ToString());
                        newsock.Send(data, data.Length, ipep);
                        data = packages[i];
                        newsock.Send(data, data.Length, ipep);
                        data = newsock.Receive(ref ipep);
                        if (Encoding.ASCII.GetString(data) != "Recived package: "+NumberSequencer.ToString()) continue;
                    }
                    newsock.Send(data, 0, ipep);
                    break;
            }
        }
    }

}
