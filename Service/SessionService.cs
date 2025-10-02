using Library;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SessionService : ISessionService
    {
        private StreamWriter measurements_sw = null;
        private StreamWriter rejects_sw = null;
        private readonly IValidateService validate_service = new ValidateService();
        private readonly Events events = new Events();

        [OperationBehavior(AutoDisposeParameters = true)]
        public SessionResult StartSession()
        {
            events.ElectricSpikeQ += OnElectricSpikeQ;
            events.ElectricSpikeD += OnElectricSpikeD;
            events.TemperatureSpike += OnTemperatureSpike;

            var results = new SessionResult();
            try
            {
                measurements_sw = new StreamWriter("measurements_session.csv");
                if (measurements_sw == null)
                {
                    results.ResultMessage = "Failed to create file: measurements_session.csv";
                    results.ResultType = ResultType.Warning;
                    return results;
                }
                rejects_sw = new StreamWriter("rejects.csv");
                if (rejects_sw == null)
                {
                    results.ResultMessage = "Failed to create file: rejects.csv";
                    results.ResultType = ResultType.Warning;
                    return results;
                }
                events.RaiseTransferStarted();
            }
            catch (Exception ex)
            {
                results.ResultType = ResultType.Failed;
                results.ResultMessage = ex.Message;
            }
            results.ResultMessage = "Succesfully started session";
            results.ResultType = ResultType.Success;
            return results;
        }
        [OperationBehavior(AutoDisposeParameters = true)]
        public SessionResult PushSample(MotorSample sample)
        {
            var results = new SessionResult();
            try
            {
                events.RaiseSampleReceived(sample);
                validate_service.Validate(sample);
                if (measurements_sw == null)
                {
                    results.ResultMessage = "Failed to open file: measurements_session.csv";
                    results.ResultType = ResultType.Warning;
                    return results;
                }
                measurements_sw.WriteLine(sample.ToString());
            }
            catch (FaultException<ValidationFault>)
            {
                if (rejects_sw == null)
                {
                    results.ResultMessage = "Failed to open file: rejects.csv";
                    results.ResultType = ResultType.Warning;
                    return results;
                }
                rejects_sw.WriteLine(sample.ToString());
            }
            catch (Exception ex)
            {
                results.ResultType = ResultType.Failed;
                results.ResultMessage = ex.Message;
                Console.WriteLine(ex.Message);
                return results;
            }
            results.ResultMessage = "Succesfully sent sample";
            results.ResultType = ResultType.Success;
            return results;
        }
        [OperationBehavior(AutoDisposeParameters = true)]
        public SessionResult EndSession()
        {
            var results = new SessionResult();
            try
            {
                measurements_sw.Close();
                rejects_sw.Close();

                events.ElectricSpikeQ -= OnElectricSpikeQ;
                events.ElectricSpikeD -= OnElectricSpikeD;
                events.TemperatureSpike -= OnTemperatureSpike;

                events.RaiseTransferCompleted();
            }
            catch (Exception ex)
            {
                results.ResultMessage = ex.Message;
                results.ResultType = ResultType.Failed;
            }
            results.ResultMessage = "Succesfully closed session.";
            results.ResultType = ResultType.Success;
            return results;
        }

        public void OnElectricSpikeQ(object sender, SpikeEventArgs e)
        {
            if (e.AboveExpected)
                Console.WriteLine("ALARM: I_q is above threshold");
            else
                Console.WriteLine("ALARM: I_q is below threshold");
        }

        public void OnElectricSpikeD(object sender, SpikeEventArgs e)
        {
            if (e.AboveExpected)
                Console.WriteLine("ALARM: I_d is above threshold");
            else
                Console.WriteLine("ALARM: I_d is below threshold");
        }

        public void OnTemperatureSpike(object sender, SpikeEventArgs e)
        {
            if (e.AboveExpected)
                Console.WriteLine("ALARM: Temperature is above threshold");
            else
                Console.WriteLine("ALARM: Temperature is below threshold");
        }
    }
}