using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole
{
    public enum ErrorCode
    {
        Error,
        OutOfResource
    }
    public class TalentGraberException : Exception
    {
        private HttpStatusCode statusCode;
        public TalentGraberException(HttpStatusCode statusCode, string msg = null) : base(msg)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return statusCode;
            }

            set
            {
                statusCode = value;
            }
        }
    }
}
