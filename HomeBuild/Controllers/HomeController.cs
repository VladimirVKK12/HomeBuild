using HomeBuild.Data;
using HomeBuild.Models;
using HomeBuild.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HomeBuild.Controllers
{
	public class HomeController : Controller
	{
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly StocksRepository _stocksRepository;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, StocksRepository stocksRepository)
        {
            _stocksRepository = stocksRepository;
            _db = db;
            _logger = logger;
        }
        public IActionResult Index()
        {
            var stocks = _db.Stocks.ToList();

            return View(stocks);
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}