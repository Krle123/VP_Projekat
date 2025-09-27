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
        private FileStream measurements_fs = null;
        private FileStream rejects_fs = null;
        //strukturu measurements_session.csv
        //rejects.csv
        [OperationBehavior(AutoDisposeParameters = true)]
        public SessionResult StartSession()
        {
            var results = new SessionResult();
            try 
            {
                measurements_fs = new FileStream("measurements_session.csv", FileMode.Create, FileAccess.Write);
                if (measurements_fs == null)
                {
                    results.ResultMessage = "Failed to create file: measurements_session.csv";
                    results.ResultType = ResultType.Warning;
                    return results;
                }
                rejects_fs = new FileStream("rejects.csv", FileMode.Create, FileAccess.Write);
                if (rejects_fs == null)
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
        public SessionResult PushSample()
        {
            throw new NotImplementedException();
        }
        [OperationBehavior(AutoDisposeParameters = true)]
        public SessionResult EndSession()
        {
            throw new NotImplementedException();
        }
    }
}
