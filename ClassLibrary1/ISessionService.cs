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
        void StartSession();
        [OperationContract]
        void PushSample();
        [OperationContract]
        void EndSession();
    }
}
