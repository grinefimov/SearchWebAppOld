using System.ComponentModel.DataAnnotations;

namespace SearchWebApp.Models
{
    public class SearchResult
    {
        public int Id { get; set; }
        [Required]
        public string SearchString { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Snippet { get; set; }

    }
}
