using Microsoft.EntityFrameworkCore;
using SearchWebApp.Models;

namespace SearchWebApp.Data
{
    public class SearchResultContext : DbContext
    {
        public SearchResultContext(DbContextOptions<SearchResultContext> options) : base(options)
        {
        }

        public DbSet<SearchResult> SearchResults { get; set; }
    }
}