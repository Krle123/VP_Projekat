using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    [ServiceContract]
    public interface ISessionService : IDisposable
    {
        [OperationContract]
        SessionResult StartSession();
        [OperationContract]
        SessionResult PushSample(MotorSample sample);
        [OperationContract]
        SessionResult EndSession();
    }
}
