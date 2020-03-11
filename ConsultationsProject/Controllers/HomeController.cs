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
    /// <summary>
    /// Контроллер, ответственный за обработку запросов главной страницы сайта (списка пациентов).
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Количество пациентов на страницу (задел для пагинации)
        /// </summary>
        private int PageSize = 10;

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Конструктор контроллера. 
        /// </summary>
        /// <param name="logger">Объект логгера</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Метод, возвращающий список пациентов.
        /// </summary>
        /// <param name="message">Сообщение об успешном добавлении/редактировании/удалении пациента.</param>
        /// <param name="page">Номер страницы списка пациентов.</param>
        /// <returns>
        /// Представление одной страницы со списком пациентов.
        /// </returns>
        public IActionResult Index(int page = 1, string message = "")
        {
            ViewBag.Message = message;
            using (PatientsContext db = new PatientsContext())
            {
                var result = db.Patients
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
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
