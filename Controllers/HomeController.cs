using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Facebook.Models;
using Microsoft.AspNetCore.Authorization;
using Facebook.DataBaseContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Facebook.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PostContext _postContext;

        public HomeController(ILogger<HomeController> logger, PostContext postContext)
        {
            _logger = logger;
            _postContext = postContext;
        }

        // GET: Home Page
        public async Task<IActionResult> Index()
        {
            return View(await _postContext.Posts.ToListAsync());
        }

        // GET: User Profile Page
        public async Task<IActionResult> Profile()
        {
            return View(await _postContext.Posts.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
