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
            String choose = "";
            Console.WriteLine("1.Recive");
            Console.WriteLine("2.Send");
            choose = Console.ReadLine();
            if (choose == "1")
            {
                new ReciveProtocol().Main();
            }
            else
            {
                new SendProtocol().Main();
            }
            Console.ReadLine();
        }
    }
}
