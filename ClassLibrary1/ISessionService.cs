using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    [ServiceContract]
    public interface ISessionService
    {
        [OperationContract]
        SessionResult StartSession();
        [OperationContract]
        SessionResult PushSample();
        [OperationContract]
        SessionResult EndSession();
    }
}
