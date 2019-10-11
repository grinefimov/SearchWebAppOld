using SearchWebApp.Data;
using SearchWebApp.Models;

namespace SearchWebApp.Tests.UnitTests
{
    public static class SearchResultContextExtensions
    {
        public static void Seed(this SearchResultContext dbContext)
        {
            dbContext.SearchResults.Add(new SearchResult()
            {
                Id = 1,
                SearchString = "SearchString1",
                SearchService = "1",
                Url = "1",
                Title = "1",
                Snippet = "1"
            });

            dbContext.SearchResults.Add(new SearchResult()
            {
                Id = 2,
                SearchString = "SearchString2",
                SearchService = "2",
                Url = "2",
                Title = "2",
                Snippet = "2"
            });

            dbContext.SaveChanges();
        }
    }
}