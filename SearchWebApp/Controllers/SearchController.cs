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
        private readonly string _apiKey;
        private readonly string _searchStringPart1;
        private readonly string _searchStringPart2;

        public IConfiguration Configuration { get; }

        public SearchController(IConfiguration configuration)
        {
            Configuration = configuration;
            _apiKey = Configuration["SerpApi:SerpApiKey"];
            _searchStringPart1 = Configuration["Yandex:SearchStringPart1"];
            _searchStringPart2 = Configuration["Yandex:SearchStringPart2"];
        }

        public IActionResult Index()
        {
            return View(new List<SearchResult>());
        }

        [HttpPost]
        [Route("/Search")]
        public async Task<IActionResult> SearchAsync(string searchString)
        {
            searchString = searchString.Trim();

            var ht = new Hashtable {{"q", searchString}, {"num", 15}};
            var googleClient = new GoogleSearchResultsClient(ht, _apiKey);
            var bingClient = new BingSearchResultsClient(ht, _apiKey);

            var resultTasks = new List<Task<List<SearchResult>>>()
            {
                SearchInYandexAsync(searchString, _searchStringPart1, _searchStringPart2),
                Task.Run(() => SearchWithSerpApi(searchString, googleClient)),
                Task.Run(() => SearchWithSerpApi(searchString, bingClient))
            };

            return View("Index", await Task.WhenAny(resultTasks).Result);
        }

        private async Task<List<SearchResult>> SearchInYandexAsync(string searchString, string searchStringPart1,
            string searchStringPart2)
        {
            var uri = searchStringPart1 + searchString + searchStringPart2;
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

        private List<SearchResult> SearchWithSerpApi(string searchString, SerpApiClient client)
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



        public IActionResult Results()
        {
            return View(new List<SearchResult>());
        }
    }
}