using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using SearchWebApp.Models;
using SerpApi;

namespace SearchWebApp.Helpers
{
    public static class SearchWithApi
    {
        public static async Task<List<SearchResult>> SearchInYandexAsync(string searchString, string yandexSearchString)
        {
            var uri = yandexSearchString.Replace("[query]", searchString);
            string xmlString;
            using (var wc = new WebClient())
            {
                xmlString = await wc.DownloadStringTaskAsync(uri);
            }

            var xml = new XmlDocument();
            xml.LoadXml(xmlString);

            var result = new List<SearchResult>();
            foreach (XmlNode doc in xml.DocumentElement.SelectNodes(
                "/yandexsearch/response/results/grouping/group/doc"))
            {
                var sr = new SearchResult();
                sr.SearchString = searchString;
                sr.SearchService = "Yandex";
                sr.Url = doc.SelectSingleNode("url").InnerText;
                sr.Title = doc.SelectSingleNode("title").InnerText;
                if (doc.SelectSingleNode("headline") != null)
                {
                    sr.Snippet = doc.SelectSingleNode("headline").InnerText;
                }
                else if (doc.SelectSingleNode("passages/passage") != null)
                {
                    sr.Snippet = doc.SelectSingleNode("passages/passage").InnerText;
                }
                else sr.Snippet = "";

                result.Add(sr);
            }

            return result;
        }

        public static List<SearchResult> SearchWithSerpApi(string searchString, SerpApiClient client)
        {
            var data = client.GetJson();
            var results = (JArray) data["organic_results"];
            var result = new List<SearchResult>();
            for (var i = 0; i < 10 && i < results.Count; i++)
            {
                var sr = new SearchResult();
                sr.SearchString = searchString;
                sr.SearchService = client.GetType() == typeof(GoogleSearchResultsClient) ? "Google" : "Bing";
                sr.Url = results[i]["link"].ToString();
                sr.Title = results[i]["title"].ToString();
                sr.Snippet = results[i]["snippet"] != null ? results[i]["snippet"].ToString() : "";

                result.Add(sr);
            }

            return result;
        }
    }
}