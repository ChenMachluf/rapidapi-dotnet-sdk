using System;
using System.IO;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Runtime.Remoting.Proxies;
using System.Threading.Tasks;
using RapidAPISDK.Response;

namespace RapidAPISDK
{
    public class RapidAPI
    {

        #region Private Static Functions

        private const string BaseUrl = "https://rapidapi.io/connect";

        /***
        * Build a URL for a block call
        * @param pack Package where the block is
        * @param block Block to be called
        * @returns {string} Generated URL
        */
        private string BuildBlockUrl(string pack, string block)
        {
            return $"{BaseUrl}/{pack}/{block}";
        }

        #endregion 

        #region Private Members 

        private readonly HttpClient _Client;

        #endregion 

        #region C'tor


        /***
        * Creates a new RapidAPI Connect instance
        * @param project Name of the _project you are working with
        * @param key API _key for the project
        */
        public RapidAPI(string project, string key)
        {
            _Client = new HttpClient();

            _Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

            var auth = Encoding.ASCII.GetBytes($"{project}:{key}");
            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(auth));
        }

        #endregion

        #region Public Functions

        /***
        * Call a block
        * @param pack Package of the block
        * @param block Name of the block
        * @param parameters Arguments to send to the block (JSON)
        */
        public async Task<object> CallAsync(string pack, string block, params Parameter[] parameters)
        {
            return await CallAsync<object>(pack, block, parameters);
        }


        /***
        * Call a block
        * @param pack Package of the block
        * @param block Name of the block
        * @param parameters Arguments to send to the block (JSON)
        */
        public async Task<T> CallAsync<T>(string pack, string block, params Parameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(pack)) throw new ArgumentException("Cannot be null or empty", nameof(pack));
            if (string.IsNullOrWhiteSpace(block)) throw new ArgumentException("Cannot be null or empty", nameof(block));
            if (parameters == null) parameters = new Parameter[0];

            MultipartFormDataContent form = new MultipartFormDataContent();

            foreach (var parameter in parameters)
                parameter.AddToContent(form);

            var response = await _Client.PostAsync(BuildBlockUrl(pack, block), form);
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var rapidRes = JsonConvert.DeserializeObject<RapidResponse<T>>(content);

                if (!response.IsSuccessStatusCode || !rapidRes.IsSuccess)
                    throw rapidRes.RapidApiServerException;

                return rapidRes.Payload;
            }
            catch (RapidAPIServerException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception("Uknown Error. see inner exception for more details.", e);
            }
        }

        #endregion 

    }
}