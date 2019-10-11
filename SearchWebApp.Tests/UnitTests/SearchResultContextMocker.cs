using Microsoft.EntityFrameworkCore;
using SearchWebApp.Data;

namespace SearchWebApp.Tests.UnitTests
{
    class SearchResultContextMocker
    {
        public static SearchResultContext GetSearchResultContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SearchResultContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new SearchResultContext(options);

            dbContext.Seed();

            return dbContext;
        }
    }
}
