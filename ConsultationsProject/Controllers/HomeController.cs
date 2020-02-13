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
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Patient patient1 = new Patient
            {
                FirstName = "Dmitry",
                LastName = "Panaev",
                BirthDate = DateTime.Parse("01/30/2000"),
                Patronymic = "Vladimirovich",
                PensionNumber = "None"
            };
            Patient patient2;
            using(PatientsContext db = new PatientsContext())
            {
                db.Patients.Add(patient1);
                db.SaveChanges();
                patient2 = db.Patients.FirstOrDefault();
            }
            return View(patient2);
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
