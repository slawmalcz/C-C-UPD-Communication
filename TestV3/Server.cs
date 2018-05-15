using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestV3;

class Server
    {
    public const int connectionport = 25565;
    public const string localIP = "150.254.79.212";
    public const int byteBuffer = 1024;

        public void Main()
        {
            try
            {
            // Setting client reciver IP
            IPAddress ipAd = IPAddress.Parse(GetLocalIPAddress());
            // use local m/c IP address, and 
            // use the same in the client

            TcpListener myList = new TcpListener(ipAd, connectionport);
                Console.WriteLine("Starting ...");
                
                myList.Start();

                Console.WriteLine("The ReciveClient is running at port "+connectionport.ToString()+"...");
                Console.WriteLine("The local End point is  :" +
                                  myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");

                Socket s = myList.AcceptSocket();
                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                // Reciving nameand Extension
                String nameAndExtension = "";
                byte[] b = new byte[byteBuffer];
                int k = s.Receive(b);
                Console.WriteLine("Recieved...");
                for (int i = 0; i < k; i++)
                {
                    Console.Write(Convert.ToChar(b[i]));
                    nameAndExtension += Convert.ToChar(b[i]);
                }

                ASCIIEncoding asen = new ASCIIEncoding();
                s.Send(asen.GetBytes("The string was recieved by the server."));
                Console.WriteLine("\nSent Acknowledgement");

                //Reciving num of packages
                String numPackageString = "";
                b = new byte[byteBuffer];
                k = s.Receive(b);
                Console.WriteLine("Recieved...");
                for (int i = 0; i < k; i++)
                {
                    Console.Write(Convert.ToChar(b[i]));
                    numPackageString += Convert.ToChar(b[i]);
                }

                asen = new ASCIIEncoding();
                s.Send(asen.GetBytes("The string was recieved by the server."));
                Console.WriteLine("\nSent Acknowledgement");

                //Creating instance of FileProcessor
                FileProcesor fileProcesor = new FileProcesor(nameAndExtension, Int32.Parse(numPackageString));
                
                //Main package recive loop
                for(int j=0; j < Int32.Parse(numPackageString); j++)
                {
                    //Reciving key
                    String key = "";
                    b = new byte[byteBuffer];
                    k = s.Receive(b);
                    Console.WriteLine("Recieved...");
                    for (int i = 0; i < k; i++)
                    {
                        Console.Write(Convert.ToChar(b[i]));
                        key += Convert.ToChar(b[i]);
                    }

                    asen = new ASCIIEncoding();
                    s.Send(asen.GetBytes("The string was recieved by the server."));
                    Console.WriteLine("\nSent Acknowledgement");

                    //Reciving package
                    b = new byte[byteBuffer];
                    k = s.Receive(b);
                    Console.WriteLine("Recieved...");
                    for (int i = 0; i < k; i++)
                    {
                        Console.Write(Convert.ToChar(b[i]));
                    }

                    asen = new ASCIIEncoding();
                    s.Send(asen.GetBytes("The string was recieved by the server."));
                    Console.WriteLine("\nSent Acknowledgement");

                    //Adding package to file processor
                    fileProcesor.insertPackage(Int32.Parse(key), b);
                }

                //Combining packages
                Console.WriteLine("All packages recived. Attempting to combine file...");
                fileProcesor.combineFile(Int32.Parse(numPackageString));

                /* clean up */
                s.Close();
                myList.Stop();
                Console.WriteLine("Procedure compleated");

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.Message + e.StackTrace);
            }
        }

    private static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
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
