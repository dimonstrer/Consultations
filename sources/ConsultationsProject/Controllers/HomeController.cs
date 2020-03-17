using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConsultationsProject.Models;
using Microsoft.Extensions.Configuration;

namespace ConsultationsProject.Controllers
{
    /// <summary>
    /// Контроллер, ответственный за обработку запросов главной страницы сайта (списка пациентов).
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Количество пациентов на страницу.
        /// </summary>
        private int PatientsPageSize;

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<HomeController> logger;

        /// <summary>
        /// Конфигурация.
        /// </summary>
        private readonly IConfiguration config;

        /// <summary>
        /// Конструктор контроллера. 
        /// </summary>
        /// <param name="logger">Объект логгера.</param>
        /// <param name="config">Конфигурация.</param>
        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
            PatientsPageSize = config.GetValue<int>("PaginationSettings:PatientsPageSize");
        }

        /// <summary>
        /// Метод, возвращающий список пациентов.
        /// </summary>
        /// <param name="message">Сообщение об успешном добавлении/редактировании/удалении пациента.</param>
        /// <param name="page">Номер страницы списка пациентов.</param>
        /// <returns>
        /// Представление одной страницы со списком пациентов.
        /// </returns>
        [Route("{controller=Home}/{action=Index}")]
        [Route("patient-management/patients")]
        public IActionResult Index(int page = 1, string message = "")
        {
            ViewBag.Message = message;
            using (PatientsContext db = new PatientsContext())
            {
                var count = db.Patients.Count();
                PageViewModel pageViewModel = new PageViewModel(count, page, PatientsPageSize);

                if (page <= 0 || page > pageViewModel.TotalPages)
                    page = 1;

                var patients = db.Patients
                    .Skip((page - 1) * PatientsPageSize)
                    .Take(PatientsPageSize)
                    .ToList();

                IndexViewModel result = new IndexViewModel { PageViewModel = pageViewModel, Patients = patients };
                return View(result);
            }
        }

        /// <summary>
        /// Метод, созданный по умолчанию.
        /// </summary>
        /// <returns>
        /// Представление с информацией о приватности.
        /// </returns>
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
