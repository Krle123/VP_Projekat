using Library;
using System;
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
        [OperationBehavior(AutoDisposeParameters = true)]
        public SessionResult StartSession()
        {
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
                Console.WriteLine("Transfer in progress....");
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

                Console.WriteLine("Transfer complete");
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
    }
}
