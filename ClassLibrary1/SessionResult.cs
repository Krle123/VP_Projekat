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
    public class SessionResult : IDisposable
    {
        private bool disposedValue;
        [DataMember]
        public string ResultMessage { get; set; }
        [DataMember]
        public ResultType ResultType { get; set; }
        [DataMember]
        public MemoryStream MemoryStream { get; set; }

        public SessionResult() 
        {
            ResultType = ResultType.Success;
            MemoryStream = new MemoryStream();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (MemoryStream == null)
                        return;
                    MemoryStream.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
