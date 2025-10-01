using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(SessionService)))
            {
                host.Open();
                Console.WriteLine("Connection open, press any button to close");
                Console.ReadKey();
                host.Close();
            }
            Console.WriteLine("Service closed.");
            Console.ReadKey();
        }
    }
}
