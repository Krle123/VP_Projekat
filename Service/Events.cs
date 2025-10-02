using Library;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SampleEventArgs : EventArgs
    {
        public MotorSample Sample { get; }
        public SampleEventArgs(MotorSample sample)
        {
            Sample = sample;
        }
    }

    public class SpikeEventArgs : EventArgs
    {
        public bool AboveExpected { get; }
        public SpikeEventArgs(bool aboveExpected)
        {
            AboveExpected = aboveExpected;
        }
    }

    public class WarningEventArgs : EventArgs
    {
        public string Message { get; }
        public MotorSample Sample { get; }

        public WarningEventArgs(string message, MotorSample sample = null)
        {
            Message = message;
            Sample = sample;
        }
    }


    public class OutOfBandWarningEventArgs : EventArgs
    {
        public string Direction { get; }
        public MotorSample Sample { get; }

        public OutOfBandWarningEventArgs(string direction, MotorSample sample = null)
        {
            Direction = direction;
            Sample = sample;
        }
    }



    public class Events
    {
        public static event EventHandler OnTransferStarted;
        public static event EventHandler<SampleEventArgs> OnSampleReceived;
        public static event EventHandler OnTransferCompleted;
        public static event EventHandler<WarningEventArgs> OnWarningRaised;
        public event EventHandler<SpikeEventArgs> ElectricSpikeQ;
        public event EventHandler<SpikeEventArgs> ElectricSpikeD;
        public event EventHandler<SpikeEventArgs> TemperatureSpike;
        public static event EventHandler<OutOfBandWarningEventArgs> OnOutOfBandWarning;

        private static readonly double T_threshold = double.Parse(ConfigurationManager.AppSettings["T_threshold"]);
        private static readonly double Iq_threshold = double.Parse(ConfigurationManager.AppSettings["Iq_threshold"]);
        private static readonly double Id_threshold = double.Parse(ConfigurationManager.AppSettings["Id_threshold"]);
        private static readonly double OutOfBandThreshold = double.Parse(ConfigurationManager.AppSettings["OutOfBandThreshold"]);

        private static readonly double DeviationPercent = 0.25;

        private static double sumT = 0;
        private static int count = 0;

        private static bool firstSample = true;

        private static double currentI_q = 0;
        private static double currentI_d = 0;
        private static double currentCoolant = 0;

        private static double lastI_q = 0;
        private static double lastI_d = 0;
        private static double lastCoolant = 0;

        private static double differenceI_q = 0;
        private static double differenceI_d = 0;
        private static double differenceCoolant = 0;

        public void RaiseTransferStarted()
        {
            OnTransferStarted?.Invoke(null, EventArgs.Empty);
            Console.WriteLine("Transfer started.....");
        }

        public void RaiseSampleReceived(MotorSample sample)
        {
            OnSampleReceived?.Invoke(null, new SampleEventArgs(sample));
            Console.WriteLine($"Sample {++count} received ");

            sumT += sample.Coolant;

            double avgT = sumT / count;

            if (firstSample)
            {
                currentI_q = sample.I_q;
                currentI_d = sample.I_d;
                currentCoolant = sample.Coolant;
                firstSample = false;
            }
            else
            {
                lastI_q = currentI_q;
                lastI_d = currentI_d;
                lastCoolant = currentCoolant;
                currentI_q = sample.I_q;
                currentI_d = sample.I_d;
                currentCoolant = sample.Coolant;

                differenceI_q = currentI_q - lastI_q;
                differenceI_d = currentI_d - lastI_d;
                differenceCoolant = currentCoolant - lastCoolant;

                if (Math.Abs(differenceI_q) > Iq_threshold)
                    ElectricSpikeQ(null, new SpikeEventArgs(differenceI_q >= 0));
                if (Math.Abs(differenceI_d) > Id_threshold)
                    ElectricSpikeD(null, new SpikeEventArgs(differenceI_d >= 0));
                if (Math.Abs(differenceCoolant) > T_threshold)
                    TemperatureSpike(null, new SpikeEventArgs(differenceCoolant >= 0));

                if (Math.Abs(sample.Coolant - avgT) > DeviationPercent * avgT)
                    OnWarningRaised?.Invoke(null, new WarningEventArgs($"Torque odstupa više od 25% od proseka ({avgT:F2})", sample));


                double lowerBound = avgT * (1 - OutOfBandThreshold);
                double upperBound = avgT * (1 + OutOfBandThreshold);

                if (sample.Coolant < lowerBound)
                    OnOutOfBandWarning?.Invoke(null, new OutOfBandWarningEventArgs("ispod očekivane vrednosti", sample));
                else if (sample.Coolant > upperBound)
                    OnOutOfBandWarning?.Invoke(null, new OutOfBandWarningEventArgs("iznad očekivane vrednosti", sample));
            }

        }

        public void RaiseTransferCompleted()
        {
            OnTransferCompleted?.Invoke(null, EventArgs.Empty);
            Console.WriteLine("Transfer completed.");
            ResetStats();
        }

        private void ResetStats()
        {
            sumT = 0;
            count = 0;
            firstSample = true;
        }
    }
}
