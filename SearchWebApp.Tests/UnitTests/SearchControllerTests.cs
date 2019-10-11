using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SearchWebApp.Controllers;
using SearchWebApp.Data;
using SearchWebApp.Models;
using Xunit;

namespace SearchWebApp.Tests.UnitTests
{
    public class SearchControllerTests
    {
        [Fact]
        public void Index_ReturnsAViewResultWhereSearchViewModelIsNull()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockContext = new Mock<SearchResultContext>();
            var controller = new SearchController(mockConfiguration.Object, mockContext.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewData.Model);
        }

        [Fact]
        public async void Search_SaveResultsToDBAndReturnCorrectModel_WithNotEmptySearchString()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var yandexSearchString =
                "https://yandex.com/search/xml?user=grinefimov&key=03.44608562:de43bbf914fad85361a7c78d6dbd7165&" +
                "query=[query]&l10n=en&sortby=rlv&filter=none&maxpassages=1&" +
                "groupby=attr%3D%22%22.mode%3Dflat.groups-on-page%3D10.docs-in-group%3D1&page=0";
            var apiKey = "tempApiKey";
            mockConfiguration.Setup(c => c["Yandex:SearchString"]).Returns(yandexSearchString);
            mockConfiguration.Setup(c => c["SerpApi:SerpApiKey"]).Returns(apiKey);

            var context = SearchResultContextMocker.GetSearchResultContext(
                nameof(Search_SaveResultsToDBAndReturnCorrectModel_WithNotEmptySearchString));
            var controller = new SearchController(mockConfiguration.Object, context);
            const string searchString = " test ";

            // Act
            var result = await controller.Search(searchString) as ViewResult;
            var databaseData = await context.SearchResults.ToListAsync();
            context.Dispose();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<SearchViewModel>(viewResult.ViewData.Model);
            Assert.Equal("test", model.SearchString);
            Assert.Equal(10, model.SearchResults.Count);
            Assert.Equal(12, databaseData.Count);
        }

        [Fact]
        public void Results_ReturnsAViewResultWhereSearchViewModelIsNull()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockContext = new Mock<SearchResultContext>();
            var controller = new SearchController(mockConfiguration.Object, mockContext.Object);

            // Act
            var result = controller.Results();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewData.Model);
        }

        [Fact]
        public void
            SearchResults_ReturnViewResultWithSearchViewModelWithSearchResultsListWithSingleElement_WithSpecificSearchString()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var context = SearchResultContextMocker.GetSearchResultContext(
                nameof(
                    SearchResults_ReturnViewResultWithSearchViewModelWithSearchResultsListWithSingleElement_WithSpecificSearchString
                ));
            var controller = new SearchController(mockConfiguration.Object, context);
            const string searchString = "SearchString2";

            // Act
            var result = controller.SearchResults(searchString) as ViewResult;
            context.Dispose();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<SearchViewModel>(viewResult.ViewData.Model);
            Assert.Single(model.SearchResults);
        }
    }
}