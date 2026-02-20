using Microsoft.AspNetCore.Mvc;

namespace BookNest.MVC.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
