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
            var yandexSearchString = "yandexSearchString";

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