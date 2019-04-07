using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CompNetworkProject
{
    class ReciveProtocol
    {
        FileProcesor fileProcesor;

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
            try
            {
                newsock = new UdpClient(ipep);
            }
            catch (Exception e)
            {
                newsock = new UdpClient();
                Console.WriteLine(e);
                throw new Exception("Cannot connect");
            }
            return newsock;
        }

        public void Main()
        {
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
            UdpClient newsock = new UdpClient();
            int stateMachine = 0;


            try
            {
                while (true)
                {
                    switch (stateMachine)
                    {
                        /*
                         * Initialize recive protocol.
                         */
                        case 0:
                            newsock = InitializeSocket(out ipep);
                            stateMachine = 1;

                            break;
                        /*
                         * Waait to validate connection
                         */
                        case 1:
                            data = newsock.Receive(ref ipep);
                            if (Encoding.ASCII.GetString(data) == "Validate connection")
                            {
                                data = Encoding.ASCII.GetBytes("Validate");
                                newsock.Send(data, data.Length, ipep);
                                stateMachine = 2;
                            }
                            else
                            {
                                Console.WriteLine("Bad UDP communications");
                            }
                            break;

                        /*
                         * Read data information:
                         *  - name and extension
                         *  - number of package
                         * and send confirmation
                         * Creating file procesor
                         */
                        case 2:
                            String nameAndExtension;
                            int numOfPackages;
                            data = newsock.Receive(ref ipep);
                            nameAndExtension = Encoding.ASCII.GetString(data);
                            data = Encoding.ASCII.GetBytes("OK");
                            newsock.Send(data, data.Length, ipep);
                            data = newsock.Receive(ref ipep);
                            numOfPackages = Int32.Parse(Encoding.ASCII.GetString(data));
                            data = Encoding.ASCII.GetBytes("OK");
                            newsock.Send(data, data.Length, ipep);

                            fileProcesor = new FileProcesor(nameAndExtension, numOfPackages);

                            stateMachine = 3;
                            break;

                        /*
                         * Sending data protocol
                         */
                        case 3:
                            int NumberSequencer;
                            for (int i = 0; i < fileProcesor.NumOfPacket; i++)
                            {
                                data = newsock.Receive(ref ipep);
                                NumberSequencer = Int32.Parse(Encoding.ASCII.GetString(data));
                                data = Encoding.ASCII.GetBytes("OK");
                                newsock.Send(data, data.Length, ipep);

                                data = newsock.Receive(ref ipep);
                                fileProcesor.insertPackage(NumberSequencer, data);
                                data = Encoding.ASCII.GetBytes("OK");
                                newsock.Send(data, data.Length, ipep);
                            }
                            data = newsock.Receive(ref ipep);
                            if (Encoding.ASCII.GetString(data) == "")
                            {
                                stateMachine = 4;
                            }
                            else
                            {
                                Console.WriteLine("Error");
                                stateMachine = 0;
                            }
                            break;

                        case 4:
                            fileProcesor.combineFile(fileProcesor.NumOfPacket);
                            stateMachine = 0;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong");
                Console.WriteLine(e.Message);
            }

        }

    }
}
