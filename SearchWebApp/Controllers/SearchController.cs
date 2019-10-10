using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Extensions.Configuration;
using SearchWebApp.Models;

namespace SearchWebApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly HttpClient _client = new HttpClient();
        //private readonly WebSearchClient _bingClient = new WebSearchClient(new ApiKeyServiceClientCredentials("YOUR_SUBSCRIPTION_KEY"));

        public IConfiguration Configuration { get; }

        public SearchController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var searchString = "pear";
            searchString = searchString.Trim();
            var yandexResult = await SearchInYandex(_client, searchString);
            //var googleResult = await SearchInYandex(_client, "pear");

            return View(yandexResult);
        }


        private async Task<List<SearchResult>> SearchInGoogle(HttpClient client, string searchString)
        {
            List<SearchResult> result = new List<SearchResult>();

            return result;
        }

        private async Task<List<SearchResult>> SearchInYandex(HttpClient client, string searchString)
        {
            string uri = Configuration["Yandex:SearchStringPart1"] + searchString +
                         Configuration["Yandex:SearchStringPart2"];
            string xmlString;
            using (var wc = new WebClient())
            {
                xmlString = await wc.DownloadStringTaskAsync(uri);
            }

            var xml = new XmlDocument();
            xml.LoadXml(xmlString);

            List<SearchResult> result = new List<SearchResult>();
            foreach (XmlNode doc in xml.DocumentElement.SelectNodes("/yandexsearch/response/results/grouping/group/doc")
            )
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

        private async Task<List<string>> SearchInBing(WebSearchClient client)
        {
            List<string> result = new List<string>();

            var webData = await client.Web.SearchAsync(query: "wiki");

            if (webData?.WebPages?.Value?.Count > 0)
            {
                var firstWebPagesResult = webData.WebPages.Value.FirstOrDefault();

                if (firstWebPagesResult != null)
                {
                    result.Add("Webpage Results # " + webData.WebPages.Value.Count);
                    result.Add("First web page name: " + firstWebPagesResult.Name);
                    result.Add("First web page URL: " + firstWebPagesResult.Url);
                }
                else
                {
                    result.Add("Didn't find any web pages...");
                }
            }
            else
            {
                result.Add("Didn't find any web pages...");
            }

            return result;
        }
    }
}