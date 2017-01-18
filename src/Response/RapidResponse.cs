using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RapidAPISDK.Response
{
    [JsonConverter(typeof(RapidResponseConverter))]
    internal class RapidResponse<T>
    {

        #region C'tor

        public RapidResponse(T payload)
        {
            Payload = payload;
        }

        public RapidResponse(RapidAPIServerException rapidApiServerException)
        {
            RapidApiServerException = rapidApiServerException;
        }

        #endregion

        #region Properties

        public RapidAPIServerException RapidApiServerException { get; set; }

        public T Payload { get; set; }

        public bool IsSuccess
        {
            get { return RapidApiServerException == null; }
        }

        #endregion

    }

}
