using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestV3;

class Client
    {
    public const int connectionport = 25565;
    public const int byteBuffer = 1024;
    public void Main()
    {
        try
        {
            TcpClient tcpclnt = new TcpClient();
            FileProcesor fileProcesor = new FileProcesor();
            String str;
            Stream stm;
            byte[] ba, bb;
            int k;

            ASCIIEncoding asen = new ASCIIEncoding();

            // Connecting witch server and sending clientIP
            String serverIP;
            Console.WriteLine("Podaj adres IPServera:");
            serverIP = Console.ReadLine();

            Console.WriteLine("Connecting.....");
            tcpclnt.Connect(IPAddress.Parse(serverIP), connectionport);
            // use the ipaddress as in the server program

            Console.WriteLine("Connected");

            //This part is only walid when connectiong external port
            Console.WriteLine("Czy połączenie jest pośredniczone przez server? (Y/N)");
            if (Console.ReadLine() == "Y")
            {
                Console.WriteLine("PodajIP clienta do transferu : ");

                str = Console.ReadLine();
                stm = tcpclnt.GetStream();

                asen = new ASCIIEncoding();
                ba = asen.GetBytes(str);
                Console.WriteLine("Transmitting.....");

                stm.Write(ba, 0, ba.Length);

                bb = new byte[byteBuffer];
                k = stm.Read(bb, 0, 1024);

                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(bb[i]));
            }

            //Sending name and extension
            String filePath;
            Console.WriteLine("Podaj sciezke pliku do wyslania:");
            filePath = Console.ReadLine();
            fileProcesor = new FileProcesor(filePath);

            Console.WriteLine("Sending name and extension...");

            str = fileProcesor.getNameAndExtension();
            stm = tcpclnt.GetStream();
            Console.WriteLine(str);

            asen = new ASCIIEncoding();
            ba = asen.GetBytes(str);
            Console.WriteLine("Transmitting.....");

            stm.Write(ba, 0, ba.Length);

            bb = new byte[byteBuffer];
            k = stm.Read(bb, 0, byteBuffer);

            for (int i = 0; i < k; i++)
                Console.Write(Convert.ToChar(bb[i]));

            // Sending package number
            Console.WriteLine("Sending package number...");

            str = fileProcesor.numOfPacket.ToString();
            stm = tcpclnt.GetStream();
            Console.WriteLine(str);

            asen = new ASCIIEncoding();
            ba = asen.GetBytes(str);
            Console.WriteLine("Transmitting.....");

            stm.Write(ba, 0, ba.Length);

            bb = new byte[byteBuffer];
            k = stm.Read(bb, 0, byteBuffer);

            for (int i = 0; i < k; i++)
                Console.Write(Convert.ToChar(bb[i]));

            //Sending file
            var packages = fileProcesor.GetPackages();
            Console.WriteLine("Sending " + fileProcesor.numOfPacket + " packages...");
            foreach (var package in packages)
            {
                //Sending key ==>
                Console.WriteLine("Sending key value...");

                str = package.Key.ToString();
                stm = tcpclnt.GetStream();
                Console.WriteLine(str);

                asen = new ASCIIEncoding();
                ba = asen.GetBytes(str);
                Console.WriteLine("Transmitting.....");

                stm.Write(ba, 0, ba.Length);

                bb = new byte[byteBuffer];
                k = stm.Read(bb, 0, byteBuffer);

                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(bb[i]));
                // <== Sending key
                // Sending package ==>
                Console.WriteLine("Sending package " + package.Key);


                stm = tcpclnt.GetStream();

                ba = package.Value;
                Console.WriteLine(ba);
                Console.WriteLine("Transmitting.....");

                stm.Write(ba, 0, ba.Length);

                bb = new byte[byteBuffer];
                k = stm.Read(bb, 0, byteBuffer);

                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(bb[i]));
                // <== Sending package
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
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
