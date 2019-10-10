﻿using System.Collections;
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

        public IConfiguration Configuration { get; }

        public SearchController(IConfiguration configuration)
        {
            Configuration = configuration;
            _apiKey = Configuration["SerpApi:SerpApiKey"];
        }

        public async Task<IActionResult> Index()
        {
            var searchString = "orange";
            searchString = searchString.Trim();

            var ht = new Hashtable {{"q", searchString}, {"num", 15}};
            var bingClient = new BingSearchResultsClient(ht, _apiKey);
            var googleClient = new GoogleSearchResultsClient(ht, _apiKey);

            //var results = new List<Task<List<SearchResult>>>();
            var yandexResult = await SearchInYandex(searchString, Configuration["Yandex:SearchStringPart1"],
                Configuration["Yandex:SearchStringPart2"]);
            var googleResult = await SearchWithSerpApi(searchString, googleClient);
            var bingResult = await SearchWithSerpApi(searchString, bingClient);

            return View(googleResult);
        }

        private async Task<List<SearchResult>> SearchInYandex(string searchString, string searchStringPart1,
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

        private async Task<List<SearchResult>> SearchWithSerpApi(string searchString, SerpApiClient client)
        {
            var data = client.GetJson();
            var results = (JArray) data["organic_results"];
            var result = new List<SearchResult>();
            for (var i = 0; i < 10 && i < results.Count; i++)
            {
                var sr = new SearchResult();
                sr.SearchString = searchString;
                sr.Url = results[i]["link"].ToString();
                sr.Title = results[i]["title"].ToString();
                sr.Snippet = results[i]["snippet"] != null ? results[i]["snippet"].ToString() : "";

                result.Add(sr);
            }

            return result;
        }
    }
}