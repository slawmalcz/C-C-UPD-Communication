using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestV3
{
    class Program
    {
        static void Main(string[] args)
        {
            String choose = "";
            Console.WriteLine("1.Client");
            Console.WriteLine("2.Server");
            choose = Console.ReadLine();
            if (choose == "1")
            {
                new Client().Main();
            }
            else
            {
                new Server().Main();
            }
            Console.ReadLine();
        }
    }
}
