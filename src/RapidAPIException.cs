using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RapidAPISDK
{

    public class RapidAPIException : Exception
    {
        public RapidAPIException(RapidAPIExceptionArgs args) : base(args.Message.Message)
        {
            Code = args.Code;
            StatusCode = args.Message.StatusCode;
        }

        public string Code { get; }

        public HttpStatusCode StatusCode { get; }

        #region Overrides of Exception

        public override string ToString()
        {
            return $"Code: {Code} , StatusCode: {StatusCode} , Message :{Message}" +
                   $"{base.ToString()}";
        }

        #endregion

        #region Args

        public class RapidAPIExceptionArgs : EventArgs
        {

            [JsonProperty("status_code")]
            public string Code { get; set; }

            [JsonProperty("status_msg")]
            public StatusMessage Message { get; set; }

            public class StatusMessage
            {
                public HttpStatusCode StatusCode { get; set; }

                public string Message { get; set; }
            }

        }

        #endregion
    }

}
