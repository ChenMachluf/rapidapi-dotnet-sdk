using System;
using RapidAPISDK;
using System.Collections.Generic;
using System.Linq;
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
                return res;
            }
            catch (RapidAPIServerException e)
            {
                Console.WriteLine("Server error: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown exeption: " + e);
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


        private static void TestPackWithImg(bool fromFile)
        {
            var subscriptionKey = new DataParameter("subscriptionKey", "fa9a945249f446cd82c8628179132474");
            var image = fromFile ? new FileParameter("image", "dog.jpg") : (Parameter)new DataParameter("image", "http://cdn.litlepups.net/2015/08/31/cute-dog-baby-wallpaper-hd-21.jpg");
            var width = new DataParameter("width", "50");
            var height = new DataParameter("height", "50");
            var smartCropping = new DataParameter("smartCropping");

            var analyze = Call<AnalyzeImage>("MicrosoftComputerVision", "analyzeImage", subscriptionKey, image, width, height, smartCropping);
            if (analyze == null) return;

            Console.WriteLine(analyze.Categories.FirstOrDefault());
        }

        #endregion Tests

        static void Main(string[] args)
        {
            TestPublicPack();
            Console.ReadKey();
            TestPackWithImg(false);
            Console.ReadKey();
            TestPackWithImg(true);
            Console.ReadKey();
            TestPack();
            Console.ReadKey();
        }

        #region Response Classes

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

        private class AnalyzeImage
        {
            public List<Category> Categories { get; set; }

            public Dictionary<string, object> Metadata { get; set; }

            public string RequestId { get; set; }

            public class Category
            {
                public string Name { get; set; }

                public double Score { get; set; }

                #region Overrides of Object

                public override string ToString()
                {
                    return $"{Name} : {Score}";
                }

                #endregion
            }

        }

        #endregion

    }
}
