using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    [DataContract]
    public enum ResultType
    {
        [EnumMember]
        Success,
        [EnumMember]
        Warning,
        [EnumMember]
        Failed
    }
    [DataContract]
    public class SessionResult
    {
        [DataMember]
        public string ResultMessage { get; set; }
        [DataMember]
        public ResultType ResultType { get; set; }

        public SessionResult()
        {
            ResultType = ResultType.Success;
        }
    }
}
