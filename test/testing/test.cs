using System;
using RapidAPISDK;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace test
{

    class Test
    {

        #region Helpers

        private static RapidAPI RapidApi = new RapidAPI("MachlufTest", "edf34f67-d14d-4608-ab44-9b44d37d16d3");

        private static T Call<T>(string pack, string block, params Parameter[] parameters)
        {
            try
            {
                var res = RapidApi.CallAsync<T>(pack, block, parameters).Result;
                Console.WriteLine("success: " + res);
                return res;
            }
            catch (RapidAPIException e)
            {
                Console.WriteLine("Server error: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown exeption: " + e);
            }
            finally
            {
                Console.ReadKey();
            }

            return default(T);
        }

        #endregion Helpers

        #region Tests

        private static void TestPublicPack()
        {
            var apiKey = new DataParameter("apiKey");
            var date = new DataParameter("date");
            var highResolution = new DataParameter("highResolution");

            var pic = Call<NasaPicOfDay>("NasaAPI", "getPictureOfTheDay", apiKey, date, highResolution);
            if (pic == null) return;

            Console.WriteLine(pic.Explanation);
            Console.WriteLine(pic.Url);
        }

        private static void TestPack()
        {
            var apiKey = new DataParameter("apiKey", "edf34f67-d14d-4608-ab44-9b44d37d16d3");
            var str = new DataParameter("string", "מבחן");
            var targetLanguage = new DataParameter("targetLanguage", "en");
            var sourceLanguage = new DataParameter("sourceLanguage");

            Call<object>("GoogleTranslate", "translateAutomatic", apiKey, str, targetLanguage, sourceLanguage);
        }


        private static void TestPackWithImg()
        {
            var subscriptionKey = new DataParameter("subscriptionKey", "edf34f67-d14d-4608-ab44-9b44d37d16d3");
            var image = new DataParameter("image", "http://cdn.litlepups.net/2015/08/31/cute-dog-baby-wallpaper-hd-21.jpg");
            var width = new DataParameter("width", "50");
            var height = new DataParameter("height", "50");
            var smartCropping = new DataParameter("smartCropping");

            Call<object>("MicrosoftComputerVision", "analyzeImage", subscriptionKey, image, width, height, smartCropping);
        }

        #endregion Tests


        static void Main(string[] args)
        {
            TestPublicPack();
            TestPackWithImg();
            TestPack();

        }

        private class NasaPicOfDay
        {
            public string Copyright { get; set; }

            public DateTime Date { get; set; }

            public string Explanation { get; set; }

            public string HdUrl { get; set; }

            public string Url { get; set; }

            [JsonProperty("media_type")]
            public string MediaType { get; set; }

            public string Title { get; set; }
        }

    }
}
