using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConsultationsProject.Models;
using Microsoft.Extensions.Configuration;
using ConsultationsProject.Models.Interfaces;
using AutoMapper;
using ConsultationsProject.Models.DTO;

namespace ConsultationsProject.Controllers
{
    /// <summary>
    /// Контроллер, ответственный за обработку запросов главной страницы сайта (списка пациентов).
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
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
        /// Сервис пациентов и консультаций.
        /// </summary>
        private readonly IPatientService patientService;

        /// <summary>
        /// Объект маппера.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Конструктор контроллера. 
        /// </summary>
        /// <param name="logger">Объект логгера.</param>
        /// <param name="config">Конфигурация.</param>
        /// <param name="patientService">Сервис, ответственный за бизнес логику в работе с пациентами и консультациями.</param>
        /// <param name="mapper">Объект маппера.</param>
        public HomeController(ILogger<HomeController> logger, IConfiguration config, IPatientService patientService, IMapper mapper)
        {
            this.logger = logger;
            this.config = config;
            this.patientService = patientService;
            this.mapper = mapper;
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
            try
            {
                ViewBag.Message = message;
                var count = patientService.GetPatients().Count();
                PageViewModel pageViewModel = new PageViewModel(count, page, PatientsPageSize);

                if (page <= 0 || page > pageViewModel.TotalPages)
                    page = 1;

                var patients = patientService.GetPatients()
                    .Skip((page - 1) * PatientsPageSize)
                    .Take(PatientsPageSize)
                    .ToList();

                var patientsDTO = mapper.Map<List<PatientDTO>>(patients);

                IndexViewModel result = new IndexViewModel { PageViewModel = pageViewModel, Patients = patientsDTO };
                return View(result);
            }
            catch (Exception e)
            {
                logger.LogCritical($"Произошла ошибка при получении списка пациентов в базе данных." +
                    $" Страница: {page}", e);
                return View("Error",
                            new ErrorViewModel
                            {
                                Message = $"Произошла ошибка. Не удалось найти пациентов. " +
                                "Обратитесь к администратору."
                            });
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
