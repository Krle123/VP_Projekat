using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Client
{
    public class CSVLoader
    {
        public static List<MotorSample> LoadMotorSamples(string csvPath, string logPath, int maxRows)
        {
            List<MotorSample> samples = new List<MotorSample>();
            using (var reader = new StreamReader(csvPath))
            using (var logWriter = new StreamWriter(logPath))
            {
                string headerLine = reader.ReadLine(); 
                int rowCount = 0;
                int lineNumber = 1;

                while (!reader.EndOfStream && rowCount < maxRows)
                {
                    lineNumber++;
                    string line = reader.ReadLine();
                    var values = line.Split(',');

                    try
                    {
                        if (values.Length < 12) 
                        {
                            logWriter.WriteLine($"[Error] Line {lineNumber}: not enough columns ({values.Length})");
                            continue;
                        }

                        var sample = new MotorSample
                        {
                            I_q = double.Parse(values[7], CultureInfo.InvariantCulture),  
                            I_d = double.Parse(values[6], CultureInfo.InvariantCulture),  
                            Coolant = double.Parse(values[1], CultureInfo.InvariantCulture), 
                            Ambient = double.Parse(values[10], CultureInfo.InvariantCulture), 
                            Torque = double.Parse(values[11], CultureInfo.InvariantCulture),  
                            ProfileId = int.Parse(values[12], CultureInfo.InvariantCulture)   
                        };

                        samples.Add(sample);
                        rowCount++;
                    }
                    catch (Exception ex)
                    {
                        logWriter.WriteLine($"[Error] Line {lineNumber}: {ex.Message}");
                    }
                }
            }
            return samples;
        }
    }
}

