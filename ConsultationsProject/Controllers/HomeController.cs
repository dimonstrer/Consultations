using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConsultationsProject.Models;

namespace ConsultationsProject.Controllers
{
    public class HomeController : Controller
    {
        private int PageSize = 10;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int page = 1)
        {
            using (PatientsContext db = new PatientsContext())
            {
                var result = db.Patients
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                return View(result);
            }
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
