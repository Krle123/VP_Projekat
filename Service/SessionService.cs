using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Service
{
    public class SessionService : ISessionService
    {
        private StreamWriter measurements_sw = null;
        private StreamWriter rejects_sw = null;
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
                bool validated = false; //Validate();
                if (validated)
                {
                    if (measurements_sw == null)
                    {
                        results.ResultMessage = "Failed to open file: measurements_session.csv";
                        results.ResultType = ResultType.Warning;
                        return results;
                    }
                    measurements_sw.Write(sample.ToString());
                }
                else
                {
                    if (rejects_sw == null)
                    {
                        results.ResultMessage = "Failed to open file: rejects.csv";
                        results.ResultType = ResultType.Warning;
                        return results;
                    }
                    rejects_sw.WriteLine(sample.ToString());
                }
            }
            catch (Exception ex)
            {
                results.ResultType = ResultType.Failed;
                results.ResultMessage = ex.Message;
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
