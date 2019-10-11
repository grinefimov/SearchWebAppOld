using System.Collections;
using System.Collections.Generic;
using SearchWebApp.Models;
using SerpApi;
using Xunit;

namespace SearchWebApp.Tests.UnitTests
{
    public class SearchWithApiTests
    {
        [Fact]
        public async void SearchInYandexAsync_ReturnSearchResultsListWithTenElements_WithNotEmptySearchString()
        {
            // Arrange
            var searchString = "test";
            var yandexSearchString =
                "https://yandex.com/search/xml?user=grinefimov&key=03.44608562:de43bbf914fad85361a7c78d6dbd7165&" +
                "query=[query]&l10n=en&sortby=rlv&filter=none&maxpassages=1&" +
                "groupby=attr%3D%22%22.mode%3Dflat.groups-on-page%3D10.docs-in-group%3D1&page=0";

            // Act
            var result = await SearchWithApi.SearchInYandexAsync(searchString, yandexSearchString);

            // Assert
            Assert.IsType<List<SearchResult>>(result);
            Assert.Equal(10, result.Count);
        }

        [Fact]
        public void SearchWithSerpApi_ReturnSearchResultsListWithTenElements_WithNotEmptySearchString()
        {
            // Arrange
            var searchString = "test";
            var ht = new Hashtable {{"q", searchString}, {"num", 15}};
            var apiKey = "tempApiKey";
            var client = new GoogleSearchResultsClient(ht, apiKey);

            // Act
            var result = SearchWithApi.SearchWithSerpApi(searchString, client);

            // Assert
            Assert.IsType<List<SearchResult>>(result);
            Assert.Equal(10, result.Count);
        }
    }
}