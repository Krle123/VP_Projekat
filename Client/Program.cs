using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Library;

using static Client.CSVLoader;
using System.Threading;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<ISessionService> channelFactory = new ChannelFactory<ISessionService>("SessionService");
            string csvPath = @"measures_v2.csv"; 
            string logPath = @"log.txt";

            int number = 0;
            do
            {

                var samples = LoadMotorSamples(csvPath, logPath, 100);

                Console.WriteLine($"Loaded {samples.Count} valid rows.");
                ISessionService proxy = channelFactory.CreateChannel();
                number = PrintMenu();
                if(number == 1)
                {
                    proxy.StartSession();
                    for (int i = 0; i < samples.Count; i++)
                    {
                        proxy.PushSample(samples[i]);
                        Thread.Sleep(500);
                    }
                    proxy.EndSession();
                }
            }
            while (number != 2);
        }

        public static int PrintMenu()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("Option 1: Send rows");
            Console.WriteLine("Option 2: Exit");
            Console.Write("Write the number of the action you would like to choose:");
            try
            {
                int number = Int32.Parse(Console.ReadLine());
                if (number >= 1 && number < 3)
                {
                    return number;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
