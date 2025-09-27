using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

using static Client.CSVLoader;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            string csvPath = @"";       //TODO 
            string logPath = @"";

            var samples = LoadMotorSamples(csvPath, logPath, 100);

            Console.WriteLine($"Učitano {samples.Count} validnih redova.");
        }
    }
}
