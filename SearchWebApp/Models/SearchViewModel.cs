using System.Collections.Generic;

namespace SearchWebApp.Models
{
    public class SearchViewModel
    {
        public List<SearchResult> SearchResults { get; set; }
        public string SearchString { get; set; }

        public SearchViewModel()
        {
            SearchResults = new List<SearchResult>();
        }
    }
}