using Microsoft.AspNetCore.Mvc;
using Infotecs.RssFeeder.Models;
using System.Diagnostics;
using Infotecs.RssFeeder.Services;

namespace Infotecs.RssFeeder.Controllers
{
	public class HomeController : Controller
	{ 

		public IActionResult Index()
		{
			return View(RssService.Feeds);
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