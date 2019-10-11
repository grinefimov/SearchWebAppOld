using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SearchWebApp.Data;
using SearchWebApp.Models;
using SerpApi;

namespace SearchWebApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly string _apiKey;
        private readonly string _yandexSearchString;
        private readonly SearchResultContext _context;

        public IConfiguration Configuration { get; }

        public SearchController(IConfiguration configuration, SearchResultContext context)
        {
            Configuration = configuration;
            _yandexSearchString = Configuration["Yandex:SearchString"];
            _apiKey = Configuration["SerpApi:SerpApiKey"];
            _context = (SearchResultContext) context;
        }

        public IActionResult Index()
        {
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return View("Index", null);
            }

            searchString = searchString.Trim();

            var results = await GetSearchWithApiResults(searchString);

            _context.SearchResults.AddRange(results);
            await _context.SaveChangesAsync();

            var model = new SearchViewModel()
            {
                SearchResults = results,
                SearchString = searchString
            };

            return View("Index", model);
        }

        public IActionResult Results()
        {
            return View(null);
        }

        public IActionResult SearchResults(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return View("Results", null);
            }

            searchString = searchString.Trim();

            var results = _context.SearchResults
                .Where(r => r.SearchString.Contains(searchString) ||
                            r.Url.Contains(searchString) ||
                            r.Title.Contains(searchString) ||
                            r.Snippet.Contains(searchString))
                .ToList();

            var model = new SearchViewModel()
            {
                SearchResults = results,
                SearchString = searchString
            };

            return View("Results", model);
        }

        private async Task<List<SearchResult>> GetSearchWithApiResults(string searchString)
        {
            var ht = new Hashtable { { "q", searchString }, { "num", 15 } };
            var googleClient = new GoogleSearchResultsClient(ht, _apiKey);
            var bingClient = new BingSearchResultsClient(ht, _apiKey);

            var resultTasks = new List<Task<List<SearchResult>>>()
            {
                SearchWithApi.SearchInYandexAsync(searchString, _yandexSearchString),
                Task.Run(() => SearchWithApi.SearchWithSerpApi(searchString, googleClient)),
                Task.Run(() => SearchWithApi.SearchWithSerpApi(searchString, bingClient))
            };

            var results = await Task.WhenAny(resultTasks).Result;
            return results;
        }
    }
}