using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Ha.Models;
using Shopping_Ha.Repository;
using System.Diagnostics;

namespace Shopping_Ha.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        //public IActionResult Index()
        //{
        //    var products = _dataContext.Products.Include("Category").Include("Brand").ToList();
        //    return View(products);
        //}
        public IActionResult Index()
        {
            var products = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToList();

            return View(products);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if(statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

            }
        }
    }
}
