
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookNest.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

      
    }
}
