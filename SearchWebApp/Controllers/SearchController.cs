using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SearchWebApp.Controllers
{
    public class SearchController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}