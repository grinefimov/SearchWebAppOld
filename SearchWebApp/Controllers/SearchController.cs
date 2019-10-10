using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SearchWebApp.Models;
using SerpApi;

namespace SearchWebApp.Controllers
{
    public class SearchController : Controller
    {
        public IConfiguration Configuration { get; }

        public SearchController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var searchString = "orange";
            searchString = searchString.Trim();
            //var results = new List<Task<List<SearchResult>>>();
            //var yandexResult = await SearchInYandex(searchString, Configuration["Yandex:SearchStringPart1"],
            //    Configuration["Yandex:SearchStringPart2"]);
            //var googleResult = await SearchInGoogle(searchString, Configuration["SerpApi:SerpApiKey"]);
            var bingResult = await SearchInBing(searchString, Configuration["SerpApi:SerpApiKey"]);


            return View(bingResult);
        }

        private async Task<List<SearchResult>> SearchInBing(string searchString, string apiKey)
        {
            Hashtable ht = new Hashtable();
            ht.Add("q", searchString);
            ht.Add("num", 15);

            BingSearchResultsClient client = new BingSearchResultsClient(ht, apiKey);
            JObject data = client.GetJson();

            JArray results = (JArray) data["organic_results"];
            List<SearchResult> result = new List<SearchResult>();
            for (int i = 0; i < results.Count || i < 10; i++)
            {
                var sr = new SearchResult();
                sr.SearchString = searchString;
                sr.Url = results[i]["link"].ToString();
                sr.Title = results[i]["title"].ToString();
                if (results[i]["snippet"] != null)
                {
                    sr.Snippet = results[i]["snippet"].ToString();
                }
                else
                {
                    sr.Snippet = "";
                }

                result.Add(sr);
            }

            return result;
        }

        private async Task<List<SearchResult>> SearchInGoogle(string searchString, string apiKey)
        {
            Hashtable ht = new Hashtable();
            ht.Add("q", searchString);
            ht.Add("num", 15);

            GoogleSearchResultsClient client = new GoogleSearchResultsClient(ht, apiKey);
            JObject data = client.GetJson();
            
            JArray results = (JArray) data["organic_results"];
            List<SearchResult> result = new List<SearchResult>();
            for (int i = 0; i < results.Count || i < 10; i++)
            {
                var sr = new SearchResult();
                sr.SearchString = searchString;
                sr.Url = results[i]["link"].ToString();
                sr.Title = results[i]["title"].ToString();
                if (results[i]["snippet"] != null)
                {
                    sr.Snippet = results[i]["snippet"].ToString();
                }
                else
                {
                    sr.Snippet = "";
                }

                result.Add(sr);
            }

            return result;
        }

        private async Task<List<SearchResult>> SearchInYandex(string searchString, string searchStringPart1,
            string searchStringPart2)
        {
            string uri = searchStringPart1 + searchString + searchStringPart2;
            string xmlString;
            using (var wc = new WebClient())
            {
                xmlString = await wc.DownloadStringTaskAsync(uri);
            }

            var xml = new XmlDocument();
            xml.LoadXml(xmlString);

            List<SearchResult> result = new List<SearchResult>();
            foreach (XmlNode doc in xml.DocumentElement.SelectNodes(
                "/yandexsearch/response/results/grouping/group/doc"))
            {
                var sr = new SearchResult();
                sr.SearchString = searchString;
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
    }
}