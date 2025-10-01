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
    public class Events
    {
        public static event EventHandler OnTransferStarted;
        public static event EventHandler<SampleEventArgs> OnSampleReceived;
        public static event EventHandler OnTransferCompleted;
        public static event EventHandler<WarningEventArgs> OnWarningRaised;

        private static readonly double T_threshold = double.Parse(ConfigurationManager.AppSettings["T_threshold"]);
        private static readonly double Iq_threshold = double.Parse(ConfigurationManager.AppSettings["Iq_threshold"]);
        private static readonly double Id_threshold = double.Parse(ConfigurationManager.AppSettings["Id_threshold"]);
        private static readonly double DeviationPercent = 0.25; 

        private static double sumIq = 0, sumId = 0, sumT = 0;
        private static int count = 0;

        public static void RaiseTransferStarted()
        {
            OnTransferStarted?.Invoke(null, EventArgs.Empty);
            Console.WriteLine("[INFO] Transfer started.");
        }

        public static void RaiseSampleReceived(MotorSample sample)
        {
            OnSampleReceived?.Invoke(null, new SampleEventArgs(sample));
            Console.WriteLine($"[INFO] Sample received: {sample}");

            sumIq += sample.I_q;
            sumId += sample.I_d;
            sumT += sample.Torque;
            count++;

            double avgIq = sumIq / count;
            double avgId = sumId / count;
            double avgT = sumT / count;

            if (Math.Abs(sample.I_q) > Iq_threshold)
                OnWarningRaised?.Invoke(null, new WarningEventArgs($"I_q preko praga: {sample.I_q}", sample));
            if (Math.Abs(sample.I_d) > Id_threshold)
                OnWarningRaised?.Invoke(null, new WarningEventArgs($"I_d preko praga: {sample.I_d}", sample));
            if (Math.Abs(sample.Torque) > T_threshold)
                OnWarningRaised?.Invoke(null, new WarningEventArgs($"Torque preko praga: {sample.Torque}", sample));

            if (Math.Abs(sample.I_q - avgIq) > DeviationPercent * avgIq)
                OnWarningRaised?.Invoke(null, new WarningEventArgs($"I_q odstupa više od 25% od proseka ({avgIq:F2})", sample));
            if (Math.Abs(sample.I_d - avgId) > DeviationPercent * avgId)
                OnWarningRaised?.Invoke(null, new WarningEventArgs($"I_d odstupa više od 25% od proseka ({avgId:F2})", sample));
            if (Math.Abs(sample.Torque - avgT) > DeviationPercent * avgT)
                OnWarningRaised?.Invoke(null, new WarningEventArgs($"Torque odstupa više od 25% od proseka ({avgT:F2})", sample));
        }

        public static void RaiseTransferCompleted()
        {
            OnTransferCompleted?.Invoke(null, EventArgs.Empty);
            Console.WriteLine("[INFO] Transfer completed.");
            ResetStats();
        }

        private static void ResetStats()
        {
            sumIq = sumId = sumT = 0;
            count = 0;
        }
    }
}
